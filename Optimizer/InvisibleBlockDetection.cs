using NbtToObj.Geometry;
using NbtToObj.Gui;
using NbtToObj.Minecraft;
using System;
using System.Collections.Generic;

namespace NbtToObj.Optimizer
{
    static class InvisibleBlockDetection
    {
        private static BlockIdentifier[] transparentBlocks = new BlockIdentifier[] { BlockIdentifier.Leaves };

        public static void DetectAndFilterInvisible(WorldState worldState)
        {
            worldState.invisibleBricks = new HashSet<CoordinateInt>();

            foreach (KeyValuePair<CoordinateInt, Block> pair in worldState.visibleAndInvisibleBlocks)
                if (IsInvisible(pair.Key, worldState.visibleAndInvisibleBlocks))
                    worldState.invisibleBricks.Add(pair.Key);

            worldState.visibleBlocks = new Dictionary<CoordinateInt, Block>(worldState.visibleAndInvisibleBlocks);

            foreach (CoordinateInt coord in worldState.invisibleBricks)
                worldState.visibleBlocks.Remove(coord);
        }


        private static bool IsInvisible(CoordinateInt coord, Dictionary<CoordinateInt, Block> rawWorld)
        {
            bool isInvisible =
                OpaqueBrickAt(coord.Offset(-1, 0, 0), rawWorld) &&
                OpaqueBrickAt(coord.Offset(1, 0, 0), rawWorld) &&
                OpaqueBrickAt(coord.Offset(0, -1, 0), rawWorld) &&
                OpaqueBrickAt(coord.Offset(0, 1, 0), rawWorld) &&
                OpaqueBrickAt(coord.Offset(0, 0, -1), rawWorld) &&
                OpaqueBrickAt(coord.Offset(0, 0, 1), rawWorld);
            return isInvisible;
        }

        private static bool OpaqueBrickAt(CoordinateInt coord, Dictionary<CoordinateInt, Block> rawWorld)
        {
            Block blockType;
            return (rawWorld.TryGetValue(coord, out blockType)) && Array.IndexOf(transparentBlocks, blockType.id) == -1;
        }
    }
}
