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
    enum PunchState
    {
        IDLE,
        X_POS_acc, X_NEG_acc,
        Y_POS_acc, Y_NEG_acc,
        Z_POS_acc, Z_NEG_acc,

        X_POS_2_acc, X_NEG_2_acc,
        Y_POS_2_acc, Y_NEG_2_acc,
        Z_POS_2_acc, Z_NEG_2_acc,
    }

    public partial class PunchDetector : Form
    {
        // UI parameters
        int accelerationAveragingCount = 100;

        // Concurrent Queue
        ConcurrentQueue<Int32> dataQueue = new ConcurrentQueue<Int32>();

        // Vec3 Queue
        ExpectedNextRead expectedNextRead = ExpectedNextRead.LEAD;
        Queue<Vec3> accelQueue = new Queue<Vec3>();
        Vec3 mostRecentAccel = new Vec3();
        // Calibration data:  x:151, 125, 100     y: 152, 126, 100    z: 153, 103 
        Vec3 bias = new Vec3(-125f, -126f, -128f);
        Vec3 scale = new Vec3(0.3849f, 0.3773f, 0.3924f);

        // Gesture detection parameters
        PunchState currentState = PunchState.IDLE;
        Queue<PunchState> stateQueue = new Queue<PunchState>();
        int idlesToPause = 10; // Number of sequential "idles" before considered a "pause"
        int ignoreLessThan = 3; // Number of sequential states before being considered a state
        float thresholdAccel = 8.0f; // Minimum accel to not be idle
        float thresholdAccel2 = 18.0f; // Minimum accel to count as double accel
        string GESTURE_ONE = "X"; string GESTURE_ONE_NAME = "Simple Punch";
        string GESTURE_TWO = "ZX"; string GESTURE_TWO_NAME = "High Punch";
        string GESTURE_THREE = "XYZ"; string GESTURE_THREE_NAME = "Right Hook";
        char[] IGNORED_STATES = {'A', 'B', 'C' };

        public PunchDetector()
        {
            InitializeComponent();
            refreshComboBox();
        }

        private void SerialDemo_Load(object sender, EventArgs e)
        {
            avgLabel.Text = "Average acceleration over last " + accelerationAveragingCount + " data points:";
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
            DisplayInstantAccel();
            DisplayOrientation();
            AvgAndDisplayLastNPoints();
            currentStateDisplay.Text = "Current state: " + currentState;

            // Decay one char from the gesture display
            int l = gestureDisplayLabel.Text.Length;
            if (l > 0)
            {
                //gestureDisplayLabel.Text = gestureDisplayLabel.Text.Substring(0, l - 1);
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
                            accelQueue.Enqueue(new Vec3(correctedX, 0.0f, 0.0f));
                            expectedNextRead++;
                            break;
                        case ExpectedNextRead.Y:
                            accelQueue.Last<Vec3>().Y = correctedY;
                            expectedNextRead++;
                            break;
                        case ExpectedNextRead.Z:
                            accelQueue.Last<Vec3>().Z = correctedZ;
                            mostRecentAccel = accelQueue.Last();
                            StateMachine();
                            ScanForGestures();
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
            if (accelQueue.Count > 0)
                orientationLabel.Text = "The board's top face is the " + mostRecentAccel.UpAxisSign() + " " + mostRecentAccel.UpAxis() + " axis";
        }

        private void DisplayInstantAccel()
        {
            if (accelQueue.Count > 0)
            {
                if (unitsCheckbox.Checked)
                {
                    aXDisplay.Text = mostRecentAccel.X.ToString();
                    aYDisplay.Text = mostRecentAccel.Y.ToString();
                    aZDisplay.Text = mostRecentAccel.Z.ToString();
                } 
                else
                {
                    // De-convert bias and offset
                    float correctedX = (mostRecentAccel.X / scale.X) - bias.X;
                    float correctedY = (mostRecentAccel.Y / scale.Y) - bias.Y;
                    float correctedZ = (mostRecentAccel.Z / scale.Z) - bias.Z;
                    aXDisplay.Text = correctedX.ToString();
                    aYDisplay.Text = correctedY.ToString();
                    aZDisplay.Text = correctedZ.ToString();
                }
            }
        }

        private void AvgAndDisplayLastNPoints()
        {
            if (accelQueue.Count >= accelerationAveragingCount)
            {
                // Have enough to avg

                // Skip to get just the last accelerationAveragingCount
                int skip = accelQueue.Count - accelerationAveragingCount;
                float avgX = 0.0f, avgY = 0.0f, avgZ = 0.0f;

                // THIS IS NOT PERFORMANT. ANY LAG SHOULD BE INVESTIGATED HERE
                int i = 0;
                foreach (Vec3 accelVector in accelQueue)
                {
                    if (i  >= skip)
                    {
                        avgX += accelVector.X;
                        avgY += accelVector.Y;
                        avgZ += accelVector.Z;
                    }
                    i++;
                }

                // Process the averages
                avgX = avgX / accelerationAveragingCount;
                avgY = avgY / accelerationAveragingCount;
                avgZ = avgZ / accelerationAveragingCount;
                if (!unitsCheckbox.Checked)
                {
                    // De-convert bias and offset
                    avgX = (mostRecentAccel.X / scale.X) - bias.X;
                    avgY = (mostRecentAccel.Y / scale.Y) - bias.Y;
                    avgZ = (mostRecentAccel.Z / scale.Z) - bias.Z;
                }
                // Reduce to 3 decimal points
                avgX = (float)Math.Round((double)avgX, 3);
                avgY = (float)Math.Round((double)avgY, 3);
                avgZ = (float)Math.Round((double)avgZ, 3);

                // Display average acceleration
                avgXDisplay.Text = avgX.ToString();
                avgYDisplay.Text = avgY.ToString();
                avgZDisplay.Text = avgZ.ToString();
            }
        }

        // ---------------------------------------------------------
        // ---------- Gesture Detection Related Functions ----------
        // ---------------------------------------------------------

        private void StateMachine()
        {
            PunchState lastState = currentState;
            currentState = GetNextState();
            stateQueue.Enqueue(currentState);

            // Display state history
            stateHistory.AppendText(lastState + " -> " + mostRecentAccel.ToString() + " -> " + currentState + "\r\n");
        }

        private PunchState GetNextState()
        {
            // Remove gravity from mostRecentAccel
            Vec3 Accel = new Vec3(mostRecentAccel.X, mostRecentAccel.Y, mostRecentAccel.Z - 9.81f);
            float absX = Math.Abs(Accel.X); float absY = Math.Abs(Accel.Y); float absZ = Math.Abs(Accel.Z);

            if (absX > thresholdAccel && absX >= absY && absX >= absZ)
            {
                // X wins out
                if (absX > thresholdAccel2)
                {
                    // Double strength
                    if (Accel.X > 0)
                    {
                        return PunchState.X_POS_2_acc;
                    }
                    return PunchState.X_NEG_2_acc;
                }
                else
                {
                    // Single strength
                    if (Accel.X > 0)
                    {
                        return PunchState.X_POS_acc;
                    }
                    return PunchState.X_NEG_acc;
                }
            }
            else if (absY > thresholdAccel && absY >= absX && absY >= absZ)
            {
                // Y wins out
                if (absY > thresholdAccel2)
                {
                    // Double strength
                    if (Accel.Y > 0)
                    {
                        return PunchState.Y_POS_2_acc;
                    }
                    return PunchState.Y_NEG_2_acc;
                }
                else
                {
                    // Single strength
                    if (Accel.Y > 0)
                    {
                        return PunchState.Y_POS_acc;
                    }
                    return PunchState.Y_NEG_acc;
                }
            }
            else if (absZ > thresholdAccel && absZ >= absX && absZ >= absY)
            {
                // Z wins out
                if (absZ > thresholdAccel2)
                {
                    // Double strength
                    if (Accel.Z > 0)
                    {
                        return PunchState.Z_POS_2_acc;
                    }
                    return PunchState.Z_NEG_2_acc;
                }
                else
                {
                    // Single strength
                    if (Accel.Z > 0)
                    {
                        return PunchState.Z_POS_acc;
                    }
                    return PunchState.Z_NEG_acc;
                }
            }

            // Nothing is bigger than the threshold value so still idle
            return PunchState.IDLE;
        }
        private void ScanForGestures()
        {
            string preStateString = QueueToString();
            string stateString = PunchStringFormat(preStateString);

            // if ending with an idle
            if (stateString.Length > 0 && stateString.EndsWith("P"))
            {
                string pureString = stateString.Replace("P", "");
                if (pureString.Equals(GESTURE_ONE))
                {
                    // Gesture One
                    GestureDetected(GESTURE_ONE_NAME);
                    SoundPlayer player = new SoundPlayer("C:\\Users\\fsant\\Desktop\\MECH-423\\Lab 1\\Exercises\\Exercise8\\SimplePunch.wav");
                    player.Play();
                }
                else if (pureString.Equals(GESTURE_TWO))
                {
                    // Gesture Two
                    GestureDetected(GESTURE_TWO_NAME);
                    SoundPlayer player = new SoundPlayer("C:\\Users\\fsant\\Desktop\\MECH-423\\Lab 1\\Exercises\\Exercise8\\HighPunch.wav");
                    player.Play();
                }
                else if (pureString.Equals(GESTURE_THREE))
                {
                    // Gesture Three
                    GestureDetected(GESTURE_THREE_NAME);
                    SoundPlayer player = new SoundPlayer("C:\\Users\\fsant\\Desktop\\MECH-423\\Lab 1\\Exercises\\Exercise8\\RightHook.wav");
                    player.Play();
                }
                else if (pureString.Equals(""))
                {
                    // Idled
                    stateQueue.Clear();
                    return;
                }
                else
                {
                    GestureDetected("Invalid Gesture detected");
                }
                preStringVisual.Text = preStateString;
                postStringVisual.Text = stateString;
                // Clear gesture tracking
                stateQueue.Clear();
            }
        }

        private void GestureDetected(string gesture)
        {
            gestureDisplayLabel.Text = gesture;
            // TOOD: fancier detection
        }

        // ----------------------------------------------
        // ---------- Gesture String Functions ----------
        // ----------------------------------------------

        private string QueueToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (PunchState item in stateQueue)
            {
                switch (item)
                {
                    case PunchState.IDLE:
                        sb.Append("I");
                        break;
                    case PunchState.X_POS_acc:
                        sb.Append("X");
                        break;
                    case PunchState.X_NEG_acc:
                        sb.Append("A");
                        break;
                    case PunchState.Y_POS_acc:
                        sb.Append("Y");
                        break;
                    case PunchState.Y_NEG_acc:
                        sb.Append("B");
                        break;
                    case PunchState.Z_POS_acc:
                        sb.Append("Z");
                        break;
                    case PunchState.Z_NEG_acc:
                        sb.Append("C");
                        break;
                    case PunchState.X_POS_2_acc:
                        sb.Append("XXX");
                        break;
                    case PunchState.X_NEG_2_acc:
                        sb.Append("AAA");
                        break;
                    case PunchState.Y_POS_2_acc:
                        sb.Append("YYY");
                        break;
                    case PunchState.Y_NEG_2_acc:
                        sb.Append("BBB");
                        break;
                    case PunchState.Z_POS_2_acc:
                        sb.Append("ZZZ");
                        break;
                    case PunchState.Z_NEG_2_acc:
                        sb.Append("CCC");
                        break;
                    default:
                        break;
                }

                sb.Append(',');
            }

            return sb.ToString();
        }

        private string PunchStringFormat(string input)
        {
            // Strip out seperator commas
            input = input.Replace(",", "");

            // Ignore idles < idlesToPause, insert a pause for idles > idlesToPause
            StringBuilder sb = new StringBuilder();
            int idleCount = 0;
            foreach (char currentChar in input)
            {
                if (currentChar == 'I')
                {
                    idleCount++;
                    if (idleCount >= idlesToPause)
                    {
                        sb.Append("P");
                    }
                }
                else
                {
                    idleCount = 0;
                    sb.Append(currentChar);
                }
            }
            input = sb.ToString();

            // Ignore chars in the ignore list
            foreach (char c in IGNORED_STATES)
            {
                input = input.Replace("" + c, "");
            }

            // Ignore "noise" by removing random chars that don't string > ignoreLessThan
            sb = new StringBuilder();
            int repeatCount = 0;
            char previousChar = '\0';
            foreach (char currentChar in input)
            {
                if (currentChar == previousChar) { repeatCount++; } else { repeatCount = 0; }

                if (currentChar != 'P')
                {
                    if (repeatCount >= ignoreLessThan)
                    {
                        sb.Append(currentChar);
                    }
                }
                else
                {
                    sb.Append('P');
                }

                previousChar = currentChar;
            }
            input = sb.ToString();

            // - Collapse duplicates of characters
            sb = new StringBuilder();
            previousChar = '\0';
            foreach (char currentChar in input)
            {
                if (currentChar != previousChar)
                {
                    sb.Append(currentChar);
                }
                previousChar = currentChar;
            }
            input = sb.ToString();


            // - Delete paired deccelerations
            sb = new StringBuilder();
            previousChar = '\0';
            foreach (char currentChar in input)
            {
                if (InvertFromChar(currentChar) == previousChar)
                { }
                else
                { sb.Append(currentChar); }
                previousChar = currentChar;
            }
            input = sb.ToString();

            return input;
        }

        private char InvertFromChar(char input)
        {
            char[] inputs = { 'X', 'Y', 'Z', 'A', 'B', 'C' };
            char[] outputs = { 'A', 'B', 'C', 'X', 'Y', 'Z' };

            for (int i = 0; i < inputs.Length; i++)
            {
                if (input == inputs[i])
                {
                    return outputs[i];
                }
            }

            return '_';
        }
    }
}
