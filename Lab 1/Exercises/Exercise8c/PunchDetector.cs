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
    public partial class PunchDetector : Form
    {

        // Gesture detection parameters
        string GESTURE_ONE = "X"; string GESTURE_ONE_NAME = "Simple Punch";
        string GESTURE_TWO = "ZX"; string GESTURE_TWO_NAME = "High Punch";
        string GESTURE_THREE = "XYZ"; string GESTURE_THREE_NAME = "Right Hook";

        // Concurrent Queue
        ConcurrentQueue<Int32> dataQueue = new ConcurrentQueue<Int32>();

        // Gesture detection
        GestureDetector detector = new GestureDetector();

        public PunchDetector()
        {
            InitializeComponent();
            refreshComboBox();

            detector.IgnoreChannels(new char[] { 'A', 'B', 'C' });
        }

        private void SerialDemo_Load(object sender, EventArgs e)
        {
            avgLabel.Text = "Average acceleration over last " + detector.GetAveragingCount() + " data points:";
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
            preStringVisual.Text = "Current compound gesture: " + detector.GetPartialGesture();
            DisplayInstantAccel();
            DisplayOrientation();
            AvgAndDisplayLastNPoints();
            CheckForGestures();
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
                detector.FeedSerialValue(nextVal);

                succ = dataQueue.TryDequeue(out nextVal);
            }
        }

        private void DisplayOrientation()
        {
            Vec3 latestAccel = detector.GetLatestAcceleration_SI();
            orientationLabel.Text = "The board's top face is the " + latestAccel.UpAxisSign() + " " + latestAccel.UpAxis() + " axis";
        }

        private void DisplayInstantAccel()
        {
            Vec3 latestAccel;
            if (unitsCheckbox.Checked)
            {
                latestAccel = detector.GetLatestAcceleration_SI(); 
            } 
            else
            {
                latestAccel = detector.GetLatestAcceleration_RAW();
            }
            aXDisplay.Text = latestAccel.X.ToString();
            aYDisplay.Text = latestAccel.Y.ToString();
            aZDisplay.Text = latestAccel.Z.ToString();
        }

        private void AvgAndDisplayLastNPoints()
        {
            Vec3 avg;
            if (unitsCheckbox.Checked)
            {
                avg = detector.GetAverageAcceleration_SI();
            }
            else
            {
                avg = detector.GetAverageAcceleration_RAW();
            }
            
            // Reduce to 3 decimal points
            float avgX = (float)Math.Round((double)avg.X, 3);
            float avgY = (float)Math.Round((double)avg.Y, 3);
            float avgZ = (float)Math.Round((double)avg.Z, 3);

            // Display average acceleration
            avgXDisplay.Text = avgX.ToString();
            avgYDisplay.Text = avgY.ToString();
            avgZDisplay.Text = avgZ.ToString();
        }

        private void CheckForGestures()
        {
            string compoundGesture = detector.PopMostRecentGesture();
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
        }
    }
}
