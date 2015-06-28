using System;

namespace MC2Blender
{
    /// <summary>Immutable coordinate.</summary>
    public struct Coordinate : IEquatable<Coordinate>
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        public Coordinate(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Coordinate Offset(int dx, int dy, int dz)
        {
            return new Coordinate(X + dx, Y + dy, Z + dz);
        }

        public bool Equals(Coordinate spawn)
        {
            return X == spawn.X && Y == spawn.Y && Z == spawn.Z;
        }

        public override bool Equals(Object o)
        {
            if (o is Coordinate)
            {
                return this == (Coordinate)o;
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

        public static bool operator ==(Coordinate k1, Coordinate k2)
        {
            return k1.X == k2.X && k1.Y == k2.Y && k1.Z == k2.Z;
        }

        public static bool operator !=(Coordinate k1, Coordinate k2)
        {
            return k1.X != k2.X || k1.Y != k2.Y || k1.Z != k2.Z;
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ", " + Z + ")";
        }
    }
}
