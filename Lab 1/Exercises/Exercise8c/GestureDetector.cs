using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace Exercise4
{
    enum ExpectedNextRead
    {
        LEAD, X, Y, Z
    }
    public class GestureDetector
    {
        // Averaging parameters
        int accelerationAveragingCount = 100;

        // Gesture detection parameters
        float detectionThreshold = 1000f;
        int millisToIdle = 500;
        char[] disabledChannels = new char[] {};

        // Vec3 Queue
        ExpectedNextRead expectedNextRead = ExpectedNextRead.LEAD;
        List<Vec3> accelerations = new List<Vec3>();
        Vec3 mostRecentAcceleration = new Vec3();
        Vec3 averageAcceleration = new Vec3();

        // Calibration data:  x:151, 125, 100     y: 152, 126, 100    z: 153, 103 
        Vec3 bias = new Vec3(-125f, -126f, -128f);
        Vec3 scale = new Vec3(0.3849f, 0.3773f, 0.3924f);

        // Gesture detection
        int idleTimer = 0;
        string compoundGesture = "";
        string mostRecentGesture = "";
        int[] channelRecentlyDetectedTimer = new int[6];
        float[] movementMatchFull = new float[] {
            0.57735f, 0f, -0.096225f, 0f, 0.096225f, 0.288675f, 0.673575f, 1.5396f, 3.27165f,
            7.698f, 15.0111f, 20.880825f, 15.396f, -6.5433f, -28.001475f, -21.265725f, -11.450775f,
            -6.5433f, -3.65655f, -3.65655f, -3.27165f, -3.175425f, -2.88675f, -2.598075f, -2.405625f
        };
        float[] movementMatchTrim = new float[] {
            0f, 0.288675f, 0.673575f, 1.5396f, 3.27165f, 7.698f, 15.0111f, 20.880825f, 
            15.396f, -6.5433f, -28.001475f, -21.265725f, -11.450775f, -6.5433f, -3.65655f
        };
        float[] movementMatch;

        // Pseudo-timing
        int millisPerValue = 10;

        public GestureDetector() {
            movementMatch = movementMatchFull;
        }

        public void IgnoreChannels(char[] channels)
        {
            disabledChannels = channels;
        }

        public void EnableFastDetection(bool fasterDetection)
        {
            movementMatch = fasterDetection ? movementMatchTrim : movementMatchFull;
            millisToIdle = fasterDetection ? 100 : 500;
        }

        public string PopMostRecentGesture()
        {
            string pop = mostRecentGesture;
            mostRecentGesture = "";
            return pop;
        }

        public string GetPartialGesture()
        { return compoundGesture; }

        public Vec3 GetLatestAcceleration_SI()
        {
            return mostRecentAcceleration;
        }

        public Vec3 GetLatestAcceleration_RAW()
        {
            return DeconvertVec3(GetLatestAcceleration_SI());
        }

        private Vec3 DeconvertVec3(Vec3 SI)
        {
            // De-convert bias and offset
            float correctedX = (SI.X / scale.X) - bias.X;
            float correctedY = (SI.Y / scale.Y) - bias.Y;
            float correctedZ = (SI.Z / scale.Z) - bias.Z;

            return new Vec3(correctedX, correctedY, correctedZ);
        }

        public void SetAveragingCount(int count)
        {
            accelerationAveragingCount = count;
        }

        public int GetAveragingCount() { return accelerationAveragingCount; }

        public Vec3 GetAverageAcceleration_SI()
        {
            int count = accelerations.Count >= accelerationAveragingCount ? accelerationAveragingCount : accelerations.Count;
                
            // Skip to get just the last "count" values
            int skip = accelerations.Count - count;
            float avgX = 0.0f, avgY = 0.0f, avgZ = 0.0f;

            // THIS IS NOT PERFORMANT. ANY LAG SHOULD BE INVESTIGATED HERE
            int i = 0;
            foreach (Vec3 accelVector in accelerations)
            {
                if (i >= skip)
                {
                    avgX += accelVector.X;
                    avgY += accelVector.Y;
                    avgZ += accelVector.Z;
                }
                i++;
            }

            // Process the averages
            avgX = avgX / count;
            avgY = avgY / count;
            avgZ = avgZ / count;

            return new Vec3(avgX, avgY, avgZ);
        }

        public Vec3 GetAverageAcceleration_RAW()
        {
            return DeconvertVec3(GetAverageAcceleration_SI());
        }

        public void FeedSerialValue(int serialValue)
        {
            // Pseudo-timing values
            idleTimer -= millisPerValue;
            for (int i = 0; i < 6; i++)
            {
                if (channelRecentlyDetectedTimer[i] > 0)
                    channelRecentlyDetectedTimer[i] -= millisPerValue;
            }
            CheckForIdle();


            if (serialValue == 255)
            {
                // Indicates new data frame
                expectedNextRead = ExpectedNextRead.X;
            }
            else
            {
                float correctedX = (serialValue + bias.X) * scale.X;
                float correctedY = (serialValue + bias.Y) * scale.Y;
                float correctedZ = (serialValue + bias.Z) * scale.Z;
                switch (expectedNextRead)
                {
                    case ExpectedNextRead.LEAD:
                        break;
                    case ExpectedNextRead.X:
                        accelerations.Add(new Vec3(correctedX, 0.0f, 0.0f));

                        // If we have enough values toss the old one
                        if (accelerations.Count > movementMatchFull.Length && accelerations.Count > accelerationAveragingCount)
                        {
                            accelerations.RemoveAt(0);
                        }

                        expectedNextRead++;
                        break;
                    case ExpectedNextRead.Y:
                        accelerations.Last<Vec3>().Y = correctedY;
                        expectedNextRead++;
                        break;
                    case ExpectedNextRead.Z:
                        accelerations.Last<Vec3>().Z = correctedZ;
                        mostRecentAcceleration = accelerations.Last<Vec3>();
                        ScanForMovements();
                        expectedNextRead = ExpectedNextRead.LEAD;
                        break;
                    default:
                        break;
                }
            }
        }

        // ---------------------------------------------------------
        // ---------- Gesture Detection Related Functions ----------
        // ---------------------------------------------------------

        private void CheckForIdle()
        {
            if (idleTimer < 0 && compoundGesture.Length > 0)
            {
                mostRecentGesture = compoundGesture;
                compoundGesture = "";
                idleTimer = millisToIdle;
            }
        }
        private void ScanForMovements()
        {
            if (accelerations.Count < movementMatch.Length)
            {
                // Not enough values to put the window on
                return;
            }

            // Parameters to check 6 channels
            char[] targets = new char[] { 'X', 'Y', 'Z', 'A', 'B', 'C' };
            float[] directions = new float[] { 1f, 1f, 1f, -1f, -1f, -1f };
            int[] axes = new int[] { 0, 1, 2, 0, 1, 2 };

            // Iterate through the 6 channels
            for (int j = 0; j < 6; j++)
            {
                char target = targets[j]; float direction = directions[j]; int axis = axes[j];

                if (disabledChannels.Contains(target)) { break; } // Ignore disabled channels

                int start = accelerations.Count - movementMatch.Length;
                float total = 0;
                for (int i = 0; i < movementMatch.Length; i++)
                {
                    total += direction * ReachAccelChannel(start + i, axis) * movementMatch[i];
                }
                if (total > detectionThreshold)
                {
                    // Detected the target
                    idleTimer = millisToIdle;
                    if (channelRecentlyDetectedTimer[j] <= 0 && (compoundGesture.Length == 0 || compoundGesture[compoundGesture.Length - 1] != target))
                    {
                        // The target was novel
                        compoundGesture += target;
                        channelRecentlyDetectedTimer[j] = millisPerValue * 4 * movementMatch.Length / 2;
                    }
                }
            }
        }

        private float ReachAccelChannel(int index, int axis)
        {
            if (axis == 0)
            {
                // Read X
                return accelerations[index].X;
            }
            if (axis == 1)
            {
                // Read Y
                return accelerations[index].Y;
            }
            // Read Z without the gravity factor
            return accelerations[index].Z - 9.81f;
        }
    }

    public class Vec3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vec3(float x, float y, float z)
        {
            X = x; Y = y; Z = z;
        }
        public Vec3(float val)
        {
            X = val; Y = val; Z = val;
        }
        public Vec3()
        {
            X = 0; Y = 0; Z = 0;
        }

        public float Length() { return (float)Math.Sqrt(X * X + Y * Y + Z * Z); }

        public float AbsX() { return (float)Math.Abs(X); }
        public float AbsY() { return (float)Math.Abs(Y); }
        public float AbsZ() { return (float)Math.Abs(Z); }

        public string UpAxis()
        {
            float threshold = 5f;

            if (Length() < threshold)
            {
                return "Freefall";
            }

            if (AbsX() > threshold && AbsX() > Math.Max(AbsY(), AbsZ()))
                return "X";
            else if (AbsY() > threshold && AbsY() > Math.Max(AbsX(), AbsZ()))
                return "Y";
            else
                return "Z";
        }

        public int UpAxisSign()
        {
            switch (UpAxis())
            {
                case "X":
                    return (int)(X / AbsX());
                case "Y":
                    return (int)(Y / AbsY());
                case "Z":
                    return (int)(Z / AbsZ());
                default:
                    return 0;
            }
        }

        override public string ToString()
        {
            return "{" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + "}";
        }
    }
}
