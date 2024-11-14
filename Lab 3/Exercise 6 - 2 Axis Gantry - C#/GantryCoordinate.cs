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
        public const int X_WIDTH_CM = 7;
        public const int Y_WIDTH_CM = 7;

        public const float DRAWING_SCALE = 0.5f;

        public const float DC_REVS_PER_CM = 0.25f;      // 1 revolution = 4 cm
        public const float COUNTS_PER_REV = 245f;        // 245 counts = 1 revolution
        public const float STP_REVS_PER_CM = 0.25f;     // 1 revolution = 4 cm
        public const float HALFSTEPS_PER_REV = 400f;     // 400 halfsteps = 1 revolution

        public float X = 0;
        public float Y = 0;
        public uint Speed = 100;

        public GantryCoordinate(float x=0f, float y=0f, uint s=100)
        {
            X = Clamp(x, -X_WIDTH_CM, X_WIDTH_CM);
            Y = Clamp(y, -Y_WIDTH_CM, Y_WIDTH_CM);
            Speed = Clamp(s, 10, 100);
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
            if (X >= 0)
            {
                return (byte)COMM_BYTE.GAN_ABS_POS_DC;
            }
            return (byte)COMM_BYTE.GAN_ABS_NEG_DC;
        }
        private byte Stepper_Direction()
        {
            if (Y >= 0)
            {
                return (byte)COMM_BYTE.GAN_ABS_POS_STP;
            }
            return (byte)COMM_BYTE.GAN_ABS_NEG_STP;
        }
        private ushort DCLocation()
        {
            float counts = Math.Abs(X) * DC_REVS_PER_CM * COUNTS_PER_REV * DRAWING_SCALE;
            return (ushort)(counts);
        }
        private ushort StepperLocation()
        { 
            float steps = Math.Abs(Y) * STP_REVS_PER_CM * HALFSTEPS_PER_REV * DRAWING_SCALE;
            return (ushort)(steps);
        }

        private ushort StepperDelayFromSpeed()
        {
            //return 10;
            // Speed = 100, return 10 =  1ms
            // Speed = 10, return 100 = 10ms
            return (ushort)(1000 / Speed);
        }

        private ushort DCPWMFromSpeed()
        {
            ushort pwm = (ushort)(32000f * (Speed / 100f));
            if (pwm < 10000) pwm = 10000;
            return pwm;
        }

        public override string ToString()
        {
            return $"({X:F2}cm, {Y:F2}cm, {Speed}%)";
        }

    }
}
