using System;
using NbtToObj.Geometry;

namespace NbtToObj.Minecraft
{    
    /// <summary>
    /// Minecraft blocks are made up of several smaller blocks; a staircase consists of 6 (of 8) sub-blocks.
    /// </summary>
    struct CompositeBlock : IEquatable<CompositeBlock>
    {
        public BlockIdentifier id;
        public byte data;

        public bool IsOpaque { get { return LookupOpaqueBlock.IsOpaque(id); } }

        public bool IsHalfBlock
        {
            get
            {
                return id == BlockIdentifier.StoneSlab;
            }
        }

        public CompositeBlock(BlockIdentifier id, byte data)
        {
            this.id = id;
            this.data = data;
        }

        public BlockFaceTexture GetFaceTexture(Face face)
        {
            switch (id)
            {
                case BlockIdentifier.Dirt: return BlockFaceTexture.Dirt;
                case BlockIdentifier.Wood: return BlockFaceTexture.Wood;
                case BlockIdentifier.Leaves: return BlockFaceTexture.Leaves;
                case BlockIdentifier.Cobblestone: return BlockFaceTexture.Cobblestone;
                case BlockIdentifier.WoodPlank: return BlockFaceTexture.WoodPlank;
                case BlockIdentifier.StoneBrick: return BlockFaceTexture.StoneBrick;
                case BlockIdentifier.Grass:
                    switch(face)
                    {
                        case Face.PositiveX:
                        case Face.NegativeX:
                        case Face.PositiveY:
                        case Face.NegativeY:
                            return BlockFaceTexture.Grass_Side;
                        case Face.NegativeZ:
                            return BlockFaceTexture.Dirt;
                        case Face.PositiveZ:
                            return BlockFaceTexture.Grass_Top;
                        default: throw new Exception("Unknown texture face.");
                    }
                default: throw new Exception("Unknown block identifier.");
            }
        }

        public bool Equals(CompositeBlock other)
        {
            return id == other.id && data == other.data;
        }

        public override bool Equals(object obj)
        {
            if (obj is CompositeBlock)
                return Equals((CompositeBlock)obj);
            else return false;
        }

        public static bool operator ==(CompositeBlock a, CompositeBlock b) { return a.Equals(b); }

        public static bool operator !=(CompositeBlock a, CompositeBlock b) { return !a.Equals(b); }

        public override int GetHashCode()
        {
            return ((int)id << 8) | data;
        }

        public override string ToString()
        {
            return GetFaceTexture(Face.PositiveZ).ToString();
        }
    }
}
