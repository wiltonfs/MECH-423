namespace Exercise6
{
    partial class Gantry
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.serialComboBox = new System.Windows.Forms.ComboBox();
            this.refreshButton = new System.Windows.Forms.Button();
            this.disconnectButton = new System.Windows.Forms.Button();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.boardConnectedLabel = new System.Windows.Forms.Label();
            this.visualTimer = new System.Windows.Forms.Timer(this.components);
            this.outQueueDisplay = new System.Windows.Forms.Label();
            this.postStringVisual = new System.Windows.Forms.Label();
            this.preStringVisual = new System.Windows.Forms.Label();
            this.txTimer = new System.Windows.Forms.Timer(this.components);
            this.label9 = new System.Windows.Forms.Label();
            this.txHistoryDisplay = new System.Windows.Forms.TextBox();
            this.rxHistoryDisplay = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.rxTimer = new System.Windows.Forms.Timer(this.components);
            this.PauseGantryButton = new System.Windows.Forms.Button();
            this.totalRXdDisplay = new System.Windows.Forms.Label();
            this.ResumeGantryButton = new System.Windows.Forms.Button();
            this.ClearTrajectoryButton = new System.Windows.Forms.Button();
            this.LoadTrajectoryButton = new System.Windows.Forms.Button();
            this.XCoordinateInputBox = new System.Windows.Forms.TextBox();
            this.YCoordinateInputBox = new System.Windows.Forms.TextBox();
            this.SpeedInputBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.AddCoordinateButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.TrajectoryVisualizationBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // serialComboBox
            // 
            this.serialComboBox.FormattingEnabled = true;
            this.serialComboBox.Location = new System.Drawing.Point(13, 13);
            this.serialComboBox.Name = "serialComboBox";
            this.serialComboBox.Size = new System.Drawing.Size(121, 21);
            this.serialComboBox.TabIndex = 0;
            this.serialComboBox.SelectedIndexChanged += new System.EventHandler(this.serialComboBox_SelectedIndexChanged);
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(141, 10);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(129, 23);
            this.refreshButton.TabIndex = 1;
            this.refreshButton.Text = "Refresh Serial Ports";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // disconnectButton
            // 
            this.disconnectButton.Location = new System.Drawing.Point(276, 10);
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new System.Drawing.Size(119, 23);
            this.disconnectButton.TabIndex = 2;
            this.disconnectButton.Text = "Disconnect Serial";
            this.disconnectButton.UseVisualStyleBackColor = true;
            this.disconnectButton.Click += new System.EventHandler(this.disconnectButton_Click);
            // 
            // serialPort1
            // 
            this.serialPort1.PortName = "COM3";
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // boardConnectedLabel
            // 
            this.boardConnectedLabel.AutoSize = true;
            this.boardConnectedLabel.Location = new System.Drawing.Point(14, 37);
            this.boardConnectedLabel.Name = "boardConnectedLabel";
            this.boardConnectedLabel.Size = new System.Drawing.Size(96, 13);
            this.boardConnectedLabel.TabIndex = 18;
            this.boardConnectedLabel.Text = "No board detected";
            // 
            // visualTimer
            // 
            this.visualTimer.Enabled = true;
            this.visualTimer.Tick += new System.EventHandler(this.visualTimer_Tick);
            // 
            // outQueueDisplay
            // 
            this.outQueueDisplay.AutoSize = true;
            this.outQueueDisplay.Location = new System.Drawing.Point(13, 256);
            this.outQueueDisplay.Name = "outQueueDisplay";
            this.outQueueDisplay.Size = new System.Drawing.Size(109, 13);
            this.outQueueDisplay.TabIndex = 23;
            this.outQueueDisplay.Text = "Bytes pending TX: 12";
            this.outQueueDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // postStringVisual
            // 
            this.postStringVisual.AutoSize = true;
            this.postStringVisual.Location = new System.Drawing.Point(14, 380);
            this.postStringVisual.Name = "postStringVisual";
            this.postStringVisual.Size = new System.Drawing.Size(0, 13);
            this.postStringVisual.TabIndex = 33;
            // 
            // preStringVisual
            // 
            this.preStringVisual.AutoSize = true;
            this.preStringVisual.Location = new System.Drawing.Point(14, 367);
            this.preStringVisual.Name = "preStringVisual";
            this.preStringVisual.Size = new System.Drawing.Size(0, 13);
            this.preStringVisual.TabIndex = 34;
            // 
            // txTimer
            // 
            this.txTimer.Enabled = true;
            this.txTimer.Interval = 15;
            this.txTimer.Tick += new System.EventHandler(this.txTimer_Tick);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(14, 277);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 13);
            this.label9.TabIndex = 47;
            this.label9.Text = "Tx History:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txHistoryDisplay
            // 
            this.txHistoryDisplay.Location = new System.Drawing.Point(16, 294);
            this.txHistoryDisplay.Multiline = true;
            this.txHistoryDisplay.Name = "txHistoryDisplay";
            this.txHistoryDisplay.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txHistoryDisplay.Size = new System.Drawing.Size(379, 144);
            this.txHistoryDisplay.TabIndex = 48;
            // 
            // rxHistoryDisplay
            // 
            this.rxHistoryDisplay.Location = new System.Drawing.Point(409, 294);
            this.rxHistoryDisplay.Multiline = true;
            this.rxHistoryDisplay.Name = "rxHistoryDisplay";
            this.rxHistoryDisplay.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.rxHistoryDisplay.Size = new System.Drawing.Size(379, 144);
            this.rxHistoryDisplay.TabIndex = 49;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(406, 278);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 50;
            this.label5.Text = "Rx History:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // rxTimer
            // 
            this.rxTimer.Enabled = true;
            this.rxTimer.Interval = 5;
            this.rxTimer.Tick += new System.EventHandler(this.rxTimer_Tick);
            // 
            // PauseGantryButton
            // 
            this.PauseGantryButton.Location = new System.Drawing.Point(531, 260);
            this.PauseGantryButton.Name = "PauseGantryButton";
            this.PauseGantryButton.Size = new System.Drawing.Size(129, 23);
            this.PauseGantryButton.TabIndex = 54;
            this.PauseGantryButton.Text = "Pause Gantry";
            this.PauseGantryButton.UseVisualStyleBackColor = true;
            this.PauseGantryButton.Click += new System.EventHandler(this.PauseGantryButton_Click);
            // 
            // totalRXdDisplay
            // 
            this.totalRXdDisplay.AutoSize = true;
            this.totalRXdDisplay.Location = new System.Drawing.Point(14, 243);
            this.totalRXdDisplay.Name = "totalRXdDisplay";
            this.totalRXdDisplay.Size = new System.Drawing.Size(107, 13);
            this.totalRXdDisplay.TabIndex = 57;
            this.totalRXdDisplay.Text = "Total bytes RXd: 100";
            this.totalRXdDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ResumeGantryButton
            // 
            this.ResumeGantryButton.Location = new System.Drawing.Point(659, 260);
            this.ResumeGantryButton.Name = "ResumeGantryButton";
            this.ResumeGantryButton.Size = new System.Drawing.Size(129, 23);
            this.ResumeGantryButton.TabIndex = 58;
            this.ResumeGantryButton.Text = "Resume Gantry";
            this.ResumeGantryButton.UseVisualStyleBackColor = true;
            this.ResumeGantryButton.Click += new System.EventHandler(this.ResumeGantryButton_Click);
            // 
            // ClearTrajectoryButton
            // 
            this.ClearTrajectoryButton.Location = new System.Drawing.Point(531, 231);
            this.ClearTrajectoryButton.Name = "ClearTrajectoryButton";
            this.ClearTrajectoryButton.Size = new System.Drawing.Size(129, 23);
            this.ClearTrajectoryButton.TabIndex = 59;
            this.ClearTrajectoryButton.Text = "Clear";
            this.ClearTrajectoryButton.UseVisualStyleBackColor = true;
            this.ClearTrajectoryButton.Click += new System.EventHandler(this.ClearTrajectoryButton_Click);
            // 
            // LoadTrajectoryButton
            // 
            this.LoadTrajectoryButton.Location = new System.Drawing.Point(659, 231);
            this.LoadTrajectoryButton.Name = "LoadTrajectoryButton";
            this.LoadTrajectoryButton.Size = new System.Drawing.Size(129, 23);
            this.LoadTrajectoryButton.TabIndex = 60;
            this.LoadTrajectoryButton.Text = "Load Trajectory";
            this.LoadTrajectoryButton.UseVisualStyleBackColor = true;
            this.LoadTrajectoryButton.Click += new System.EventHandler(this.LoadTrajectoryButton_Click);
            // 
            // XCoordinateInputBox
            // 
            this.XCoordinateInputBox.Location = new System.Drawing.Point(17, 97);
            this.XCoordinateInputBox.Name = "XCoordinateInputBox";
            this.XCoordinateInputBox.Size = new System.Drawing.Size(100, 20);
            this.XCoordinateInputBox.TabIndex = 61;
            this.XCoordinateInputBox.Text = "0.0";
            // 
            // YCoordinateInputBox
            // 
            this.YCoordinateInputBox.Location = new System.Drawing.Point(123, 97);
            this.YCoordinateInputBox.Name = "YCoordinateInputBox";
            this.YCoordinateInputBox.Size = new System.Drawing.Size(100, 20);
            this.YCoordinateInputBox.TabIndex = 62;
            this.YCoordinateInputBox.Text = "0.0";
            // 
            // SpeedInputBox
            // 
            this.SpeedInputBox.Location = new System.Drawing.Point(229, 97);
            this.SpeedInputBox.Name = "SpeedInputBox";
            this.SpeedInputBox.Size = new System.Drawing.Size(100, 20);
            this.SpeedInputBox.TabIndex = 63;
            this.SpeedInputBox.Text = "100";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 81);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 64;
            this.label1.Text = "X (cm)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(145, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 65;
            this.label2.Text = "Y (cm)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(252, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 66;
            this.label3.Text = "Speed (%)";
            // 
            // AddCoordinateButton
            // 
            this.AddCoordinateButton.Location = new System.Drawing.Point(107, 123);
            this.AddCoordinateButton.Name = "AddCoordinateButton";
            this.AddCoordinateButton.Size = new System.Drawing.Size(129, 23);
            this.AddCoordinateButton.TabIndex = 67;
            this.AddCoordinateButton.Text = "Add Coordinate";
            this.AddCoordinateButton.UseVisualStyleBackColor = true;
            this.AddCoordinateButton.Click += new System.EventHandler(this.AddCoordinateButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(612, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 25);
            this.label4.TabIndex = 68;
            this.label4.Text = "Trajectory";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // TrajectoryVisualizationBox
            // 
            this.TrajectoryVisualizationBox.Location = new System.Drawing.Point(531, 41);
            this.TrajectoryVisualizationBox.Multiline = true;
            this.TrajectoryVisualizationBox.Name = "TrajectoryVisualizationBox";
            this.TrajectoryVisualizationBox.Size = new System.Drawing.Size(257, 184);
            this.TrajectoryVisualizationBox.TabIndex = 69;
            // 
            // Gantry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.TrajectoryVisualizationBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.AddCoordinateButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SpeedInputBox);
            this.Controls.Add(this.YCoordinateInputBox);
            this.Controls.Add(this.XCoordinateInputBox);
            this.Controls.Add(this.LoadTrajectoryButton);
            this.Controls.Add(this.ClearTrajectoryButton);
            this.Controls.Add(this.ResumeGantryButton);
            this.Controls.Add(this.totalRXdDisplay);
            this.Controls.Add(this.PauseGantryButton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.rxHistoryDisplay);
            this.Controls.Add(this.txHistoryDisplay);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.preStringVisual);
            this.Controls.Add(this.postStringVisual);
            this.Controls.Add(this.outQueueDisplay);
            this.Controls.Add(this.boardConnectedLabel);
            this.Controls.Add(this.disconnectButton);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.serialComboBox);
            this.Name = "Gantry";
            this.Text = "Gantry Commander";
            this.Load += new System.EventHandler(this.DcMotorController_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox serialComboBox;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.Button disconnectButton;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Label boardConnectedLabel;
        private System.Windows.Forms.Timer visualTimer;
        private System.Windows.Forms.Label outQueueDisplay;
        private System.Windows.Forms.Label postStringVisual;
        private System.Windows.Forms.Label preStringVisual;
        private System.Windows.Forms.Timer txTimer;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txHistoryDisplay;
        private System.Windows.Forms.TextBox rxHistoryDisplay;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Timer rxTimer;
        private System.Windows.Forms.Button PauseGantryButton;
        private System.Windows.Forms.Label totalRXdDisplay;
        private System.Windows.Forms.Button ResumeGantryButton;
        private System.Windows.Forms.Button ClearTrajectoryButton;
        private System.Windows.Forms.Button LoadTrajectoryButton;
        private System.Windows.Forms.TextBox XCoordinateInputBox;
        private System.Windows.Forms.TextBox YCoordinateInputBox;
        private System.Windows.Forms.TextBox SpeedInputBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button AddCoordinateButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TrajectoryVisualizationBox;
    }
}

