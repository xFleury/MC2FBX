using System;

namespace NbtToObj.Geometry
{
    public struct TextureCoordinate : IEquatable<TextureCoordinate>
    {
        public readonly decimal U;
        public readonly decimal V;       

        public TextureCoordinate(decimal u, decimal v)
        {
            U = u;
            V = v;
        }

        public TextureCoordinate Offset(decimal du, decimal dv)
        {
            return new TextureCoordinate(U + du, V + dv);
        }

        public bool Equals(TextureCoordinate spawn)
        {
            return U == spawn.U && V == spawn.V;
        }

        public override bool Equals(object o)
        {
            if (o is TextureCoordinate)
            {
                return this == (TextureCoordinate)o;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 37 + (int)U;
            hash = hash * 37 + (int)V;
            return hash;
        }

        public static bool operator ==(TextureCoordinate k1, TextureCoordinate k2)
        {
            return k1.U == k2.U && k1.V == k2.V;
        }

        public static bool operator !=(TextureCoordinate k1, TextureCoordinate k2)
        {
            return k1.U != k2.U || k1.V != k2.V;
        }

        public override string ToString()
        {
            return "(" + U + ", " + V + ")";
        }
    }
}
