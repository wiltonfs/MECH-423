﻿namespace Exercise4
{
    partial class SerialDemo
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
            this.label3 = new System.Windows.Forms.Label();
            this.serialDisplay = new System.Windows.Forms.TextBox();
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
            this.saveButton = new System.Windows.Forms.Button();
            this.visualTimer = new System.Windows.Forms.Timer(this.components);
            this.lengthInput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
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
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 174);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Serial data stream:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // serialDisplay
            // 
            this.serialDisplay.Location = new System.Drawing.Point(13, 191);
            this.serialDisplay.Multiline = true;
            this.serialDisplay.Name = "serialDisplay";
            this.serialDisplay.Size = new System.Drawing.Size(775, 159);
            this.serialDisplay.TabIndex = 10;
            // 
            // serialPort1
            // 
            this.serialPort1.PortName = "COM3";
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // dataTimer
            // 
            this.dataTimer.Enabled = true;
            this.dataTimer.Interval = 5;
            this.dataTimer.Tick += new System.EventHandler(this.dataTimer_Tick);
            // 
            // aXDisplay
            // 
            this.aXDisplay.Location = new System.Drawing.Point(41, 367);
            this.aXDisplay.Name = "aXDisplay";
            this.aXDisplay.Size = new System.Drawing.Size(100, 20);
            this.aXDisplay.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 370);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "aX:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // aYDisplay
            // 
            this.aYDisplay.Location = new System.Drawing.Point(200, 367);
            this.aYDisplay.Name = "aYDisplay";
            this.aYDisplay.Size = new System.Drawing.Size(100, 20);
            this.aYDisplay.TabIndex = 14;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(171, 370);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "aY:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // aZDisplay
            // 
            this.aZDisplay.Location = new System.Drawing.Point(365, 367);
            this.aZDisplay.Name = "aZDisplay";
            this.aZDisplay.Size = new System.Drawing.Size(100, 20);
            this.aZDisplay.TabIndex = 16;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(336, 370);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(23, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "aZ:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // orientationLabel
            // 
            this.orientationLabel.AutoSize = true;
            this.orientationLabel.Location = new System.Drawing.Point(13, 425);
            this.orientationLabel.Name = "orientationLabel";
            this.orientationLabel.Size = new System.Drawing.Size(129, 13);
            this.orientationLabel.TabIndex = 18;
            this.orientationLabel.Text = "The board is upside down";
            // 
            // unitsCheckbox
            // 
            this.unitsCheckbox.AutoSize = true;
            this.unitsCheckbox.Location = new System.Drawing.Point(15, 405);
            this.unitsCheckbox.Name = "unitsCheckbox";
            this.unitsCheckbox.Size = new System.Drawing.Size(109, 17);
            this.unitsCheckbox.TabIndex = 19;
            this.unitsCheckbox.Text = "Display in m/(ss)?";
            this.unitsCheckbox.UseVisualStyleBackColor = true;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(669, 415);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(119, 23);
            this.saveButton.TabIndex = 20;
            this.saveButton.Text = "Record Values";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // visualTimer
            // 
            this.visualTimer.Enabled = true;
            this.visualTimer.Tick += new System.EventHandler(this.visualTimer_Tick);
            // 
            // lengthInput
            // 
            this.lengthInput.Location = new System.Drawing.Point(122, 82);
            this.lengthInput.Name = "lengthInput";
            this.lengthInput.Size = new System.Drawing.Size(100, 20);
            this.lengthInput.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Temp String length:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SerialDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.unitsCheckbox);
            this.Controls.Add(this.orientationLabel);
            this.Controls.Add(this.aZDisplay);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.aYDisplay);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.aXDisplay);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.serialDisplay);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lengthInput);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.disconnectButton);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.serialComboBox);
            this.Name = "SerialDemo";
            this.Text = "SerialDemo";
            this.Load += new System.EventHandler(this.SerialDemo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox serialComboBox;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.Button disconnectButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox serialDisplay;
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
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Timer visualTimer;
        private System.Windows.Forms.TextBox lengthInput;
        private System.Windows.Forms.Label label1;
    }
}

