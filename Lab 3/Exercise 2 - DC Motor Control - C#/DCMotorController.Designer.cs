namespace DCMotorController
{
    partial class DCMotorController
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
            this.dataTimer = new System.Windows.Forms.Timer(this.components);
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
            ((System.ComponentModel.ISupportInitialize)(this.DC1_SpeedInput)).BeginInit();
            this.SuspendLayout();
            // 
            // serialComboBox
            // 
            this.serialComboBox.FormattingEnabled = true;
            this.serialComboBox.Location = new System.Drawing.Point(17, 16);
            this.serialComboBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.serialComboBox.Name = "serialComboBox";
            this.serialComboBox.Size = new System.Drawing.Size(160, 24);
            this.serialComboBox.TabIndex = 0;
            this.serialComboBox.SelectedIndexChanged += new System.EventHandler(this.serialComboBox_SelectedIndexChanged);
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(188, 12);
            this.refreshButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(172, 28);
            this.refreshButton.TabIndex = 1;
            this.refreshButton.Text = "Refresh Serial Ports";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // disconnectButton
            // 
            this.disconnectButton.Location = new System.Drawing.Point(368, 12);
            this.disconnectButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new System.Drawing.Size(159, 28);
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
            // dataTimer
            // 
            this.dataTimer.Enabled = true;
            this.dataTimer.Interval = 50;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 119);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(110, 16);
            this.label4.TabIndex = 11;
            this.label4.Text = "DC Motor Speed:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // boardConnectedLabel
            // 
            this.boardConnectedLabel.AutoSize = true;
            this.boardConnectedLabel.Location = new System.Drawing.Point(19, 46);
            this.boardConnectedLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.boardConnectedLabel.Name = "boardConnectedLabel";
            this.boardConnectedLabel.Size = new System.Drawing.Size(120, 16);
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
            this.outQueueDisplay.Location = new System.Drawing.Point(17, 260);
            this.outQueueDisplay.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.outQueueDisplay.Name = "outQueueDisplay";
            this.outQueueDisplay.Size = new System.Drawing.Size(169, 16);
            this.outQueueDisplay.TabIndex = 23;
            this.outQueueDisplay.Text = "Out Queue: 255 255 255 255";
            this.outQueueDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // postStringVisual
            // 
            this.postStringVisual.AutoSize = true;
            this.postStringVisual.Location = new System.Drawing.Point(19, 468);
            this.postStringVisual.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.postStringVisual.Name = "postStringVisual";
            this.postStringVisual.Size = new System.Drawing.Size(0, 16);
            this.postStringVisual.TabIndex = 33;
            // 
            // preStringVisual
            // 
            this.preStringVisual.AutoSize = true;
            this.preStringVisual.Location = new System.Drawing.Point(19, 452);
            this.preStringVisual.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.preStringVisual.Name = "preStringVisual";
            this.preStringVisual.Size = new System.Drawing.Size(0, 16);
            this.preStringVisual.TabIndex = 34;
            // 
            // DC1_SpeedInput
            // 
            this.DC1_SpeedInput.Location = new System.Drawing.Point(23, 140);
            this.DC1_SpeedInput.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.DC1_SpeedInput.Maximum = 50;
            this.DC1_SpeedInput.Minimum = -50;
            this.DC1_SpeedInput.Name = "DC1_SpeedInput";
            this.DC1_SpeedInput.Size = new System.Drawing.Size(525, 56);
            this.DC1_SpeedInput.TabIndex = 35;
            this.DC1_SpeedInput.Scroll += new System.EventHandler(this.DC1_SpeedInput_Scroll);
            this.DC1_SpeedInput.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DC1_SpeedInput_MouseUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 180);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 16);
            this.label1.TabIndex = 37;
            this.label1.Text = "Max CCW";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(277, 180);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 16);
            this.label2.TabIndex = 38;
            this.label2.Text = "0";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(501, 180);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 16);
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
            // DCMotorController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 554);
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
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "DCMotorController";
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
        private System.Windows.Forms.Timer dataTimer;
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
    }
}

