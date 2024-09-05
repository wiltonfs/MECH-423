namespace Exercise1
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
            this.xLabel = new System.Windows.Forms.Label();
            this.yLabel = new System.Windows.Forms.Label();
            this.xDisplay = new System.Windows.Forms.TextBox();
            this.yDisplay = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.recordedClicksLabel = new System.Windows.Forms.Label();
            this.recordedClicksTextbox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // xLabel
            // 
            this.xLabel.AutoSize = true;
            this.xLabel.Location = new System.Drawing.Point(13, 13);
            this.xLabel.Name = "xLabel";
            this.xLabel.Size = new System.Drawing.Size(17, 13);
            this.xLabel.TabIndex = 0;
            this.xLabel.Text = "X:";
            this.xLabel.Click += new System.EventHandler(this.label1_Click);
            // 
            // yLabel
            // 
            this.yLabel.AutoSize = true;
            this.yLabel.Location = new System.Drawing.Point(14, 43);
            this.yLabel.Name = "yLabel";
            this.yLabel.Size = new System.Drawing.Size(17, 13);
            this.yLabel.TabIndex = 1;
            this.yLabel.Text = "Y:";
            // 
            // xDisplay
            // 
            this.xDisplay.Location = new System.Drawing.Point(36, 10);
            this.xDisplay.Name = "xDisplay";
            this.xDisplay.Size = new System.Drawing.Size(100, 20);
            this.xDisplay.TabIndex = 2;
            // 
            // yDisplay
            // 
            this.yDisplay.Location = new System.Drawing.Point(36, 36);
            this.yDisplay.Name = "yDisplay";
            this.yDisplay.Size = new System.Drawing.Size(100, 20);
            this.yDisplay.TabIndex = 3;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(164, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(624, 425);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            // 
            // recordedClicksLabel
            // 
            this.recordedClicksLabel.AutoSize = true;
            this.recordedClicksLabel.Location = new System.Drawing.Point(33, 84);
            this.recordedClicksLabel.Name = "recordedClicksLabel";
            this.recordedClicksLabel.Size = new System.Drawing.Size(88, 13);
            this.recordedClicksLabel.TabIndex = 5;
            this.recordedClicksLabel.Text = "Recorded Clicks:";
            // 
            // recordedClicksTextbox
            // 
            this.recordedClicksTextbox.Location = new System.Drawing.Point(16, 117);
            this.recordedClicksTextbox.Multiline = true;
            this.recordedClicksTextbox.Name = "recordedClicksTextbox";
            this.recordedClicksTextbox.Size = new System.Drawing.Size(120, 321);
            this.recordedClicksTextbox.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.recordedClicksTextbox);
            this.Controls.Add(this.recordedClicksLabel);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.yDisplay);
            this.Controls.Add(this.xDisplay);
            this.Controls.Add(this.yLabel);
            this.Controls.Add(this.xLabel);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label xLabel;
        private System.Windows.Forms.Label yLabel;
        private System.Windows.Forms.TextBox xDisplay;
        private System.Windows.Forms.TextBox yDisplay;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label recordedClicksLabel;
        private System.Windows.Forms.TextBox recordedClicksTextbox;
    }
}

