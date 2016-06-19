using NbtToObj.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NbtToObj.Minecraft
{
    static class CompositeBlockConverter
    {
        private static CoordinateInt[] offsets = new CoordinateInt[] {
            new CoordinateInt(0,0,0),
            new CoordinateInt(1,0,0),
            new CoordinateInt(0,1,0),
            new CoordinateInt(1,1,0),
            new CoordinateInt(0,0,1),
            new CoordinateInt(1,0,1),
            new CoordinateInt(0,1,1),
            new CoordinateInt(1,1,1) };

        private static int[] prefabFullBlock = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

        private static int[] prefabInnerCorner = new int[] { 0, 1, 2, 3, 4, 6, 7 };

        private static int[] prefabOuterCorner = new int[] {0, 1, 2, 3, 5 };

        private static int[] prefabStaircase = new int[] { 0, 1, 2, 3, 6, 7 };

        private static int[] rotate90 = new int[] { 1, 3, 0, 2, 5, 7, 4, 6 };

        private static int[] rotate180 = new int[] { 3, 2, 1, 0, 7, 6, 5, 4 };

        private static int[] rotate270 = new int[] { 2, 0, 3, 1, 6, 4, 7, 5 };

        public static Dictionary<CoordinateInt, Block> Convert(Dictionary<CoordinateInt, CompositeBlock> blocks)
        {
            Dictionary<CoordinateInt, Block> convertedBlocks = new Dictionary<CoordinateInt, Block>();

            foreach (KeyValuePair<CoordinateInt, CompositeBlock> block in blocks)
            {
                int rotation = block.Value.data & 3;

                BlockIdentifier id = block.Value.id;

                if (id == BlockIdentifier.CobblestoneStairs)
                {
                    BlockState state = new BlockState(blocks, block);
                    if (state.hasDiagonals)
                    {
                        BuildBlocks(convertedBlocks, BlockIdentifier.Cobblestone, block.Key, state.isCornerstone ? prefabOuterCorner : prefabInnerCorner, state.rotation);
                    }
                    else
                    {
                        BuildBlocks(convertedBlocks, BlockIdentifier.Cobblestone, block.Key, prefabStaircase, rotation);
                    }
                }
                else
                {
                    BuildBlocks(convertedBlocks, block.Value.id, block.Key, prefabFullBlock, rotation);
                }
            }

            return convertedBlocks;
        }

        private static void BuildBlocks(Dictionary<CoordinateInt, Block> convertedBlocks,
            BlockIdentifier identifier, CoordinateInt location, int[] indexes, int rotation)
        {
            foreach (CoordinateInt offset in OffsetsFromPrefabRotated(indexes, rotation))
            {
                CoordinateInt newLocation = new CoordinateInt(
                    location.X * 2 + offset.X,
                    location.Y * 2 + offset.Y,
                    location.Z * 2 + offset.Z);
                convertedBlocks.Add(newLocation, new Block(identifier));
            }
        }

        private static IEnumerable<CoordinateInt> OffsetsFromPrefabRotated(int[] indexes, int rotation)
        {
            switch (rotation)
            {
                case 3:
                foreach (int idx in indexes)
                    yield return offsets[rotate180[idx]];
                break;
                case 2:
                foreach (int idx in indexes)
                    yield return offsets[idx];
                break;
                case 0:
                foreach (int idx in indexes)
                    yield return offsets[rotate270[idx]];
                break;
                default:
                foreach (int idx in indexes)
                    yield return offsets[rotate90[idx]];
                break;
            }
        }

    }
}
