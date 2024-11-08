using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DCMotorController
{
    enum PACKET_FRAGMENT
    {
        START_BYTE,
        COM_BYTE,
        D1_BYTE,
        D2_BYTE,
        ESCP_BYTE
    }
    struct MessagePacket
    {
        public byte comm;
        public byte d1;
        public byte d2;
        public byte esc;
        public ushort combined;
    }
    enum COMM_BYTE
    {
        DEBUG_ECHO_REQUEST = 0, DEBUG_ECHO_RESPONSE = 1, DEBUG_UNHANDLED_COMM = 7,

        DCM_CW = 8, DCM_CCW = 9, DCM_BRAKE = 10,

        STP_SINGLE_CW = 16, STP_SINGLE_CCW = 17, STP_CONT_CW = 18, STP_CONT_CCW = 19, STP_STOP = 20,

        ENC_ROT_DELTA_CW = 24, ENC_ROT_DELTA_CCW = 25,

        MES_RESET = 48, MES_REQ_STEP = 49, MES_REQ_POSITION = 50, MES_RESPONSE = 51
    }

    public partial class Controls : Form
    {
        // --------------------------
        // ----- Members & Data -----
        // --------------------------

        // Input and output Serial queues
        int totalBytesRXd = 0;
        ConcurrentQueue<byte> outgoingQueue = new ConcurrentQueue<byte>();
        ConcurrentQueue<int> incomingQueue = new ConcurrentQueue<int>();
        PACKET_FRAGMENT expectedNextRx = PACKET_FRAGMENT.START_BYTE;
        MessagePacket mostRecentPacket = new MessagePacket();

        // ---------------------------------
        // ----- Constructors and Load -----
        // ---------------------------------
        public Controls()
        {
            InitializeComponent();
            refreshSerialConnectionsComboBox();
        }

        private void DcMotorController_Load(object sender, EventArgs e)
        {}

        // ---------------------
        // ----- UI Events -----
        // ---------------------

        // ----- Connecting to board -----
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
        private void refreshButton_Click(object sender, EventArgs e)
        {
            refreshSerialConnectionsComboBox();
        }
        private void StopMotorButton_Click(object sender, EventArgs e)
        {
            MeasurementDisplayBox.Text = "";
            QueueOutgoing(COMM_BYTE.MES_RESET, 0, 0);
        }
        private void StepInputButton_Click(object sender, EventArgs e)
        {
            ushort speed = 0;
            if (ushort.TryParse(StepInputTextbox.Text, out speed))
            {
                ushort PWM = (ushort)(speed * 655);
                QueueOutgoing(COMM_BYTE.MES_REQ_STEP, PWM);
            } else {
                MessageBox.Show("Invalid step input, must be 0-100. Please enter a valid integer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void PositionalCommandButton_Click(object sender, EventArgs e)
        {
            ushort position = 0;
            if (ushort.TryParse(PositionalCommandTextbox.Text, out position))
            {
                QueueOutgoing(COMM_BYTE.MES_REQ_POSITION, position);
            }
            else
            {
                MessageBox.Show("Invalid location input, must be 0-65535. Please enter a valid integer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // ----- Timers -----
        private void visualTimer_Tick(object sender, EventArgs e)
        {
            RefreshVisuals();
        }
        private void txTimer_Tick(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen && serialPort1.BytesToWrite == 0 && outgoingQueue.Count() > 0)
            {
                byte tx = 0;
                bool succ = outgoingQueue.TryDequeue(out tx);
                if (succ)
                    serialPort1.Write(new byte[] { tx }, 0, 1);
            }
        }

        private void rxTimer_Tick(object sender, EventArgs e)
        {
            if (!incomingQueue.IsEmpty)
            {
                int result = 0;
                while(incomingQueue.TryDequeue(out result))
                {
                    RxStateMachine((byte)result);
                }
            }
        }

        // -------------------------
        // ----- Serial Events -----
        // -------------------------

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            int newByte = 0;
            int bytesToRead;
            bytesToRead = serialPort1.BytesToRead;
            while (bytesToRead != 0)
            {
                newByte = serialPort1.ReadByte();
                incomingQueue.Enqueue(newByte);
                totalBytesRXd++;

                bytesToRead = serialPort1.BytesToRead;
            }
        }

        // ------------------------
        // ----- UI Functions -----
        // ------------------------

        private void RefreshVisuals()
        {
            outQueueDisplay.Text = $"Bytes pending TX: {outgoingQueue.Count}";
            totalRXdDisplay.Text = $"Total bytes RXd: {totalBytesRXd}";

            if (!serialPort1.IsOpen)
            { boardConnectedLabel.Text = "No board detected"; }
            else
            { boardConnectedLabel.Text = "Board connected"; }
        }
        void refreshSerialConnectionsComboBox()
        {
            // Refresh the combo box with the available COM ports
            serialComboBox.Items.Clear();
            serialComboBox.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
            if (serialComboBox.Items.Count == 0)
                serialComboBox.Text = "No COM ports!";
            else
                serialComboBox.SelectedIndex = 0;
        }

        // -------------------------------------------------
        // ----- Serial Functions & Messaging Protocol -----
        // -------------------------------------------------

        private void QueueOutgoing(COMM_BYTE COMMenum, ushort data)
        {
            QueueOutgoing(COMMenum, MostSignificant(data), LeastSignificant(data));
        }
        private void QueueOutgoing(COMM_BYTE COMMenum, byte D1, byte D2)
        {
            ushort combined = DataBytesToUShort(D1, D2);

            byte COMM = (byte)COMMenum;

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

            string packet = $"[{COMMenum}, {combined}] = [255 {COMM} {D1} {D2} {ESC}]\r\n";
            txHistoryDisplay.Text += packet;
        }

        private void RxStateMachine(byte nextReceive)
        {
            if (nextReceive == 255)
            {
                expectedNextRx = PACKET_FRAGMENT.COM_BYTE;
                return;
            }

            if (expectedNextRx == PACKET_FRAGMENT.COM_BYTE) {
                mostRecentPacket.comm = nextReceive;
            } else if (expectedNextRx == PACKET_FRAGMENT.D1_BYTE) {
                mostRecentPacket.d1 = nextReceive;
            } else if (expectedNextRx == PACKET_FRAGMENT.D2_BYTE) {
                mostRecentPacket.d2 = nextReceive;
            } else if (expectedNextRx == PACKET_FRAGMENT.ESCP_BYTE) {
                mostRecentPacket.esc = nextReceive;
                // Finished transmission
                rxHistoryDisplay.Text += $"[255 {mostRecentPacket.comm} {mostRecentPacket.d1} {mostRecentPacket.d2} {mostRecentPacket.esc}] = ";
                FormatCompletePacket(ref mostRecentPacket);
                rxHistoryDisplay.Text += $"[{(COMM_BYTE)mostRecentPacket.comm}, {mostRecentPacket.combined}]\t\t";
                UseCompletePacket(mostRecentPacket);
            }

            // Increment expected next read
            if (expectedNextRx == PACKET_FRAGMENT.ESCP_BYTE)
            {
                expectedNextRx = PACKET_FRAGMENT.START_BYTE;
            } else { expectedNextRx++; }
            
            
        }

        void FormatCompletePacket(ref MessagePacket MP)
        {
            // Handle the escape byte
            if ((MP.esc & 0x1) > 0)
                MP.comm = 255;
            if ((MP.esc & 0x2) > 0)
                MP.d1 = 255;
            if ((MP.esc & 0x4) > 0)
                MP.d2 = 255;

            // Combine data 1 and 2
            MP.combined = DataBytesToUShort(MP.d1, MP.d2);
        }

        void UseCompletePacket(MessagePacket MP)
        {
            // Use the command byte to use the rx
            COMM_BYTE comm = (COMM_BYTE)MP.comm;

            if (comm == COMM_BYTE.MES_RESPONSE) {
                MeasurementDisplayBox.Text += $" {MP.combined},";
            }
        }

        private byte MostSignificant(ushort value)
        {
            return (byte)((value >> 8) & 0xFF);
        }

        private byte LeastSignificant(ushort value)
        {
            return (byte)(value & 0xFF);
        }

        private ushort DataBytesToUShort(byte D1, byte D2)
        {
            return (ushort)(((ushort)D1 << 8) | (ushort)D2);
        }
    }
}
