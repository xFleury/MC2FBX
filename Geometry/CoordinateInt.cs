using System;

namespace NbtToObj.Geometry
{
    public struct CoordinateInt : IEquatable<CoordinateInt>
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        public CoordinateInt(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public CoordinateInt Offset(int dx, int dy, int dz)
        {
            return new CoordinateInt(X + dx, Y + dy, Z + dz);
        }

        public bool Equals(CoordinateInt spawn)
        {
            return X == spawn.X && Y == spawn.Y && Z == spawn.Z;
        }

        public override bool Equals(object o)
        {
            if (o is CoordinateInt)
            {
                return this == (CoordinateInt)o;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 37 + X;
            hash = hash * 37 + Y;
            hash = hash * 37 + Z;
            return hash;
        }

        public static explicit operator CoordinateDecimal (CoordinateInt coord)
        {
            return new CoordinateDecimal(coord.X, coord.Y, coord.Z);
        }

        public static bool operator ==(CoordinateInt k1, CoordinateInt k2)
        {
            return k1.X == k2.X && k1.Y == k2.Y && k1.Z == k2.Z;
        }

        public static bool operator !=(CoordinateInt k1, CoordinateInt k2)
        {
            return k1.X != k2.X || k1.Y != k2.Y || k1.Z != k2.Z;
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ", " + Z + ")";
        }
    }
}
