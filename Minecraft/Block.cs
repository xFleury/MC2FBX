using System;
using NbtToObj.Geometry;

namespace NbtToObj.Minecraft
{
    struct Block : IEquatable<Block>
    {
        private static BlockIdentifier[] nonOpaqueBlocks = new BlockIdentifier[] {
            BlockIdentifier.Air, BlockIdentifier.Glass};

        public BlockIdentifier id;

        public bool IsOpaque { get { return Array.IndexOf(nonOpaqueBlocks, id) == -1; } }

        public Block(BlockIdentifier id)
        {
            this.id = id;
        }

        public BlockFaceTexture GetFaceTexture(Face face)
        {
            switch (id)
            {
                case BlockIdentifier.Dirt: return BlockFaceTexture.Dirt;
                case BlockIdentifier.Wood: return BlockFaceTexture.Wood;
                case BlockIdentifier.Leaves: return BlockFaceTexture.Leaves;


                case BlockIdentifier.CobblestoneStairs:
                case BlockIdentifier.Cobblestone:
                    return BlockFaceTexture.Cobblestone;
                
                case BlockIdentifier.WoodPlank: return BlockFaceTexture.WoodPlank;

                case BlockIdentifier.StoneBrickStairs:
                case BlockIdentifier.StoneBrick:
                    return BlockFaceTexture.StoneBrick;
                case BlockIdentifier.Grass:
                    switch (face)
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

        public bool Equals(Block other)
        {
            return id == other.id;
        }

        public override bool Equals(object obj)
        {
            if (obj is Block)
                return Equals((Block)obj);
            else return false;
        }

        public static bool operator ==(Block a, Block b) { return a.Equals(b); }

        public static bool operator !=(Block a, Block b) { return !a.Equals(b); }

        public override int GetHashCode()
        {
            return (int)id;
        }

        public override string ToString()
        {
            return GetFaceTexture(Face.PositiveZ).ToString();
        }
    }
}
