using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exercise6
{
    public class MessagePacket
    {
        public byte comm = (byte)COMM_BYTE.DEBUG_ECHO_REQUEST;
        public byte d1 = 0;
        public byte d2 = 0;
        public byte esc = 0;
        public ushort combined = 0;

        public MessagePacket()
        { }
        public MessagePacket(byte c, byte data1, byte data2)
        {
            comm = c; 
            d1 = data1;
            d2 = data2;
            HandleEscapeByte();
            CombineDataBytes();
        }
        public MessagePacket(byte c, ushort data)
        {
            comm = c;
            combined = data;
            SeparateDataBytes();
            HandleEscapeByte();
        }
        public MessagePacket(byte c)
        {
            comm = c;
        }
        public override string ToString()
        {
            return $"[{(COMM_BYTE)comm}, {combined}]";
        }
        public string ToStringRaw()
        {
            return $"[255 {comm} {d1} {d2} {esc}]";
        }

        public void HandleEscapeByte()
        {
            if (comm > 254)
            {
                comm = 254; esc += 1;
            }
            if (d1 > 254)
            {
                d1 = 254; esc += 2;
            }
            if (d2 > 254)
            {
                d2 = 254; esc += 4;
            }
        }

        public void ApplyEscapeByte()
        {
            if ((esc & 0x1) > 0)
                comm = 255;
            if ((esc & 0x2) > 0)
                d1 = 255;
            if ((esc & 0x4) > 0)
                d2 = 255;
        }

        public void CombineDataBytes()
        {
            combined = (ushort)(((ushort)d1 << 8) | (ushort)d2);
        }

        public void SeparateDataBytes()
        {
            d1 = (byte)((combined >> 8) & 0xFF);
            d2 = (byte)(combined & 0xFF);
        }
    }
}
