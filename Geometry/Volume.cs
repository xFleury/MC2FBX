using System;
using System.Collections;
using System.Collections.Generic;

namespace MC2FBX
{
    public struct Volume : IEquatable<Volume>, IEnumerable<Coordinate>
    {
        public readonly Coordinate Coord;
        /// <summary>sX</summary>
        public readonly int Width;
        /// <summary>sY</summary>
        public readonly int Height;
        /// <summary>sZ</summary>
        public readonly int Length;

        public Volume(Coordinate coord, int width, int height, int length)
        {
            Coord = coord;
            Width = width;
            Height = height;
            Length = length;
        }

        public IEnumerator<Coordinate> GetEnumerator()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    for (int z = 0; z < Length; z++)
                        yield return Coord.Offset(x, y, z);
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public int TotalVolume { get { return Width * Height * Length; } }

        public bool Equals(Volume spawn)
        {
            return
                Coord == spawn.Coord &&
                Width == spawn.Width &&
                Height == spawn.Height &&
                Length == spawn.Length;
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
            hash = hash * 37 + Width;
            hash = hash * 37 + Height;
            hash = hash * 37 + Length;
            return hash;
        }

        public static bool operator ==(Volume k1, Volume k2)
        {
            return
                k1.Coord == k2.Coord &&
                k1.Width == k2.Width &&
                k1.Height == k2.Height &&
                k1.Length == k2.Length;
        }

        public static bool operator !=(Volume k1, Volume k2)
        {
            return
                k1.Coord != k2.Coord ||
                k1.Width != k2.Width ||
                k1.Height != k2.Height ||
                k1.Length != k2.Length;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}x{2}x{3})",
                Coord.ToString(), Width, Height, Length);
        }
    }
}
