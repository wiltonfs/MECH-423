using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exercise4
{
    public class Vec3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vec3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float Length() { return (float)Math.Sqrt(X * X + Y * Y + Z * Z); }

        override public string ToString() { return "{" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + "}";
    }
}
