using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exercise1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int x = (int)e.X;
            int y = (int)e.Y;

            xDisplay.Text = x.ToString();
            yDisplay.Text = y.ToString();
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            // Append new coords to text box
            String newCoords = "(" + (int)e.X + ", " + (int)e.Y + ") \r\n";
            recordedClicksTextbox.AppendText(newCoords);
        }
    }
}
