﻿using System;
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
    struct MessagePacket {
        public byte comm;
        public byte d1;
        public byte d2;
        public byte esc;
        public uint combined;
    }
    enum COMM_BYTE
    {
        DCM_CW = 8, DCM_CCW = 9, DCM_BRAKE = 10,

        STP_SINGLE_CW = 16, STP_SINGLE_CCW = 17, STP_CONT_CW = 18, STP_CONT_CCW = 19, STP_STOP = 20,

        ENC_ROT_DELTA = 24,
    }

    public partial class EncoderReader : Form
    {
        // --------------------------
        // ----- Members & Data -----
        // --------------------------

        // Speed must exceed this to send a non-brake command to the motor
        const int THRESHOLD_SPEED = 3;

        // Last transmitted speed values for the DC motor
        int DC_LastSpeedValue = 0;

        // Input and output Serial queues
        ConcurrentQueue<byte> outgoingQueue = new ConcurrentQueue<byte>();
        ConcurrentQueue<int> incomingQueue = new ConcurrentQueue<int>();
        PACKET_FRAGMENT expectedNextRx = PACKET_FRAGMENT.START_BYTE;
        MessagePacket mostRecentPacket = new MessagePacket();

        // Encoder data
        const float COUNTS_PER_CYCLE = 236;
        int EncoderPosition = 0;    // CW Counts
        float EncoderVelocity = 0;  // CW Counts per second
        Stopwatch VelocityStopwatch = Stopwatch.StartNew();

        // ---------------------------------
        // ----- Constructors and Load -----
        // ---------------------------------
        public EncoderReader()
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
        // ----- Timers -----
        private void visualTimer_Tick(object sender, EventArgs e)
        {
            RefreshVisuals();
        }
        private void inputsTimer_Tick(object sender, EventArgs e)
        {
            if (DC1_SpeedInput.Value != DC_LastSpeedValue)
            {
                DC_LastSpeedValue = DC1_SpeedInput.Value;
                SendDCMotorCommand(DC1_SpeedInput.Value);
            }
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

                bytesToRead = serialPort1.BytesToRead;
            }
        }

        // ------------------------
        // ----- UI Functions -----
        // ------------------------

        private void RefreshVisuals()
        {
            // Display the queue of outgoing values
            string queue = "Queue pending TX: ";
            foreach (byte b in outgoingQueue)
            {
                queue += b + " ";
            }
            outQueueDisplay.Text = queue;

            if (!serialPort1.IsOpen)
            { boardConnectedLabel.Text = "No board detected"; }
            else
            { boardConnectedLabel.Text = "Board connected"; }

            positionLabel.Text = $"Encoder position: \t\t{EncoderPosition}";
            velocityLabelHz.Text = $"Encoder velocity (Hz): \t\t{EncoderVelocity / COUNTS_PER_CYCLE}";
            velocityLabelRPM.Text = $"Encoder velocity (RPM): \t\t{EncoderVelocity / COUNTS_PER_CYCLE * 60f}";

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

        // ------------------------------
        // ----- DC Motor Functions -----
        // ------------------------------

        private void SendDCMotorCommand(int speed)
        {
            COMM_BYTE COM = COMM_BYTE.DCM_BRAKE;
            if (speed > THRESHOLD_SPEED)
                COM = COMM_BYTE.DCM_CW;
            if (speed < -THRESHOLD_SPEED)
                COM = COMM_BYTE.DCM_CCW;

            byte D1 = MostSignificant(SpeedToPWM(speed));
            byte D2 = LeastSignificant(SpeedToPWM(speed));

            QueueOutgoing(COM, D1, D2);
        }

        private uint SpeedToPWM(int speed)
        {
            if (Math.Abs(speed) <= THRESHOLD_SPEED)
                return 0;
            speed = Math.Abs(speed);

            uint offset = 0;
            uint bias = 1300;
            uint PWM = ((uint)speed * bias + offset);
            return PWM;
        }

        // -------------------------------------------------
        // ----- Serial Functions & Messaging Protocol -----
        // -------------------------------------------------
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

            string packet = $"[{COMMenum}, {DataBytesToInt(D1, D2)}] = [255 {COMM} {D1} {D2} {ESC}]\t\t";
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
                FormatCompletePacket(mostRecentPacket);
                rxHistoryDisplay.Text += $"[{(COMM_BYTE)mostRecentPacket.comm}, {mostRecentPacket.combined}]\t\t";
                UseCompletePacket(mostRecentPacket);
            }

            // Increment expected next read
            if (expectedNextRx == PACKET_FRAGMENT.ESCP_BYTE)
            {
                expectedNextRx = PACKET_FRAGMENT.START_BYTE;
            } else { expectedNextRx++; }
            
            
        }

        void FormatCompletePacket(MessagePacket MP)
        {
            // Handle the escape byte
            if ((MP.esc & 0x1) > 0)
                MP.comm = 255;
            if ((MP.esc & 0x2) > 0)
                MP.d1 = 255;
            if ((MP.esc & 0x4) > 0)
                MP.d2 = 255;

            // Combine data 1 and 2
            MP.combined = DataBytesToInt(MP.d1, MP.d2);
    }
        void UseCompletePacket(MessagePacket MP)
        {
            // Use the command byte to use the rx
            COMM_BYTE comm = (COMM_BYTE)MP.comm;

            if (comm == COMM_BYTE.ENC_ROT_DELTA)
            {
                VelocityStopwatch.Stop();

                int LastPosition = EncoderPosition;
                EncoderPosition += MP.d1 - MP.d2;

                EncoderVelocity = 1000f * ((float)(EncoderPosition - LastPosition)) / ((float)VelocityStopwatch.ElapsedMilliseconds);

                VelocityStopwatch = Stopwatch.StartNew();
            }
        }

        private byte CommandByteToByte(COMM_BYTE COMenum)
        {
            return (byte)COMenum;
            switch (COMenum)
            {
                case COMM_BYTE.DCM_CW:              return 8;
                case COMM_BYTE.DCM_CCW:             return 9;
                case COMM_BYTE.DCM_BRAKE:           return 10;
                case COMM_BYTE.STP_SINGLE_CW:       return 16;
                case COMM_BYTE.STP_SINGLE_CCW:      return 17;
                case COMM_BYTE.STP_CONT_CW:         return 18;
                case COMM_BYTE.STP_CONT_CCW:        return 19;
                case COMM_BYTE.STP_STOP:            return 20;
                default: 
                    // If we get here, there is a COMM_BYTE that hasn't been implemented in this switch statement
                    throw new NotImplementedException();
                    return 255;
            }
        }
        private byte MostSignificant(uint value)
        {
            return (byte)((value >> 8) & 0xFF);
        }

        private byte LeastSignificant(uint value)
        {
            return (byte)(value & 0xFF);
        }

        private uint DataBytesToInt(byte D1, byte D2)
        {
            return (uint)((D1 << 8) | (D2 & 0xFF));
        }
    }
}