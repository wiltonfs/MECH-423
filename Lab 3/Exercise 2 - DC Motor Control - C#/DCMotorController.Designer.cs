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
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.Stepper_SpeedInput = new System.Windows.Forms.TrackBar();
            this.label8 = new System.Windows.Forms.Label();
            this.StepCW_Button = new System.Windows.Forms.Button();
            this.StepCCW_Button = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DC1_SpeedInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Stepper_SpeedInput)).BeginInit();
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
            this.outQueueDisplay.Location = new System.Drawing.Point(13, 385);
            this.outQueueDisplay.Name = "outQueueDisplay";
            this.outQueueDisplay.Size = new System.Drawing.Size(146, 13);
            this.outQueueDisplay.TabIndex = 23;
            this.outQueueDisplay.Text = "Out Queue: 255 255 255 255";
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
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(372, 257);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 13);
            this.label5.TabIndex = 44;
            this.label5.Text = "Max CW";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(204, 257);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(13, 13);
            this.label6.TabIndex = 43;
            this.label6.Text = "0";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 257);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 42;
            this.label7.Text = "Max CCW";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Stepper_SpeedInput
            // 
            this.Stepper_SpeedInput.Location = new System.Drawing.Point(13, 225);
            this.Stepper_SpeedInput.Maximum = 50;
            this.Stepper_SpeedInput.Minimum = -50;
            this.Stepper_SpeedInput.Name = "Stepper_SpeedInput";
            this.Stepper_SpeedInput.Size = new System.Drawing.Size(394, 45);
            this.Stepper_SpeedInput.TabIndex = 41;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 208);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(111, 13);
            this.label8.TabIndex = 40;
            this.label8.Text = "Stepper Motor Speed:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // StepCW_Button
            // 
            this.StepCW_Button.Location = new System.Drawing.Point(375, 273);
            this.StepCW_Button.Name = "StepCW_Button";
            this.StepCW_Button.Size = new System.Drawing.Size(129, 23);
            this.StepCW_Button.TabIndex = 45;
            this.StepCW_Button.Text = "Step CW";
            this.StepCW_Button.UseVisualStyleBackColor = true;
            this.StepCW_Button.Click += new System.EventHandler(this.StepCW_Button_Click);
            // 
            // StepCCW_Button
            // 
            this.StepCCW_Button.Location = new System.Drawing.Point(9, 273);
            this.StepCCW_Button.Name = "StepCCW_Button";
            this.StepCCW_Button.Size = new System.Drawing.Size(129, 23);
            this.StepCCW_Button.TabIndex = 46;
            this.StepCCW_Button.Text = "Step CCW";
            this.StepCCW_Button.UseVisualStyleBackColor = true;
            this.StepCCW_Button.Click += new System.EventHandler(this.StepCCW_Button_Click);
            // 
            // DCMotorController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.StepCCW_Button);
            this.Controls.Add(this.StepCW_Button);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.Stepper_SpeedInput);
            this.Controls.Add(this.label8);
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
            this.Name = "DCMotorController";
            this.Text = "DC Motor Controller";
            this.Load += new System.EventHandler(this.DcMotorController_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DC1_SpeedInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Stepper_SpeedInput)).EndInit();
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
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TrackBar Stepper_SpeedInput;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button StepCW_Button;
        private System.Windows.Forms.Button StepCCW_Button;
    }
}

