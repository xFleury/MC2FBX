using System;

namespace MC2UE.Geometry
{
    public struct CoordinateDecimal : IEquatable<CoordinateDecimal>
    {
        public readonly decimal X;
        public readonly decimal Y;
        public readonly decimal Z;

        public CoordinateDecimal(decimal x, decimal y, decimal z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public CoordinateDecimal Offset(decimal dx, decimal dy, decimal dz)
        {
            return new CoordinateDecimal(X + dx, Y + dy, Z + dz);
        }

        public bool Equals(CoordinateDecimal spawn)
        {
            return X == spawn.X && Y == spawn.Y && Z == spawn.Z;
        }

        public override bool Equals(object o)
        {
            if (o is CoordinateDecimal)
            {
                return this == (CoordinateDecimal)o;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 37 + (int)X;
            hash = hash * 37 + (int)Y;
            hash = hash * 37 + (int)Z;
            return hash;
        }

        public static bool operator ==(CoordinateDecimal k1, CoordinateDecimal k2)
        {
            return k1.X == k2.X && k1.Y == k2.Y && k1.Z == k2.Z;
        }

        public static bool operator !=(CoordinateDecimal k1, CoordinateDecimal k2)
        {
            return k1.X != k2.X || k1.Y != k2.Y || k1.Z != k2.Z;
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ", " + Z + ")";
        }
    }
}
