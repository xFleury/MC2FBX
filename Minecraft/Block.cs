using System;

namespace MC2UE
{    
    struct Block : IEquatable<Block>
    {
        private static BlockIdentifier[] nonOpaqueBlocks = new BlockIdentifier[] {
            BlockIdentifier.Air, BlockIdentifier.Leaves, BlockIdentifier.Glass};

        public BlockIdentifier id;
        public byte data;

        public bool IsOpaque { get { return Array.IndexOf(nonOpaqueBlocks, id) == -1; } }

        public Block(BlockIdentifier id, byte data)
        {
            this.id = id; this.data = data;
        }

        public BlockFaceTexture GetFaceTexture(Face face)
        {
            switch (id)
            {
                case BlockIdentifier.Dirt: return BlockFaceTexture.Dirt;
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

        public bool Equals(Block other)
        {
            return id == other.id && data == other.data;
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
            return ((int)id << 8) | data;
        }

        public override string ToString()
        {
            switch (id)
            {
                /* BlockIdentifier.Air.ToString() is reflection based, and would not survive obfuscation. */
                case BlockIdentifier.Dirt: return "dirt";
                case BlockIdentifier.StoneBrick: return "stonebrick";
                case BlockIdentifier.BrickBlock: return "brick";
                case BlockIdentifier.WoodPlank: return "plank";
                case BlockIdentifier.Leaves: return "leaves";
                case BlockIdentifier.Grass: return "grass";
                default: throw new Exception("Unknown BlockIdentifier name.");
            }
        }
    }
}
