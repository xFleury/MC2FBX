using System;

namespace NbtToObj.Geometry
{
    public struct SizeD : IEquatable<SizeD>
    {
        public readonly decimal Width;
        public readonly decimal Height;

        public SizeD(decimal width, decimal height)
        {
            Width = width;
            Height = height;
        }

        public bool Equals(SizeD other)
        {
            return Width == other.Width && Height == other.Height;
        }

        public override bool Equals(object other)
        {
            if (other is SizeD)
            {
                return this == (SizeD)other;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 37 + (int)Width;
            hash = hash * 37 + (int)Height;
            return hash;
        }

        public static bool operator ==(SizeD k1, SizeD k2)
        {
            return k1.Width == k2.Width && k1.Height == k2.Height;
        }

        public static bool operator !=(SizeD k1, SizeD k2)
        {
            return k1.Width != k2.Width || k1.Height != k2.Height;
        }

        public override string ToString()
        {
            return "(" + Width + ", " + Height + ")";
        }
    }
}
