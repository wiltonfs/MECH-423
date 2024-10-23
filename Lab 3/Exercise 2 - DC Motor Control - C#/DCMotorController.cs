using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DCMotorController
{
    enum COMM_BYTE
    {
        STOP_DC_1, CW_DC_1, CCW_DC_1
    }

    public partial class DCMotorController : Form
    {
        Queue<byte> outgoingQueue = new Queue<byte>();
        Queue<int> incomingQueue = new Queue<int>();

        public DCMotorController()
        {
            InitializeComponent();
            refreshComboBox();
        }

        void refreshComboBox()
        {
            // Refresh the combo box with the available COM ports
            serialComboBox.Items.Clear();
            serialComboBox.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
            if (serialComboBox.Items.Count == 0)
                serialComboBox.Text = "No COM ports!";
            else
                serialComboBox.SelectedIndex = 0;
        }

        private void DcMotorController_Load(object sender, EventArgs e)
        {}

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
                incomingQueue.Enqueue(newByte);

                bytesToRead = serialPort1.BytesToRead;
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            refreshComboBox();
        }
        private void RefreshVisuals()
        {
            string queue = "Out queue: ";
            foreach (byte b in outgoingQueue)
            {
                queue += b + " ";
            }
            outQueueDisplay.Text = queue;

        }

        private void SendDCMotorCommand(int speed)
        {
            COMM_BYTE COM = COMM_BYTE.STOP_DC_1;
            if (speed > 0)
                COM = COMM_BYTE.CW_DC_1;
            if (speed < 0)
                COM = COMM_BYTE.CCW_DC_1;

            byte D1 = MostSignificant(SpeedToPWM(speed));
            byte D2 = LeastSignificant(SpeedToPWM(speed));

            QueueOutgoing(COM, D1, D2);
        }

        private void QueueOutgoing(COMM_BYTE COMMenum, byte D1, byte D2)
        {
            byte COMM = CommandByteToByte(COMMenum);

            byte ESC = 0;
            
            if (COMM > 254)
            {
                COMM = 254; ESC += 1;
            }
            if (D1 > 254)
            {
                D1 = 254; ESC += 2;
            }
            if (D2 > 254)
            {
                D2 = 254; ESC += 4;
            }

            outgoingQueue.Enqueue(255);
            outgoingQueue.Enqueue(COMM);
            outgoingQueue.Enqueue(D1);
            outgoingQueue.Enqueue(D2);
            outgoingQueue.Enqueue(ESC);
        }

        private byte CommandByteToByte(COMM_BYTE COMenum)
        {
            switch (COMenum)
            {
                case COMM_BYTE.STOP_DC_1: return 0;
                case COMM_BYTE.CW_DC_1: return 1;
                case COMM_BYTE.CCW_DC_1: return 2;
                default: return 0;
            }
        }

        private uint SpeedToPWM(int speed)
        {
            if (speed == 0)
                return 0;
            speed = Math.Abs(speed);

            uint offset = 5;
            uint bias = 1;
            uint PWM = ((uint)speed*bias + offset);
            return PWM;
        }

        private byte MostSignificant(uint value)
        {
            return (byte)((value >> 8) & 0xFF);
        }

        private byte LeastSignificant(uint value)
        {
            return (byte)(value & 0xFF);
        }

        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        private void DC1_SendButton_Click(object sender, EventArgs e)
        {
            SendDCMotorCommand(DC1_SpeedInput.Value);
        }

        private void visualTimer_Tick(object sender, EventArgs e)
        {
            RefreshVisuals();
        }

        private void txTimer_Tick(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen && serialPort1.BytesToWrite == 0)
            {
                byte tx = outgoingQueue.Dequeue();
                serialPort1.Write(new byte[] { tx }, 0, 1);
            }
        }
    }
}
