namespace NbtToObj.Minecraft
{
    static class LookupPhysicalMaterial
    {
        public static PhysicalMaterial FromBlockIdentifier(BlockIdentifier blockIdentifier)
        {
            switch (blockIdentifier)
            {
                case BlockIdentifier.Cobblestone:
                case BlockIdentifier.CobblestoneStairs:
                case BlockIdentifier.CobblestoneWall:
                case BlockIdentifier.Stone:
                case BlockIdentifier.StoneBrick:
                case BlockIdentifier.StoneBrickStairs:
                case BlockIdentifier.StonePlate:
                case BlockIdentifier.StoneSlab:
                    return PhysicalMaterial.Stone;

                default: return PhysicalMaterial.Dirt;
            }

        }
    }
}
