namespace Exercise7
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.xInput = new System.Windows.Forms.TextBox();
            this.yInput = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.zInput = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ProcessButton = new System.Windows.Forms.Button();
            this.StateDisplay = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.historyDisplay = new System.Windows.Forms.TextBox();
            this.gestureDisplay = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.gestureCheck = new System.Windows.Forms.Timer(this.components);
            this.StateStringDisplay = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ax";
            // 
            // xInput
            // 
            this.xInput.Location = new System.Drawing.Point(38, 13);
            this.xInput.Name = "xInput";
            this.xInput.Size = new System.Drawing.Size(100, 20);
            this.xInput.TabIndex = 1;
            // 
            // yInput
            // 
            this.yInput.Location = new System.Drawing.Point(169, 13);
            this.yInput.Name = "yInput";
            this.yInput.Size = new System.Drawing.Size(100, 20);
            this.yInput.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(144, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Ay";
            // 
            // zInput
            // 
            this.zInput.Location = new System.Drawing.Point(300, 13);
            this.zInput.Name = "zInput";
            this.zInput.Size = new System.Drawing.Size(100, 20);
            this.zInput.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(275, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(19, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Az";
            // 
            // ProcessButton
            // 
            this.ProcessButton.Location = new System.Drawing.Point(16, 43);
            this.ProcessButton.Name = "ProcessButton";
            this.ProcessButton.Size = new System.Drawing.Size(384, 23);
            this.ProcessButton.TabIndex = 6;
            this.ProcessButton.Text = "Process New Data Point";
            this.ProcessButton.UseVisualStyleBackColor = true;
            this.ProcessButton.Click += new System.EventHandler(this.ProcessButton_Click);
            // 
            // StateDisplay
            // 
            this.StateDisplay.AutoSize = true;
            this.StateDisplay.Location = new System.Drawing.Point(16, 73);
            this.StateDisplay.Name = "StateDisplay";
            this.StateDisplay.Size = new System.Drawing.Size(75, 13);
            this.StateDisplay.TabIndex = 7;
            this.StateDisplay.Text = "Current State: ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 116);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "State History:";
            // 
            // historyDisplay
            // 
            this.historyDisplay.Location = new System.Drawing.Point(22, 133);
            this.historyDisplay.Multiline = true;
            this.historyDisplay.Name = "historyDisplay";
            this.historyDisplay.Size = new System.Drawing.Size(378, 305);
            this.historyDisplay.TabIndex = 9;
            // 
            // gestureDisplay
            // 
            this.gestureDisplay.Location = new System.Drawing.Point(406, 133);
            this.gestureDisplay.Multiline = true;
            this.gestureDisplay.Name = "gestureDisplay";
            this.gestureDisplay.Size = new System.Drawing.Size(378, 305);
            this.gestureDisplay.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(403, 116);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Gesture History:";
            // 
            // gestureCheck
            // 
            this.gestureCheck.Enabled = true;
            this.gestureCheck.Tick += new System.EventHandler(this.gestureCheck_Tick);
            // 
            // StateStringDisplay
            // 
            this.StateStringDisplay.AutoSize = true;
            this.StateStringDisplay.Location = new System.Drawing.Point(403, 94);
            this.StateStringDisplay.Name = "StateStringDisplay";
            this.StateStringDisplay.Size = new System.Drawing.Size(93, 13);
            this.StateStringDisplay.TabIndex = 12;
            this.StateStringDisplay.Text = "StateStringDisplay";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.StateStringDisplay);
            this.Controls.Add(this.gestureDisplay);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.historyDisplay);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.StateDisplay);
            this.Controls.Add(this.ProcessButton);
            this.Controls.Add(this.zInput);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.yInput);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.xInput);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox xInput;
        private System.Windows.Forms.TextBox yInput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox zInput;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button ProcessButton;
        private System.Windows.Forms.Label StateDisplay;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox historyDisplay;
        private System.Windows.Forms.TextBox gestureDisplay;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Timer gestureCheck;
        private System.Windows.Forms.Label StateStringDisplay;
    }
}

