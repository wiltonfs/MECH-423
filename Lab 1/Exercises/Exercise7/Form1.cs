using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exercise7
{
    enum PunchState
    {
        IDLE,
        X_POS_acc, X_NEG_acc,
        Y_POS_acc, Y_NEG_acc,
        Z_POS_acc, Z_NEG_acc,
    }
    public partial class Form1 : Form
    {
        PunchState currentState;

        Queue<PunchState> stateQueue = new Queue<PunchState>();
        
        public Form1()
        {
            InitializeComponent();
            currentState = PunchState.IDLE;
            historyDisplay.AppendText("Starting State: " +  currentState + "\r\n");
            UpdateStateDisplay();
        }

        private void ProcessButton_Click(object sender, EventArgs e)
        {
            // First, read and validate Ax,Ay,Az
            float x, y, z;

            if (float.TryParse(xInput.Text, out x) && float.TryParse(yInput.Text, out y) && float.TryParse(zInput.Text, out z))
            {
                // Pass
            }
            else 
            {
                MessageBox.Show("Input must be a floating point number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; 
            }

            Vec3 InputAccel = new Vec3(x, y, z);

            // Then, do the state transition thinking
            PunchState lastState = currentState;
            currentState = EvaluateState(InputAccel);
            stateQueue.Enqueue(currentState);

            // Update and display the state history and state
            historyDisplay.AppendText(lastState + " -> " + InputAccel.ToString() + " -> " + currentState + "\r\n");
            UpdateStateDisplay();
        }

        private PunchState EvaluateState(Vec3 Accel)
        {
            float absX = Math.Abs(Accel.X); float absY = Math.Abs(Accel.Y); float absZ = Math.Abs(Accel.Z);

            // Minimum accel to not be idle
            float thresholdAccel = 3.0f;

            if (absX > thresholdAccel && absX >= absY && absX >= absZ)
            {
                // X wins out
                if (Accel.X > 0)
                {
                    return PunchState.X_POS_acc;
                }
                return PunchState.X_NEG_acc;
            }
            else if (absY > thresholdAccel && absY >= absX && absY >= absZ)
            {
                // Y wins out
                if (Accel.Y > 0)
                {
                    return PunchState.Y_POS_acc;
                }
                return PunchState.Y_NEG_acc;
            }
            else if (absZ > thresholdAccel && absZ >= absX && absZ >= absY)
            {
                // Z wins out
                if (Accel.Z > 0)
                {
                    return PunchState.Z_POS_acc;
                }
                return PunchState.Z_NEG_acc;
            }

            // Nothing is bigger than the threshold value so still idle
            return PunchState.IDLE;
        }

        private void UpdateStateDisplay()
        {
            StateDisplay.Text = "Current State: " + currentState;
        }

        private void gestureCheck_Tick(object sender, EventArgs e)
        {
            string stateString = PunchStringFormat(QueueToString());

            StateStringDisplay.Text = stateString;

            // if ending with an idle
            if (stateString.Length > 1 && stateString.EndsWith("P"))
            {
                if (stateString.Equals("XYZP"))
                {
                    // Right-hook
                    gestureDisplay.AppendText("Right Hook \r\n");
                } else if (stateString.Equals("ZXP"))
                {
                    // High punch
                    gestureDisplay.AppendText("High Punch \r\n");
                } else if (stateString.Equals("XP"))
                {
                    // Simple punch
                    gestureDisplay.AppendText("Simple Punch \r\n");
                } else
                {
                    gestureDisplay.AppendText("Invalid Gesture \r\n");
                }

                // Clear gesture tracking
                stateQueue.Clear();
            }
        }

        // ----------------------------------------------
        // ---------- Gesture String Functions ----------
        // ----------------------------------------------

        private string QueueToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (PunchState item in stateQueue)
            {
                switch (item)
                {
                    case PunchState.IDLE:
                        sb.Append("I");
                        break;
                    case PunchState.X_POS_acc:
                        sb.Append("X");
                        break;
                    case PunchState.X_NEG_acc:
                        sb.Append("A");
                        break;
                    case PunchState.Y_POS_acc:
                        sb.Append("Y");
                        break;
                    case PunchState.Y_NEG_acc:
                        sb.Append("B");
                        break;
                    case PunchState.Z_POS_acc:
                        sb.Append("Z");
                        break;
                    case PunchState.Z_NEG_acc:
                        sb.Append("C");
                        break;
                    default:
                        break;
                }
            }

            return sb.ToString();
        }

        private string PunchStringFormat(string input)
        {
            // Number of sequential "idles" before considered a "pause"
            int idlesToPause = 5;

            // Ignore idles < idlesToPause, insert a pause for idles > idlesToPause
            StringBuilder sb = new StringBuilder();
            int idleCount = 0;
            foreach (char currentChar in input)
            {
                if (currentChar == 'I')
                {
                    idleCount++;
                    if (idleCount >= idlesToPause)
                    {
                        sb.Append("P");
                    }
                } 
                else
                {
                    idleCount = 0;
                    sb.Append(currentChar);
                }
            }
            input = sb.ToString();

            // - Collapse duplicates of characters
            sb = new StringBuilder();
            char previousChar = '\0';
            foreach (char currentChar in input)
            {
                if (currentChar != previousChar)
                {
                    sb.Append(currentChar);
                }
                previousChar = currentChar;
            }
            input = sb.ToString();


            // - Delete paired deccelerations
            sb = new StringBuilder();
            previousChar = '\0';
            foreach (char currentChar in input)
            {
                if (InvertFromChar(currentChar) == previousChar)
                { }
                else
                { sb.Append(currentChar); }
                previousChar = currentChar;
            }
            input = sb.ToString();

            // - Remove any 'P' from the start of the string
            sb = new StringBuilder();
            bool pastStartingP = false;
            foreach (char currentChar in input)
            {
                if (pastStartingP)
                {
                    sb.Append(currentChar);
                } else if (currentChar != 'P')
                {
                    sb.Append(currentChar);
                    pastStartingP = true;
                }
            }
            input = sb.ToString();

            return input;
        }

        private char InvertFromChar(char input)
        {
            char[] inputs =  { 'X', 'Y', 'Z', 'A', 'B', 'C'};
            char[] outputs = { 'A', 'B', 'C', 'X', 'Y', 'Z'};

            for (int i = 0; i < inputs.Length; i++)
            {
                if (input == inputs[i])
                {
                    return outputs[i];
                }
            }

            return '_';
        }
    }
}
