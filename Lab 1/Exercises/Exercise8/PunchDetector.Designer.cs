﻿namespace Exercise4
{
    partial class PunchDetector
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
            this.aXDisplay = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.aYDisplay = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.aZDisplay = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.orientationLabel = new System.Windows.Forms.Label();
            this.unitsCheckbox = new System.Windows.Forms.CheckBox();
            this.visualTimer = new System.Windows.Forms.Timer(this.components);
            this.avgZDisplay = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.avgYDisplay = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.avgXDisplay = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.avgLabel = new System.Windows.Forms.Label();
            this.serialBufferSizeLabel = new System.Windows.Forms.Label();
            this.queueSizeLabel = new System.Windows.Forms.Label();
            this.gestureDisplayLabel = new System.Windows.Forms.Label();
            this.currentStateDisplay = new System.Windows.Forms.Label();
            this.stateHistory = new System.Windows.Forms.TextBox();
            this.postStringVisual = new System.Windows.Forms.Label();
            this.preStringVisual = new System.Windows.Forms.Label();
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
            // dataTimer
            // 
            this.dataTimer.Enabled = true;
            this.dataTimer.Interval = 50;
            this.dataTimer.Tick += new System.EventHandler(this.dataTimer_Tick);
            // 
            // aXDisplay
            // 
            this.aXDisplay.Location = new System.Drawing.Point(42, 94);
            this.aXDisplay.Name = "aXDisplay";
            this.aXDisplay.Size = new System.Drawing.Size(100, 20);
            this.aXDisplay.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "aX:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // aYDisplay
            // 
            this.aYDisplay.Location = new System.Drawing.Point(201, 94);
            this.aYDisplay.Name = "aYDisplay";
            this.aYDisplay.Size = new System.Drawing.Size(100, 20);
            this.aYDisplay.TabIndex = 14;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(172, 97);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "aY:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // aZDisplay
            // 
            this.aZDisplay.Location = new System.Drawing.Point(366, 94);
            this.aZDisplay.Name = "aZDisplay";
            this.aZDisplay.Size = new System.Drawing.Size(100, 20);
            this.aZDisplay.TabIndex = 16;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(337, 97);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(23, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "aZ:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // orientationLabel
            // 
            this.orientationLabel.AutoSize = true;
            this.orientationLabel.Location = new System.Drawing.Point(13, 124);
            this.orientationLabel.Name = "orientationLabel";
            this.orientationLabel.Size = new System.Drawing.Size(96, 13);
            this.orientationLabel.TabIndex = 18;
            this.orientationLabel.Text = "No board detected";
            // 
            // unitsCheckbox
            // 
            this.unitsCheckbox.AutoSize = true;
            this.unitsCheckbox.Location = new System.Drawing.Point(16, 71);
            this.unitsCheckbox.Name = "unitsCheckbox";
            this.unitsCheckbox.Size = new System.Drawing.Size(109, 17);
            this.unitsCheckbox.TabIndex = 19;
            this.unitsCheckbox.Text = "Display in m/(ss)?";
            this.unitsCheckbox.UseVisualStyleBackColor = true;
            // 
            // visualTimer
            // 
            this.visualTimer.Enabled = true;
            this.visualTimer.Tick += new System.EventHandler(this.visualTimer_Tick);
            // 
            // avgZDisplay
            // 
            this.avgZDisplay.Location = new System.Drawing.Point(366, 206);
            this.avgZDisplay.Name = "avgZDisplay";
            this.avgZDisplay.Size = new System.Drawing.Size(100, 20);
            this.avgZDisplay.TabIndex = 26;
            this.avgZDisplay.Text = "Not enough vals";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(332, 209);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "avgZ:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // avgYDisplay
            // 
            this.avgYDisplay.Location = new System.Drawing.Point(201, 206);
            this.avgYDisplay.Name = "avgYDisplay";
            this.avgYDisplay.Size = new System.Drawing.Size(100, 20);
            this.avgYDisplay.TabIndex = 24;
            this.avgYDisplay.Text = "Not enough vals";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(167, 209);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "avgY:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // avgXDisplay
            // 
            this.avgXDisplay.Location = new System.Drawing.Point(42, 206);
            this.avgXDisplay.Name = "avgXDisplay";
            this.avgXDisplay.Size = new System.Drawing.Size(100, 20);
            this.avgXDisplay.TabIndex = 22;
            this.avgXDisplay.Text = "Not enough vals";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 209);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "avgX:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // avgLabel
            // 
            this.avgLabel.AutoSize = true;
            this.avgLabel.Location = new System.Drawing.Point(8, 191);
            this.avgLabel.Name = "avgLabel";
            this.avgLabel.Size = new System.Drawing.Size(215, 13);
            this.avgLabel.TabIndex = 27;
            this.avgLabel.Text = "Average acceleration over last n data points";
            // 
            // serialBufferSizeLabel
            // 
            this.serialBufferSizeLabel.AutoSize = true;
            this.serialBufferSizeLabel.Location = new System.Drawing.Point(13, 267);
            this.serialBufferSizeLabel.Name = "serialBufferSizeLabel";
            this.serialBufferSizeLabel.Size = new System.Drawing.Size(112, 13);
            this.serialBufferSizeLabel.TabIndex = 28;
            this.serialBufferSizeLabel.Text = "Serial buffer size: NaN";
            // 
            // queueSizeLabel
            // 
            this.queueSizeLabel.AutoSize = true;
            this.queueSizeLabel.Location = new System.Drawing.Point(13, 289);
            this.queueSizeLabel.Name = "queueSizeLabel";
            this.queueSizeLabel.Size = new System.Drawing.Size(88, 13);
            this.queueSizeLabel.TabIndex = 29;
            this.queueSizeLabel.Text = "Queue size: NaN";
            // 
            // gestureDisplayLabel
            // 
            this.gestureDisplayLabel.AutoSize = true;
            this.gestureDisplayLabel.Location = new System.Drawing.Point(13, 393);
            this.gestureDisplayLabel.Name = "gestureDisplayLabel";
            this.gestureDisplayLabel.Size = new System.Drawing.Size(109, 13);
            this.gestureDisplayLabel.TabIndex = 30;
            this.gestureDisplayLabel.Text = "Try making a gesture!";
            // 
            // currentStateDisplay
            // 
            this.currentStateDisplay.AutoSize = true;
            this.currentStateDisplay.Location = new System.Drawing.Point(335, 267);
            this.currentStateDisplay.Name = "currentStateDisplay";
            this.currentStateDisplay.Size = new System.Drawing.Size(70, 13);
            this.currentStateDisplay.TabIndex = 31;
            this.currentStateDisplay.Text = "Current state:";
            // 
            // stateHistory
            // 
            this.stateHistory.Location = new System.Drawing.Point(335, 286);
            this.stateHistory.Multiline = true;
            this.stateHistory.Name = "stateHistory";
            this.stateHistory.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.stateHistory.Size = new System.Drawing.Size(453, 152);
            this.stateHistory.TabIndex = 32;
            // 
            // postStringVisual
            // 
            this.postStringVisual.AutoSize = true;
            this.postStringVisual.Location = new System.Drawing.Point(14, 380);
            this.postStringVisual.Name = "postStringVisual";
            this.postStringVisual.Size = new System.Drawing.Size(112, 13);
            this.postStringVisual.TabIndex = 33;
            this.postStringVisual.Text = "Cleaned gesture string";
            // 
            // preStringVisual
            // 
            this.preStringVisual.AutoSize = true;
            this.preStringVisual.Location = new System.Drawing.Point(14, 367);
            this.preStringVisual.Name = "preStringVisual";
            this.preStringVisual.Size = new System.Drawing.Size(95, 13);
            this.preStringVisual.TabIndex = 34;
            this.preStringVisual.Text = "Raw gesture string";
            // 
            // PunchDetector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.preStringVisual);
            this.Controls.Add(this.postStringVisual);
            this.Controls.Add(this.stateHistory);
            this.Controls.Add(this.currentStateDisplay);
            this.Controls.Add(this.gestureDisplayLabel);
            this.Controls.Add(this.queueSizeLabel);
            this.Controls.Add(this.serialBufferSizeLabel);
            this.Controls.Add(this.avgLabel);
            this.Controls.Add(this.avgZDisplay);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.avgYDisplay);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.avgXDisplay);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.unitsCheckbox);
            this.Controls.Add(this.orientationLabel);
            this.Controls.Add(this.aZDisplay);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.aYDisplay);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.aXDisplay);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.disconnectButton);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.serialComboBox);
            this.Name = "PunchDetector";
            this.Text = "Punch Detector";
            this.Load += new System.EventHandler(this.SerialDemo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox serialComboBox;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.Button disconnectButton;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Timer dataTimer;
        private System.Windows.Forms.TextBox aXDisplay;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox aYDisplay;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox aZDisplay;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label orientationLabel;
        private System.Windows.Forms.CheckBox unitsCheckbox;
        private System.Windows.Forms.Timer visualTimer;
        private System.Windows.Forms.TextBox avgZDisplay;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox avgYDisplay;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox avgXDisplay;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label avgLabel;
        private System.Windows.Forms.Label serialBufferSizeLabel;
        private System.Windows.Forms.Label queueSizeLabel;
        private System.Windows.Forms.Label gestureDisplayLabel;
        private System.Windows.Forms.Label currentStateDisplay;
        private System.Windows.Forms.TextBox stateHistory;
        private System.Windows.Forms.Label postStringVisual;
        private System.Windows.Forms.Label preStringVisual;
    }
}

