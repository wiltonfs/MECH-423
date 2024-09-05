using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exercise2
{
    public partial class QueueTest : Form
    {
        Queue<Int32> dataQueue = new Queue<Int32>();
        public QueueTest()
        {
            InitializeComponent();
            RefreshVisuals();
        }

        private void QueueTest_Load(object sender, EventArgs e)
        {

        }

        private void RefreshVisuals()
        {
            // Display queue contents
            String queueDisp = "Queue Contents: \n\r";
            foreach (Int32 i in dataQueue)
            {
                queueDisp += i + ", ";
            }
            queueDisplay.Text = queueDisp;

            // Display queue length
            queueSizeDisplay.Text = "Items in Queue: " + dataQueue.Count;
        }

        private void deqAndAvgButton_Click(object sender, EventArgs e)
        {
            String nString = nEntryBox.Text;

            // Try turning nString into an integer
            int n = 0;
            try
            {
                n = Convert.ToInt32(nString);
            }
            catch 
            {
                MessageBox.Show("Could not parse your input for N. Please try again.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (n > dataQueue.Count)
            {
                MessageBox.Show("Your input for N is larger than the ammount of items in the queue. Please try again.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            

            // If there are elements to remove
            if (n > 0)
            {
                int total = 0;
                for (int i = 0; i < n; i++)
                {
                    total += dataQueue.Dequeue();
                }

                double avg = Math.Round((double)total / n, 1);
                averageDisplay.Text = "Average: " + avg.ToString();
            }

            RefreshVisuals();
        }

        private void enqueueButton_Click(object sender, EventArgs e)
        {
            // Get new int and add to queue
            Int32 newEntry = Convert.ToInt32(enqueueEntryBox.Text);
            dataQueue.Enqueue(newEntry);

            RefreshVisuals();
        }

        private void dequeueButton_Click(object sender, EventArgs e)
        {
            if (dataQueue.Count < 1)
            {
                MessageBox.Show("Queue is empty. Nothing to dequeue!", "Empty Queue", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Dequeue first item
            dataQueue.Dequeue();

            RefreshVisuals();
        }
    }
}
