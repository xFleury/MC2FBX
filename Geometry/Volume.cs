using System;
using System.Collections;
using System.Collections.Generic;

namespace MC2UE.Geometry
{
    public struct Volume : IEquatable<Volume>
    {
        public readonly CoordinateInt Coord;
        public readonly int ScaleX;
        public readonly int ScaleY;
        public readonly int ScaleZ;

        public Volume(CoordinateInt coord, int scaleX, int scaleY, int scaleZ)
        {
            Coord = coord;
            ScaleX = scaleX;
            ScaleY = scaleY;
            ScaleZ = scaleZ;
        }

        public int TotalVolume { get { return ScaleX * ScaleY * ScaleZ; } }

        public bool Equals(Volume spawn)
        {
            return
                Coord == spawn.Coord &&
                ScaleX == spawn.ScaleX &&
                ScaleY == spawn.ScaleY &&
                ScaleZ == spawn.ScaleZ;
        }

        public override bool Equals(Object o)
        {
            if (o is Volume)
                return this == (Volume)o;
            else
                return false;
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 37 + Coord.GetHashCode();
            hash = hash * 37 + ScaleX;
            hash = hash * 37 + ScaleY;
            hash = hash * 37 + ScaleZ;
            return hash;
        }

        public static bool operator ==(Volume k1, Volume k2)
        {
            return
                k1.Coord == k2.Coord &&
                k1.ScaleX == k2.ScaleX &&
                k1.ScaleY == k2.ScaleY &&
                k1.ScaleZ == k2.ScaleZ;
        }

        public static bool operator !=(Volume k1, Volume k2)
        {
            return
                k1.Coord != k2.Coord ||
                k1.ScaleX != k2.ScaleX ||
                k1.ScaleY != k2.ScaleY ||
                k1.ScaleZ != k2.ScaleZ;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}x{2}x{3})",
                Coord.ToString(), ScaleX, ScaleY, ScaleZ);
        }
    }
}
