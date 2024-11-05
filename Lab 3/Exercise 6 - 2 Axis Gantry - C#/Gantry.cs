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

namespace Exercise6
{
    public enum PACKET_FRAGMENT
    {
        START_BYTE,
        COM_BYTE,
        D1_BYTE,
        D2_BYTE,
        ESCP_BYTE
    }
    public enum COMM_BYTE
    {
        DEBUG_ECHO_REQUEST = 0, DEBUG_ECHO_RESPONSE = 1, DEBUG_UNHANDLED_COMM = 7,

        DCM_CW = 8, DCM_CCW = 9, DCM_BRAKE = 10,

        STP_SINGLE_CW = 16, STP_SINGLE_CCW = 17, STP_CONT_CW = 18, STP_CONT_CCW = 19, STP_STOP = 20,

        ENC_ROT_DELTA_CW = 24, ENC_ROT_DELTA_CCW = 25,

        GAN_RESUME = 32, GAN_PAUSE = 33, GAN_DELTA_POS_DC = 34, GAN_DELTA_NEG_DC = 35,
        GAN_DELTA_POS_STP = 36, GAN_DELTA_NEG_STP = 37, GAN_SET_MAX_PWM_DC = 38, GAN_SET_DELAY_STP = 39,
        GAN_ZERO_SETPOINT = 40, GAN_REACH_SETPOINT = 41
    }

    public partial class Gantry : Form
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

        // Trajectory
        Queue<GantryCoordinate> trajectory = new Queue<GantryCoordinate>();

        // ---------------------------------
        // ----- Constructors and Load -----
        // ---------------------------------
        public Gantry()
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
        // ----- Gantry buttons -----
        private void PauseGantryButton_Click(object sender, EventArgs e)
        {
            QueueOutgoing(new MessagePacket((byte)COMM_BYTE.GAN_PAUSE));
        }
        private void AddCoordinateButton_Click(object sender, EventArgs e)
        {
            // Wrangle inputs
            if (float.TryParse(XCoordinateInputBox.Text, out float X) && float.TryParse(YCoordinateInputBox.Text, out float Y) && uint.TryParse(SpeedInputBox.Text, out uint Speed))
            {
                // Clamp the values
                X = GantryCoordinate.Clamp(X, -GantryCoordinate.X_WIDTH_CM, GantryCoordinate.X_WIDTH_CM);
                Y = GantryCoordinate.Clamp(Y, -GantryCoordinate.Y_WIDTH_CM, GantryCoordinate.Y_WIDTH_CM);
                Speed = GantryCoordinate.Clamp(Speed, 10, 100);
                XCoordinateInputBox.Text = X.ToString();
                YCoordinateInputBox.Text = Y.ToString();
                SpeedInputBox.Text = Speed.ToString();

                // Add to Coordinates queue
                trajectory.Enqueue(new GantryCoordinate(X, Y, Speed));
                RefreshVisuals();
            }
            else
            {
                MessageBox.Show("Please enter valid values. X, Y = float. Speed = unsigned integer", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClearTrajectoryButton_Click(object sender, EventArgs e)
        {
            // Pause gantry
            QueueOutgoing(new MessagePacket((byte)COMM_BYTE.GAN_PAUSE));
            // Clear Coordinates queue
            trajectory.Clear();
            RefreshVisuals();
        }
        private void LoadTrajectoryButton_Click(object sender, EventArgs e)
        {
            // Pause gantry
            QueueOutgoing(new MessagePacket((byte)COMM_BYTE.GAN_PAUSE));
            // Clear Coordinates queue
            trajectory.Clear();
            // Open file dialogue
            // Wrangle file
            // Fill Coordinates queue

            RefreshVisuals();
        }
        private void ResumeGantryButton_Click(object sender, EventArgs e)
        {
            QueueOutgoing(new MessagePacket((byte)COMM_BYTE.GAN_RESUME));
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

            string traj = "";
            foreach (GantryCoordinate c in trajectory)
            {
                traj += c.ToString();
                traj += "\r\n";
            }
            TrajectoryVisualizationBox.Text = traj;
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
        // ----- Gantry Functions -----
        // ------------------------------

        // -------------------------------------------------
        // ----- Serial Functions & Messaging Protocol -----
        // -------------------------------------------------
        private void QueueOutgoing(MessagePacket mp)
        {
            outgoingQueue.Enqueue(255);
            outgoingQueue.Enqueue(mp.comm);
            outgoingQueue.Enqueue(mp.d1);
            outgoingQueue.Enqueue(mp.d2);
            outgoingQueue.Enqueue(mp.esc);

            string packet = $"{mp.ToString()} -> {mp.ToStringRaw()}\r\n";
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
                rxHistoryDisplay.Text += $"{mostRecentPacket.ToStringRaw()} -> ";
                mostRecentPacket.ApplyEscapeByte();mostRecentPacket.SeparateDataBytes();
                rxHistoryDisplay.Text += $"{mostRecentPacket.ToString()}\r\n";
                UseCompletePacket(mostRecentPacket);
            }

            // Increment expected next read
            if (expectedNextRx == PACKET_FRAGMENT.ESCP_BYTE)
            {
                expectedNextRx = PACKET_FRAGMENT.START_BYTE;
            } else { expectedNextRx++; }
        }
        void UseCompletePacket(MessagePacket MP)
        {
            // Use the command byte to use the rx
            COMM_BYTE comm = (COMM_BYTE)MP.comm;

            // TODO: Use received command byte
        }
    }
}
