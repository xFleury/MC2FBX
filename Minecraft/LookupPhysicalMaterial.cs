namespace NbtToObj.Minecraft
{
    static class LookupPhysicalMaterial
    {
        public static PhysicalMaterial FromBlockIdentifier(BlockIdentifier blockIdentifier)
        {
            switch (blockIdentifier)
            {
                case BlockIdentifier.Wood:
                case BlockIdentifier.WoodStairs:
                case BlockIdentifier.WoodSlab:
                case BlockIdentifier.WoodPlank:
                    return PhysicalMaterial.WoodMaterial;

                case BlockIdentifier.Cobblestone:
                case BlockIdentifier.CobblestoneStairs:
                case BlockIdentifier.CobblestoneWall:
                case BlockIdentifier.Stone:
                case BlockIdentifier.StoneBrick:
                case BlockIdentifier.StoneBrickStairs:
                case BlockIdentifier.StonePlate:
                case BlockIdentifier.StoneSlab:
                    return PhysicalMaterial.StoneMaterial;

                default: return PhysicalMaterial.DirtMaterial;
            }

        }
    }
}
