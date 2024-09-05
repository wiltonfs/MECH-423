namespace Exercise2
{
    partial class QueueTest
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
            this.enqueueButton = new System.Windows.Forms.Button();
            this.dequeueButton = new System.Windows.Forms.Button();
            this.queueSizeDisplay = new System.Windows.Forms.Label();
            this.enqueueEntryBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.deqAndAvgButton = new System.Windows.Forms.Button();
            this.nLabel = new System.Windows.Forms.Label();
            this.nEntryBox = new System.Windows.Forms.TextBox();
            this.averageDisplay = new System.Windows.Forms.Label();
            this.queueDisplay = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // enqueueButton
            // 
            this.enqueueButton.Location = new System.Drawing.Point(13, 13);
            this.enqueueButton.Name = "enqueueButton";
            this.enqueueButton.Size = new System.Drawing.Size(75, 23);
            this.enqueueButton.TabIndex = 0;
            this.enqueueButton.Text = "Enqueue";
            this.enqueueButton.UseVisualStyleBackColor = true;
            this.enqueueButton.Click += new System.EventHandler(this.enqueueButton_Click);
            // 
            // dequeueButton
            // 
            this.dequeueButton.Location = new System.Drawing.Point(13, 43);
            this.dequeueButton.Name = "dequeueButton";
            this.dequeueButton.Size = new System.Drawing.Size(182, 23);
            this.dequeueButton.TabIndex = 1;
            this.dequeueButton.Text = "Dequeue first item";
            this.dequeueButton.UseVisualStyleBackColor = true;
            this.dequeueButton.Click += new System.EventHandler(this.dequeueButton_Click);
            // 
            // queueSizeDisplay
            // 
            this.queueSizeDisplay.AutoSize = true;
            this.queueSizeDisplay.Location = new System.Drawing.Point(13, 73);
            this.queueSizeDisplay.Name = "queueSizeDisplay";
            this.queueSizeDisplay.Size = new System.Drawing.Size(90, 13);
            this.queueSizeDisplay.TabIndex = 2;
            this.queueSizeDisplay.Text = "Items in Queue: 0";
            // 
            // enqueueEntryBox
            // 
            this.enqueueEntryBox.Location = new System.Drawing.Point(95, 13);
            this.enqueueEntryBox.Name = "enqueueEntryBox";
            this.enqueueEntryBox.Size = new System.Drawing.Size(100, 20);
            this.enqueueEntryBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(95, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 5;
            // 
            // deqAndAvgButton
            // 
            this.deqAndAvgButton.Location = new System.Drawing.Point(13, 90);
            this.deqAndAvgButton.Name = "deqAndAvgButton";
            this.deqAndAvgButton.Size = new System.Drawing.Size(313, 23);
            this.deqAndAvgButton.TabIndex = 6;
            this.deqAndAvgButton.Text = "Dequeue and Average first N data points";
            this.deqAndAvgButton.UseVisualStyleBackColor = true;
            this.deqAndAvgButton.Click += new System.EventHandler(this.deqAndAvgButton_Click);
            // 
            // nLabel
            // 
            this.nLabel.AutoSize = true;
            this.nLabel.Location = new System.Drawing.Point(13, 120);
            this.nLabel.Name = "nLabel";
            this.nLabel.Size = new System.Drawing.Size(21, 13);
            this.nLabel.TabIndex = 7;
            this.nLabel.Text = "N: ";
            // 
            // nEntryBox
            // 
            this.nEntryBox.Location = new System.Drawing.Point(40, 117);
            this.nEntryBox.Name = "nEntryBox";
            this.nEntryBox.Size = new System.Drawing.Size(100, 20);
            this.nEntryBox.TabIndex = 8;
            // 
            // averageDisplay
            // 
            this.averageDisplay.AutoSize = true;
            this.averageDisplay.Location = new System.Drawing.Point(160, 120);
            this.averageDisplay.Name = "averageDisplay";
            this.averageDisplay.Size = new System.Drawing.Size(53, 13);
            this.averageDisplay.TabIndex = 9;
            this.averageDisplay.Text = "Average: ";
            // 
            // queueDisplay
            // 
            this.queueDisplay.AutoSize = true;
            this.queueDisplay.Location = new System.Drawing.Point(13, 171);
            this.queueDisplay.Name = "queueDisplay";
            this.queueDisplay.Size = new System.Drawing.Size(87, 13);
            this.queueDisplay.TabIndex = 10;
            this.queueDisplay.Text = "Queue Contents:";
            // 
            // QueueTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.queueDisplay);
            this.Controls.Add(this.averageDisplay);
            this.Controls.Add(this.nEntryBox);
            this.Controls.Add(this.nLabel);
            this.Controls.Add(this.deqAndAvgButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.enqueueEntryBox);
            this.Controls.Add(this.queueSizeDisplay);
            this.Controls.Add(this.dequeueButton);
            this.Controls.Add(this.enqueueButton);
            this.Name = "QueueTest";
            this.Text = "QueueTest";
            this.Load += new System.EventHandler(this.QueueTest_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button enqueueButton;
        private System.Windows.Forms.Button dequeueButton;
        private System.Windows.Forms.Label queueSizeDisplay;
        private System.Windows.Forms.TextBox enqueueEntryBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button deqAndAvgButton;
        private System.Windows.Forms.Label nLabel;
        private System.Windows.Forms.TextBox nEntryBox;
        private System.Windows.Forms.Label averageDisplay;
        private System.Windows.Forms.Label queueDisplay;
    }
}

