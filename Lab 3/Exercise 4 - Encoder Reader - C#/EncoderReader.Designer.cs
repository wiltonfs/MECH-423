namespace DCMotorController
{
    partial class EncoderReader
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
            this.inputsTimer = new System.Windows.Forms.Timer(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.boardConnectedLabel = new System.Windows.Forms.Label();
            this.visualTimer = new System.Windows.Forms.Timer(this.components);
            this.outQueueDisplay = new System.Windows.Forms.Label();
            this.postStringVisual = new System.Windows.Forms.Label();
            this.preStringVisual = new System.Windows.Forms.Label();
            this.DC1_SpeedInput = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txTimer = new System.Windows.Forms.Timer(this.components);
            this.velocityLabelHz = new System.Windows.Forms.Label();
            this.positionLabel = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txHistoryDisplay = new System.Windows.Forms.TextBox();
            this.rxHistoryDisplay = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.velocityLabelRPM = new System.Windows.Forms.Label();
            this.rxTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.DC1_SpeedInput)).BeginInit();
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
            // inputsTimer
            // 
            this.inputsTimer.Enabled = true;
            this.inputsTimer.Interval = 50;
            this.inputsTimer.Tick += new System.EventHandler(this.inputsTimer_Tick);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "DC Motor Speed:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            this.outQueueDisplay.Location = new System.Drawing.Point(13, 339);
            this.outQueueDisplay.Name = "outQueueDisplay";
            this.outQueueDisplay.Size = new System.Drawing.Size(184, 13);
            this.outQueueDisplay.TabIndex = 23;
            this.outQueueDisplay.Text = "Queue pending TX: 255 255 255 255";
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
            // DC1_SpeedInput
            // 
            this.DC1_SpeedInput.Location = new System.Drawing.Point(17, 114);
            this.DC1_SpeedInput.Maximum = 50;
            this.DC1_SpeedInput.Minimum = -50;
            this.DC1_SpeedInput.Name = "DC1_SpeedInput";
            this.DC1_SpeedInput.Size = new System.Drawing.Size(394, 45);
            this.DC1_SpeedInput.TabIndex = 35;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 146);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 37;
            this.label1.Text = "Max CCW";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(208, 146);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 13);
            this.label2.TabIndex = 38;
            this.label2.Text = "0";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(376, 146);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 39;
            this.label3.Text = "Max CW";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txTimer
            // 
            this.txTimer.Enabled = true;
            this.txTimer.Interval = 15;
            this.txTimer.Tick += new System.EventHandler(this.txTimer_Tick);
            // 
            // velocityLabelHz
            // 
            this.velocityLabelHz.AutoSize = true;
            this.velocityLabelHz.Location = new System.Drawing.Point(9, 232);
            this.velocityLabelHz.Name = "velocityLabelHz";
            this.velocityLabelHz.Size = new System.Drawing.Size(112, 13);
            this.velocityLabelHz.TabIndex = 42;
            this.velocityLabelHz.Text = "Encoder Velocity (Hz):";
            this.velocityLabelHz.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // positionLabel
            // 
            this.positionLabel.AutoSize = true;
            this.positionLabel.Location = new System.Drawing.Point(9, 208);
            this.positionLabel.Name = "positionLabel";
            this.positionLabel.Size = new System.Drawing.Size(93, 13);
            this.positionLabel.TabIndex = 40;
            this.positionLabel.Text = "Encoder Position: ";
            this.positionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            // velocityLabelRPM
            // 
            this.velocityLabelRPM.AutoSize = true;
            this.velocityLabelRPM.Location = new System.Drawing.Point(9, 255);
            this.velocityLabelRPM.Name = "velocityLabelRPM";
            this.velocityLabelRPM.Size = new System.Drawing.Size(123, 13);
            this.velocityLabelRPM.TabIndex = 51;
            this.velocityLabelRPM.Text = "Encoder Velocity (RPM):";
            this.velocityLabelRPM.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // rxTimer
            // 
            this.rxTimer.Enabled = true;
            this.rxTimer.Interval = 5;
            this.rxTimer.Tick += new System.EventHandler(this.rxTimer_Tick);
            // 
            // EncoderReader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.velocityLabelRPM);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.rxHistoryDisplay);
            this.Controls.Add(this.txHistoryDisplay);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.velocityLabelHz);
            this.Controls.Add(this.positionLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DC1_SpeedInput);
            this.Controls.Add(this.preStringVisual);
            this.Controls.Add(this.postStringVisual);
            this.Controls.Add(this.outQueueDisplay);
            this.Controls.Add(this.boardConnectedLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.disconnectButton);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.serialComboBox);
            this.Name = "EncoderReader";
            this.Text = "DC Motor Controller";
            this.Load += new System.EventHandler(this.DcMotorController_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DC1_SpeedInput)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox serialComboBox;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.Button disconnectButton;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Timer inputsTimer;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label boardConnectedLabel;
        private System.Windows.Forms.Timer visualTimer;
        private System.Windows.Forms.Label outQueueDisplay;
        private System.Windows.Forms.Label postStringVisual;
        private System.Windows.Forms.Label preStringVisual;
        private System.Windows.Forms.TrackBar DC1_SpeedInput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Timer txTimer;
        private System.Windows.Forms.Label velocityLabelHz;
        private System.Windows.Forms.Label positionLabel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txHistoryDisplay;
        private System.Windows.Forms.TextBox rxHistoryDisplay;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label velocityLabelRPM;
        private System.Windows.Forms.Timer rxTimer;
    }
}

