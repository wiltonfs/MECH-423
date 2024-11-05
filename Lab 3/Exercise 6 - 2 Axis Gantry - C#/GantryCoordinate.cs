using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exercise6
{
    public class GantryCoordinate
    {
        public const int X_WIDTH_CM = 8;
        public const int Y_WIDTH_CM = 8;

        public bool absoluteCoordinate = true;
        public float X = 0;
        public float Y = 0;
        public uint Speed = 100;

        public GantryCoordinate(float x=0f, float y=0f, uint s=100, bool abs = true)
        {
            X = Clamp(x, -X_WIDTH_CM, X_WIDTH_CM);
            Y = Clamp(y, -Y_WIDTH_CM, Y_WIDTH_CM);
            Speed = Clamp(s, 10, 100);
            absoluteCoordinate = abs;
        }
        public static uint Clamp(uint value, uint min, uint max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static float Clamp(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public Queue<MessagePacket> ConvertToCommands(bool absoluteCoordinate = true)
        {
            Queue<MessagePacket> commands = new Queue<MessagePacket>();

            // Pause Gantry
            commands.Enqueue(new MessagePacket((byte)COMM_BYTE.GAN_PAUSE));
            // Update DC coordinate absolute
            commands.Enqueue(new MessagePacket(DC_Direction(), DCLocation()));
            // Update Stepper coordiante absolute
            commands.Enqueue(new MessagePacket(Stepper_Direction(), StepperLocation()));
            // Update Stepper speed
            commands.Enqueue(new MessagePacket((byte)COMM_BYTE.GAN_SET_DELAY_STP, StepperDelayFromSpeed()));
            // Update DC speed
            commands.Enqueue(new MessagePacket((byte)COMM_BYTE.GAN_SET_MAX_PWM_DC, DCPWMFromSpeed()));
            // Resume Gantry
            commands.Enqueue(new MessagePacket((byte)COMM_BYTE.GAN_RESUME));
            return commands;
        }

        private byte DC_Direction()
        {
            if (X >= 0 && !absoluteCoordinate)
            {
                return (byte)COMM_BYTE.GAN_DELTA_POS_DC;
            }
            if (X < 0 && !absoluteCoordinate)
            {
                return (byte)COMM_BYTE.GAN_DELTA_NEG_DC;
            }
            if (X >= 0 && absoluteCoordinate)
            {
                return (byte)COMM_BYTE.GAN_ABS_POS_DC;
            }
            return (byte)COMM_BYTE.GAN_ABS_NEG_DC;
        }
        private byte Stepper_Direction()
        {
            if (Y >= 0 && !absoluteCoordinate)
            {
                return (byte)COMM_BYTE.GAN_DELTA_POS_STP;
            }
            if (Y < 0 && !absoluteCoordinate)
            {
                return (byte)COMM_BYTE.GAN_DELTA_NEG_STP;
            }
            if (Y >= 0 && absoluteCoordinate)
            {
                return (byte)COMM_BYTE.GAN_ABS_POS_STP;
            }
            return (byte)COMM_BYTE.GAN_ABS_NEG_STP;
        }
        private ushort DCLocation()
        {
            // TODO: implement this better
            float CM_TO_COUNTS = 300f;
            return (ushort)(Math.Abs(X) * CM_TO_COUNTS);
        }
        private ushort StepperLocation()
        {
            // TODO: implement this better
            float CM_TO_STEPS = 1000f;
            return (ushort)(Math.Abs(Y) * CM_TO_STEPS);
        }

        private ushort StepperDelayFromSpeed()
        {
            // Speed = 100, return 100
            // Speed = 10, return 1000
            return (ushort)(10000 / Speed);
        }

        private ushort DCPWMFromSpeed()
        {
            // TODO: implement this better
            return 16000;
        }

        public override string ToString()
        {
            return $"({X:F2}cm, {Y:F2}cm, {Speed}%)";
        }

    }
}
