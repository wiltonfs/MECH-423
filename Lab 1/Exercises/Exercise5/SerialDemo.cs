﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exercise4
{
    enum ExpectedNextRead
    {
        LEAD, X, Y, Z
    }

    public partial class SerialDemo : Form
    {
        ConcurrentQueue<Int32> dataQueue = new ConcurrentQueue<Int32>();
        ExpectedNextRead expectedNextRead = ExpectedNextRead.LEAD;
        
        Queue<Vec3> accelQueue = new Queue<Vec3>();
        Vec3 mostRecentAccel = new Vec3();
        Vec3 bias = new Vec3(-125f, -126f, -125f);
        Vec3 scale = new Vec3(0.35f, 0.35f, 0.35f);

        public SerialDemo()
        {
            InitializeComponent();

            refreshComboBox();
        }

        private void SerialDemo_Load(object sender, EventArgs e)
        {

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

        private void ProcessSerialData()
        {
            if (serialPort1.IsOpen)
                bytesInput.Text = serialPort1.BytesToRead.ToString();
            else
            {
                bytesInput.Text = "port not open";
            }

            // Display sizes of the queue
            queueSizeDisplay.Text = dataQueue.Count.ToString();

            // Display contents of queue
            foreach (var item in dataQueue)
            {
                serialDisplay.AppendText(item.ToString() + ", ");
            }

            // Read and process queue
            ReadQueue();

            
            
        }

        // --- Acceleration and Queue Related Functions ---

        private void ReadQueue()
        {
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
                } else
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            ProcessSerialData();
            DisplayInstantAccel();
            DisplayOrientation();
        }
    }
}
