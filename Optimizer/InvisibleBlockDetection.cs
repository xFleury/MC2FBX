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

        public static Dictionary<CoordinateInt, Block> DetectAndFilterInvisible(Dictionary<CoordinateInt, Block> blocks)
        {
            MapPartition.invisibleBricks = new HashSet<CoordinateInt>();

            foreach (KeyValuePair<CoordinateInt, Block> pair in blocks)
                if (IsInvisible(pair.Key, blocks))
                    MapPartition.invisibleBricks.Add(pair.Key);

            Dictionary<CoordinateInt, Block> visibleBlocks = new Dictionary<CoordinateInt, Block>(blocks);

            foreach (CoordinateInt coord in MapPartition.invisibleBricks)
                visibleBlocks.Remove(coord);

            return visibleBlocks;
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
