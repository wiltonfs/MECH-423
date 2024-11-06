﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exercise6
{
    public class GantryCoordinate
    {
        public const int X_WIDTH_CM = 6;
        public const int Y_WIDTH_CM = 6;

        public const float DC_REVS_PER_CM = 1 / 3;      // 1 revolution = 3 cm
        public const float COUNTS_PER_REV = 244;        // 244 counts = 1 revolution
        public const float STP_REVS_PER_CM = 1 / 4;     // 1 revolution = 4 cm
        public const float HALFSTEPS_PER_REV = 2000;    // 2000 halfsteps = 1 revolution
        public const float PWM_PER_SPEED = 65000 / 5;   // 65000 PWM = 5 Hz

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
            float counts = Math.Abs(X) * DC_REVS_PER_CM * COUNTS_PER_REV;
            return (ushort)(counts);
        }
        private ushort StepperLocation()
        {
            float steps = Math.Abs(Y) * STP_REVS_PER_CM * HALFSTEPS_PER_REV;
            return (ushort)(steps);
        }

        private ushort StepperDelayFromSpeed()
        {
            // Speed = 100, return 100 =  1ms
            // Speed = 10, return 1000 = 10ms
            return (ushort)(10000 / Speed);
        }

        private ushort DCPWMFromSpeed()
        {
            if (Y == 0)
            {
                return (ushort)(650*Speed);
            }
            float CM_PER_HALFSTEP = 1 / (HALFSTEPS_PER_REV * STP_REVS_PER_CM);
            float S = (CM_PER_HALFSTEP) / (StepperDelayFromSpeed() / 100000); // cm / s
            float D = S * X / Y; // cm /s
            D = D * DC_REVS_PER_CM; // Hz

            ushort PWM = (ushort)Math.Abs(PWM_PER_SPEED * D);
            return PWM;
        }

        public override string ToString()
        {
            return $"({X:F2}cm, {Y:F2}cm, {Speed}%)";
        }

    }
}
