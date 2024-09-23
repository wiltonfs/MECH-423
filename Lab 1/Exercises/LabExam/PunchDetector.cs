using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exercise4
{
    enum ExpectedNextRead
    {
        LEAD, X, Y, Z
    }

    public partial class PunchDetector : Form
    {
        float gestureTimeout = 1000;

        // UI parameters
        int accelerationAveragingCount = 20;
        int accelerationMaxCount = 100;

        // Gesture detection parameters
        float detectionThreshold = 1000f;
        int millisToIdle = 500;
        string GESTURE_ONE = "Z"; string GESTURE_ONE_NAME = "Hand Up";
        string GESTURE_TWO = "XY"; string GESTURE_TWO_NAME = "Frsibee Throw";
        string GESTURE_THREE = "ZXC"; string GESTURE_THREE_NAME = "Slam Dunk";
        char[] disabledChannels = new char[] {'A', 'B'};

        // Concurrent Queue
        ConcurrentQueue<Int32> dataQueue = new ConcurrentQueue<Int32>();
        // Vec3 Queue
        ExpectedNextRead expectedNextRead = ExpectedNextRead.LEAD;
        List<Vec3> accelerations = new List<Vec3>();
        // Calibration data:  x:151, 125, 100     y: 152, 126, 100    z: 153, 103 
        Vec3 bias = new Vec3(-125f, -126f, -128f);
        Vec3 scale = new Vec3(0.3849f, 0.3773f, 0.3924f);
        // Gesture detection
        int idleTimer = 0;
        string compoundGesture = "";
        bool[] channelRecentlyDetected = new bool[6];
        float[] movementMatch = new float[] {
            0.57735f, 0f, -0.096225f, 0f, 0.096225f, 0.288675f, 0.673575f, 1.5396f, 3.27165f, 
            7.698f, 15.0111f, 20.880825f, 15.396f, -6.5433f, -28.001475f, -21.265725f, -11.450775f, 
            -6.5433f, -3.65655f, -3.65655f, -3.27165f, -3.175425f, -2.88675f, -2.598075f, -2.405625f
        };
        

        public PunchDetector()
        {
            InitializeComponent();
            refreshComboBox();
        }

        private void SerialDemo_Load(object sender, EventArgs e)
        {
            maxLabel.Text = "Max acceleration over last " + accelerationMaxCount + " data points (g):";
            avgLabel.Text = "Avg total acceleration over last " + accelerationAveragingCount + " data points (g):";
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            refreshComboBox();
        }

        private void refreshComboBox()
        {
            // Refresh the combo box with the available COM ports
            serialComboBox.Items.Clear();
            serialComboBox.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
            if (serialComboBox.Items.Count == 0)
                serialComboBox.Text = "No COM ports!";
            else
                serialComboBox.SelectedIndex = 0;
        }

        private void serialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                serialPort1.PortName = serialComboBox.Text;
                serialPort1.Open();
            }
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
        }
        private void dataTimer_Tick(object sender, EventArgs e)
        {
            ReadQueue();
        }

        private void visualTimer_Tick(object sender, EventArgs e)
        {
            gestureTimeout -= visualTimer.Interval;

            if (gestureTimeout < 0)
            {
                gestureDisplayLabel.Text = "";
            }

            DisplayInstantAccel();
            DisplayOrientation();
            AvgAndDisplayLastNPoints();
            MaxAndDisplayLastNPoints();
            preStringVisual.Text = "Current compound gesture: " + compoundGesture;

            // Check the idle timer
            idleTimer -= visualTimer.Interval;
            if (idleTimer < 0)
            {
                // We have idled. Check if we have a valid gesture or not.
                ScanForGestures();
                compoundGesture = "";
                idleTimer = millisToIdle;
            }

        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            int newByte = 0;
            int bytesToRead;
            bytesToRead = serialPort1.BytesToRead;
            while (bytesToRead != 0)
            {
                newByte = serialPort1.ReadByte();
                dataQueue.Enqueue(newByte);

                bytesToRead = serialPort1.BytesToRead;
            }
        }

        // --- Acceleration and Queue Related Functions ---

        private void ReadQueue()
        {
            // Display queue size here
            queueSizeLabel.Text = "Concurrent queue size: " + dataQueue.Count;

            // Display serial buffer size here
            int bytesToRead = 0;
            if (serialPort1.IsOpen)
                bytesToRead = serialPort1.BytesToRead;
            serialBufferSizeLabel.Text = "Serial buffer size: " + bytesToRead;

            int nextVal;
            bool succ = dataQueue.TryDequeue(out nextVal);
            while (succ)
            {
                if (nextVal == 255)
                {
                    // Indicates new data frame
                    expectedNextRead = ExpectedNextRead.X;
                } 
                else
                {
                    float correctedX = (nextVal + bias.X) * scale.X;
                    float correctedY = (nextVal + bias.Y) * scale.Y;
                    float correctedZ = (nextVal + bias.Z) * scale.Z;
                    switch (expectedNextRead)
                    {
                        case ExpectedNextRead.LEAD:
                            break;
                        case ExpectedNextRead.X:
                            accelerations.Add(new Vec3(correctedX, 0.0f, 0.0f));

                            // If we have enough values toss the old one
                            if (accelerations.Count > movementMatch.Length && accelerations.Count > accelerationMaxCount )
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
                            ScanForMovements();
                            expectedNextRead = ExpectedNextRead.LEAD;
                            break;
                        default:
                            break;
                    }
                }

                succ = dataQueue.TryDequeue(out nextVal);
            }
        }

        private void DisplayOrientation()
        {
            Vec3 latestAccel = accelerations.Count > 1 ? accelerations[accelerations.Count - 2] : new Vec3();
            if (accelerations.Count > 0)
                orientationLabel.Text = "The board's top face is the " + latestAccel.UpAxisSign() + " " + latestAccel.UpAxis() + " axis";
        }

        private void DisplayInstantAccel()
        {
            if (accelerations.Count > 0)
            {
                Vec3 latestAccel = accelerations[accelerations.Count - 2];
                if (unitsCheckbox.Checked)
                {
                    aXDisplay.Text = latestAccel.X.ToString();
                    aYDisplay.Text = latestAccel.Y.ToString();
                    aZDisplay.Text = latestAccel.Z.ToString();
                } 
                else
                {
                    // De-convert bias and offset
                    float correctedX = (latestAccel.X / scale.X) - bias.X;
                    float correctedY = (latestAccel.Y / scale.Y) - bias.Y;
                    float correctedZ = (latestAccel.Z / scale.Z) - bias.Z;
                    aXDisplay.Text = correctedX.ToString();
                    aYDisplay.Text = correctedY.ToString();
                    aZDisplay.Text = correctedZ.ToString();
                }
            }
        }

        private void MaxAndDisplayLastNPoints()
        {
            if (accelerations.Count >= accelerationMaxCount)
            {
                // Have enough to search for max
                Vec3 max = VecToG(MaxAccelOverLastN(accelerationMaxCount));

                // Reduce to 3 decimal points
                float X = (float)Math.Round((double)max.X, 3);
                float Y = (float)Math.Round((double)max.Y, 3);
                float Z = (float)Math.Round((double)max.Z, 3);

                // Display average acceleration
                maxXDisplay.Text = X.ToString();
                maxYDisplay.Text = Y.ToString();
                maxZDisplay.Text = Z.ToString();
            }
        }

        private void AvgAndDisplayLastNPoints()
        {
            if (accelerations.Count >= accelerationAveragingCount)
            {
                // Have enough to search for avg
                Vec3 avg = VecToG(AvgAccelOverLastN(accelerationAveragingCount));

                float mag = (float)Math.Round((double)avg.Length(), 3);

                avgAccelDisplay.Text = mag.ToString();
            }
        }

        // ---------------------------------------------------------
        // ---------- Lab Exam Related Functions ----------
        // ---------------------------------------------------------

        private Vec3 VecToG(Vec3 input)
        {
            float scale = 1f / 9.81f;
            return new Vec3(input.X * scale, input.Y * scale, input.Z * scale);
        }

        private Vec3 AvgAccelOverLastN(int n)
        {
            Vec3 avg = new Vec3();

            int offset = accelerations.Count - n;

            for (int i = offset; i < accelerations.Count; i++)
            {

                Vec3 compare = accelerations[i];

                avg.X += compare.X;
                avg.Y += compare.Y;
                avg.Z += compare.Z;
            }

            avg.X /= n;
            avg.Y /= n;
            avg.Z /= n;

            return avg;
        }

        private Vec3 MaxAccelOverLastN(int n)
        {
            Vec3 max = new Vec3();

            int offset = accelerations.Count - n;

            for (int i = offset; i < accelerations.Count; i++)
            {

                Vec3 compare = accelerations[i];

                if (compare.AbsX() > max.AbsX())
                {
                    max.X = compare.X;
                }
                if (compare.AbsY() > max.AbsY())
                {
                    max.Y = compare.Y;
                }
                if (compare.AbsZ() > max.AbsZ())
                {
                    max.Z = compare.Z;
                }
            }

            return max;
        }

        // ---------------------------------------------------------
        // ---------- Gesture Detection Related Functions ----------
        // ---------------------------------------------------------

        private void ScanForGestures()
        {
            // We've idled... do we match a valid gesture?
            if (compoundGesture.Equals(GESTURE_ONE))
            {
                // Gesture One
                GestureDetected(GESTURE_ONE_NAME);
                SoundPlayer player = new SoundPlayer("C:\\Users\\fsant\\Desktop\\MECH-423\\Lab 1\\Exercises\\Exercise8\\SimplePunch.wav");
                player.Play();
            }
            else if (compoundGesture.Equals(GESTURE_TWO))
            {
                // Gesture Two
                GestureDetected(GESTURE_TWO_NAME);
                SoundPlayer player = new SoundPlayer("C:\\Users\\fsant\\Desktop\\MECH-423\\Lab 1\\Exercises\\Exercise8\\HighPunch.wav");
                player.Play();
            }
            else if (compoundGesture.Equals(GESTURE_THREE))
            {
                // Gesture Three
                GestureDetected(GESTURE_THREE_NAME);
                SoundPlayer player = new SoundPlayer("C:\\Users\\fsant\\Desktop\\MECH-423\\Lab 1\\Exercises\\Exercise8\\RightHook.wav");
                player.Play();
            }
            else if (compoundGesture.Equals(""))
            {
                // Idled
                return;
            }
            else
            {
                GestureDetected("Invalid Gesture detected");
            }
            postStringVisual.Text = "Last detected gesture: " + compoundGesture;
        }

        private void GestureDetected(string gesture)
        {
            gestureDisplayLabel.Text = gesture;
            gestureTimeout = 1000;
            // TOOD: fancier detection
        }

        private void ScanForMovements()
        {
            if (accelerations.Count < movementMatch.Length)
            {
                // Not enough values to put the window on
                return;
            }

            // Parameters to check 6 channels
            char[] targets = new char[]         { 'X', 'Y', 'Z', 'A', 'B', 'C' };
            float[] directions = new float[]    { 1f,  1f,  1f,  -1f, -1f, -1f };
            int[] axes = new int[]              {  0,   1,   2,    0,   1,   2 };

            // Iterate through the 6 channels
            for (int j = 0; j < 6; j++)
            {
                char target = targets[j]; float direction = directions[j]; int axis = axes[j];

                if (!disabledChannels.Contains(target)){
                    // Ignore disabled channels
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
                        if (!channelRecentlyDetected[j] && (compoundGesture.Length == 0 || compoundGesture[compoundGesture.Length - 1] != target))
                        {
                            // The target was novel
                            compoundGesture += target;
                            channelRecentlyDetected[j] = true;
                        }
                    }
                    else if (total < detectionThreshold / 2f)
                    {
                        // Far off target, reset our detection tracker for this channel
                        channelRecentlyDetected[j] = false;
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
}
