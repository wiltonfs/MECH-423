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

        ENC_ROT_DELTA_CW = 24, ENC_ROT_DELTA_CCW = 25
    }

    public partial class Controls : Form
    {
        // --------------------------
        // ----- Members & Data -----
        // --------------------------

        // Speed must exceed this to send a non-brake command to the motor
        const int THRESHOLD_SPEED = 3;

        // Last transmitted speed values for the DC motor
        int DC_LastSpeedValue = 0;
        uint DC_LastPWMValue = 0;

        // Input and output Serial queues
        int totalBytesRXd = 0;
        ConcurrentQueue<byte> outgoingQueue = new ConcurrentQueue<byte>();
        ConcurrentQueue<int> incomingQueue = new ConcurrentQueue<int>();
        PACKET_FRAGMENT expectedNextRx = PACKET_FRAGMENT.START_BYTE;
        MessagePacket mostRecentPacket = new MessagePacket();

        // Encoder data
        const float COUNTS_PER_CYCLE = 236;
        const float SECONDS_PER_TX = 0.2025f;
        int EncoderPosition = 0;    // CW Counts
        float EncoderVelocity = 0;  // CW Counts per second

        // Encoder plotting data
        int PLOT_HISTORY_SIZE = 100;
        Queue<int> EncoderPositions = new Queue<int>();
        Queue<float> EncoderVelocities = new Queue<float>();
        string DataLoggingFilePath = "";
        Stopwatch LoggingStopwatch = Stopwatch.StartNew();

        // ---------------------------------
        // ----- Constructors and Load -----
        // ---------------------------------
        public Controls()
        {
            InitializeComponent();
            refreshSerialConnectionsComboBox();
            CreateDataFile();
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
            SendDCMotorCommand(0);
        }
        private void StepInputButton_Click(object sender, EventArgs e)
        {
            int speed = 0;
            if (int.TryParse(StepInputTextbox.Text, out speed))
            {
                SendDCMotorCommand(speed);
            } else {
                MessageBox.Show("Invalid step input, must be 0-50. Please enter a valid integer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            WriteDataFileLine();
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

            positionLabel.Text = $"Encoder position: \t\t{EncoderPosition}";
            velocityLabelHz.Text = $"Encoder velocity (Hz): \t\t{EncoderVelocity / COUNTS_PER_CYCLE}";
            velocityLabelRPM.Text = $"Encoder velocity (RPM): \t\t{EncoderVelocity / COUNTS_PER_CYCLE * 60f}";

            UpdatePositionChart();
            UpdateVelocityChart();
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

            DC_LastPWMValue = SpeedToPWM(speed);
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

            string packet = $"[{COMMenum}, {DataBytesToUShort(D1, D2)}] = [255 {COMM} {D1} {D2} {ESC}]\t\t";
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

            if (comm == COMM_BYTE.ENC_ROT_DELTA_CW) {
                UpdateEncoderPositionAndVelocity((int)MP.combined);
            } else if (comm == COMM_BYTE.ENC_ROT_DELTA_CCW) {
                UpdateEncoderPositionAndVelocity(-1 * (int)MP.combined);
            }
        }

        void UpdateEncoderPositionAndVelocity(int EncoderPositionDelta)
        {
            EncoderPosition += EncoderPositionDelta;
            EncoderVelocity = (((float)EncoderPositionDelta)) / (SECONDS_PER_TX);

            EncoderPositions.Enqueue(EncoderPosition);
            EncoderVelocities.Enqueue(EncoderVelocity);

            if (EncoderVelocities.Count > PLOT_HISTORY_SIZE)
            {
                EncoderPositions.Dequeue();
                EncoderVelocities.Dequeue();
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

        private ushort DataBytesToUShort(byte D1, byte D2)
        {
            return (ushort)(((ushort)D1 << 8) | (ushort)D2);
        }

        // --------------------
        // ----- Plotting -----
        // --------------------
        private void UpdatePositionChart()
        {
            int[] poss = { 1, 1, 3, 3, 4, 4, 3, 5, 7, 4, 1, 1, 5, 10};
            //EncoderPositions = new Queue<int>(poss);

            // Initialize Chart
            positionChart.Series.Clear();
            positionChart.ChartAreas.Clear();
            positionChart.ChartAreas.Add("MainArea");

            // Position Series
            var positionSeries = positionChart.Series.Add("Position");
            positionSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            positionSeries.Color = Color.Blue;

            float time = 0f;
            foreach (int pos in EncoderPositions)
            {
                time += SECONDS_PER_TX;
                positionSeries.Points.AddXY(time, pos);
            }

            positionChart.ChartAreas["MainArea"].AxisX.Title = "Time [s]";
            positionChart.ChartAreas["MainArea"].AxisY.Title = "Position [counts]";

            positionChart.ChartAreas[0].AxisX.LabelStyle.Enabled = false;
            positionChart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
        }

        private void UpdateVelocityChart()
        {
            float[] vels = { -2, 3, 3, 19, -20, 0, 0.5f, -9, 3, 4, 5, 3, 12, 14 };
            //EncoderVelocities = new Queue<float>(vels);

            // Initialize Chart
            velocityChart.Series.Clear();
            velocityChart.ChartAreas.Clear();
            velocityChart.ChartAreas.Add("MainArea");

            // Velocity Series
            var velocitySeries = velocityChart.Series.Add("Velocity");
            velocitySeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            velocitySeries.Color = Color.Red;

            float time = 0f;
            foreach (int vel in EncoderVelocities)
            {
                time += SECONDS_PER_TX;
                velocitySeries.Points.AddXY(time, vel / COUNTS_PER_CYCLE);
            }

            velocityChart.ChartAreas["MainArea"].AxisX.Title = "Time [s]";
            velocityChart.ChartAreas["MainArea"].AxisY.Title = "Velocity [Hz]";

            velocityChart.ChartAreas[0].AxisX.LabelStyle.Enabled = false;
            velocityChart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
        }

        private void CreateDataFile()
        {
            int fileNum = 0;
            string directoryPath = "../../Data";
            Directory.CreateDirectory(directoryPath);

            string filePath = Path.Combine(directoryPath, $"output_{fileNum}.csv");
            while (File.Exists(filePath))
            {
                fileNum++;
                filePath = Path.Combine(directoryPath, $"output_{fileNum}.csv");
            }

            DataLoggingFilePath = filePath;

            using (StreamWriter writer = new StreamWriter(DataLoggingFilePath))
            {
                writer.WriteLine("C# time [ms],DC Motor PWM,Encoder Position [counts],Encoder Velocity [Hz]");
            }
        }

        private void WriteDataFileLine()
        {
            using (StreamWriter writer = new StreamWriter(DataLoggingFilePath, true))
            {
                //writer.WriteLine("C# time [ms],DC Motor PWM,Encoder Position [counts],Encoder Velocity [Hz]");
                string data = $"{LoggingStopwatch.ElapsedMilliseconds},{DC_LastPWMValue},{EncoderPosition},{EncoderVelocity / COUNTS_PER_CYCLE}";
                writer.WriteLine(data);
            }
        }
    }
}
