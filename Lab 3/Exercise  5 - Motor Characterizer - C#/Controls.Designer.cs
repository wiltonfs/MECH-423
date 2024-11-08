namespace DCMotorController
{
    partial class Controls
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
            this.ClearMeasurementButton = new System.Windows.Forms.Button();
            this.StepInputButton = new System.Windows.Forms.Button();
            this.StepInputTextbox = new System.Windows.Forms.TextBox();
            this.totalRXdDisplay = new System.Windows.Forms.Label();
            this.PositionalCommandTextbox = new System.Windows.Forms.TextBox();
            this.PositionalCommandButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.MeasurementDisplayBox = new System.Windows.Forms.TextBox();
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
            this.outQueueDisplay.Location = new System.Drawing.Point(13, 346);
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
            this.label9.Location = new System.Drawing.Point(14, 367);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 13);
            this.label9.TabIndex = 47;
            this.label9.Text = "Tx History:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txHistoryDisplay
            // 
            this.txHistoryDisplay.Location = new System.Drawing.Point(16, 384);
            this.txHistoryDisplay.Multiline = true;
            this.txHistoryDisplay.Name = "txHistoryDisplay";
            this.txHistoryDisplay.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txHistoryDisplay.Size = new System.Drawing.Size(379, 54);
            this.txHistoryDisplay.TabIndex = 48;
            // 
            // rxHistoryDisplay
            // 
            this.rxHistoryDisplay.Location = new System.Drawing.Point(409, 384);
            this.rxHistoryDisplay.Multiline = true;
            this.rxHistoryDisplay.Name = "rxHistoryDisplay";
            this.rxHistoryDisplay.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.rxHistoryDisplay.Size = new System.Drawing.Size(379, 54);
            this.rxHistoryDisplay.TabIndex = 49;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(406, 368);
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
            // ClearMeasurementButton
            // 
            this.ClearMeasurementButton.Location = new System.Drawing.Point(12, 65);
            this.ClearMeasurementButton.Name = "ClearMeasurementButton";
            this.ClearMeasurementButton.Size = new System.Drawing.Size(129, 23);
            this.ClearMeasurementButton.TabIndex = 54;
            this.ClearMeasurementButton.Text = "Clear Measurement";
            this.ClearMeasurementButton.UseVisualStyleBackColor = true;
            this.ClearMeasurementButton.Click += new System.EventHandler(this.StopMotorButton_Click);
            // 
            // StepInputButton
            // 
            this.StepInputButton.Location = new System.Drawing.Point(12, 94);
            this.StepInputButton.Name = "StepInputButton";
            this.StepInputButton.Size = new System.Drawing.Size(153, 23);
            this.StepInputButton.TabIndex = 55;
            this.StepInputButton.Text = "Step Input [PWM]";
            this.StepInputButton.UseVisualStyleBackColor = true;
            this.StepInputButton.Click += new System.EventHandler(this.StepInputButton_Click);
            // 
            // StepInputTextbox
            // 
            this.StepInputTextbox.Location = new System.Drawing.Point(171, 96);
            this.StepInputTextbox.Name = "StepInputTextbox";
            this.StepInputTextbox.Size = new System.Drawing.Size(100, 20);
            this.StepInputTextbox.TabIndex = 56;
            // 
            // totalRXdDisplay
            // 
            this.totalRXdDisplay.AutoSize = true;
            this.totalRXdDisplay.Location = new System.Drawing.Point(14, 333);
            this.totalRXdDisplay.Name = "totalRXdDisplay";
            this.totalRXdDisplay.Size = new System.Drawing.Size(107, 13);
            this.totalRXdDisplay.TabIndex = 57;
            this.totalRXdDisplay.Text = "Total bytes RXd: 100";
            this.totalRXdDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // PositionalCommandTextbox
            // 
            this.PositionalCommandTextbox.Location = new System.Drawing.Point(171, 125);
            this.PositionalCommandTextbox.Name = "PositionalCommandTextbox";
            this.PositionalCommandTextbox.Size = new System.Drawing.Size(100, 20);
            this.PositionalCommandTextbox.TabIndex = 59;
            // 
            // PositionalCommandButton
            // 
            this.PositionalCommandButton.Location = new System.Drawing.Point(12, 123);
            this.PositionalCommandButton.Name = "PositionalCommandButton";
            this.PositionalCommandButton.Size = new System.Drawing.Size(153, 23);
            this.PositionalCommandButton.TabIndex = 58;
            this.PositionalCommandButton.Text = "Positional Command [counts]";
            this.PositionalCommandButton.UseVisualStyleBackColor = true;
            this.PositionalCommandButton.Click += new System.EventHandler(this.PositionalCommandButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(359, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 60;
            this.label1.Text = "Measurement:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // MeasurementDisplayBox
            // 
            this.MeasurementDisplayBox.Location = new System.Drawing.Point(362, 86);
            this.MeasurementDisplayBox.Multiline = true;
            this.MeasurementDisplayBox.Name = "MeasurementDisplayBox";
            this.MeasurementDisplayBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.MeasurementDisplayBox.Size = new System.Drawing.Size(379, 133);
            this.MeasurementDisplayBox.TabIndex = 61;
            // 
            // Controls
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.MeasurementDisplayBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PositionalCommandTextbox);
            this.Controls.Add(this.PositionalCommandButton);
            this.Controls.Add(this.totalRXdDisplay);
            this.Controls.Add(this.StepInputTextbox);
            this.Controls.Add(this.StepInputButton);
            this.Controls.Add(this.ClearMeasurementButton);
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
            this.Name = "Controls";
            this.Text = "System Characterization";
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
        private System.Windows.Forms.Button ClearMeasurementButton;
        private System.Windows.Forms.Button StepInputButton;
        private System.Windows.Forms.TextBox StepInputTextbox;
        private System.Windows.Forms.Label totalRXdDisplay;
        private System.Windows.Forms.TextBox PositionalCommandTextbox;
        private System.Windows.Forms.Button PositionalCommandButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox MeasurementDisplayBox;
    }
}

