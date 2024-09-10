using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
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
        // Editable parameters
        int accelerationAveragingCount = 100;

        // Concurrent Queue
        ConcurrentQueue<Int32> dataQueue = new ConcurrentQueue<Int32>();

        // Vec3 Queue
        ExpectedNextRead expectedNextRead = ExpectedNextRead.LEAD;
        Queue<Vec3> accelQueue = new Queue<Vec3>();
        Vec3 mostRecentAccel = new Vec3();
        Vec3 bias = new Vec3(-125f, -126f, -125f);
        Vec3 scale = new Vec3(0.35f, 0.35f, 0.35f);

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
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            int newByte = 0;
            int bytesToRead;
            bytesToRead = serialPort1.BytesToRead;

            // Display serial buffer size here
            serialBufferSizeLabel.Text = "Serial buffer size: " + bytesToRead;
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
                // Have enough to avg at least

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
                    float correctedX = (mostRecentAccel.X / scale.X) - bias.X;
                    float correctedY = (mostRecentAccel.Y / scale.Y) - bias.Y;
                    float correctedZ = (mostRecentAccel.Z / scale.Z) - bias.Z;
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
    }
}
