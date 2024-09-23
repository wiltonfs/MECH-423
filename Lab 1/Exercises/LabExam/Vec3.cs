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
        public long timestamp { get; set; }

        public Vec3(float x, float y, float z, long ts)
        {
            X = x; Y = y; Z = z; timestamp = ts;
        }
        public Vec3(float x, float y, float z)
        {
            X = x; Y = y; Z = z;
        }
        public Vec3(float val)
        {
            X = val; Y = val; Z = val;
        }
        public Vec3()
        {
            X = 0; Y = 0; Z = 0;
        }

        public float Length() { return (float)Math.Sqrt(X * X + Y * Y + Z * Z); }

        public float AbsX() { return (float)Math.Abs(X);}
        public float AbsY() { return (float)Math.Abs(Y); }
        public float AbsZ() { return (float)Math.Abs(Z); }

        public string UpAxis() 
        {
            float threshold = 5f;

            if (Length() < threshold) 
            {
                return "Freefall";
            }

            if (AbsX() > threshold && AbsX() > Math.Max(AbsY(), AbsZ()))
                return "X";
            else if (AbsY() > threshold && AbsY() > Math.Max(AbsX(), AbsZ()))
                return "Y";
            else
                return "Z";
        }

        public int UpAxisSign()
        {
            switch (UpAxis())
            {
                case "X":
                    return (int)(X / AbsX());
                case "Y":
                    return (int)(Y / AbsY());
                case "Z":
                    return (int)(Z / AbsZ());
                default:
                    return 0;
            }
        }

        override public string ToString()
        {
            return "{" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + "}";
        }

        public string ToStringWithTimestamp()
        {
            return X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ", " + timestamp.ToString();
        }
    }
}
