using System;
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
    public partial class SerialDemo : Form
    {
        string serialDataString = "";
        ConcurrentQueue<Int32> dataQueue = new ConcurrentQueue<Int32>();

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
                serialDataString = serialDataString + newByte.ToString() + ", ";
                dataQueue.Enqueue(newByte);

                bytesToRead = serialPort1.BytesToRead;
            }

        }

        private void DisplaySerialData()
        {
            if (serialPort1.IsOpen)
                bytesInput.Text = serialPort1.BytesToRead.ToString();
            else
                bytesInput.Text = "port not open";

            // Display sizes of data containers
            lengthInput.Text = serialDataString.Length.ToString();
            queueSizeDisplay.Text = dataQueue.Count.ToString();

            // Display contents of string container (DEPRECATED)
            //serialDisplay.AppendText(serialDataString);
            serialDataString = "";

            // Display contents of queue container
            int nextVal;
            bool succ = dataQueue.TryDequeue(out nextVal);
            while (succ)
            {
                serialDisplay.AppendText(nextVal.ToString() + ", ");
                succ = dataQueue.TryDequeue(out nextVal);
            }
        }

        private void DisplayInstantAccel()
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DisplaySerialData();
        }
    }
}
