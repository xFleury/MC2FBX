using NbtToObj.Geometry;
using NbtToObj.Minecraft;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using NbtToObj.Helpers;

namespace NbtToObj.Gui
{
    static class ChunkStateBuilder
    {
        const int chunkSize = 16;

        public static void Partition(WorldState worldState)
        {
            

            Dictionary<CoordinateInt, Dictionary<CoordinateInt, Block>> chunks =
                new Dictionary<CoordinateInt, Dictionary<CoordinateInt, Block>>();

            foreach (KeyValuePair<CoordinateInt, Block> block in worldState.visibleBlocks)
            {
                Debug.Assert(block.Key.X >= 0);
                Debug.Assert(block.Key.Y >= 0);
                Debug.Assert(block.Key.Z >= 0);
                CoordinateInt key = new CoordinateInt(
                    block.Key.X / chunkSize,
                    block.Key.Y / chunkSize,
                    block.Key.Z / chunkSize);

                Dictionary<CoordinateInt, Block> blocks;
                if (chunks.TryGetValue(key, out blocks))
                {
                    blocks.Add(block.Key, block.Value);
                }
                else
                {
                    blocks = new Dictionary<CoordinateInt, Block>();
                    blocks.Add(block.Key, block.Value);
                    chunks.Add(key, blocks);
                }
            }

            worldState.chunks = Organize(chunks);

            Console.WriteLine(worldState.chunks + " static meshes.");
        }

        private static List<ChunkState> Organize(Dictionary<CoordinateInt, Dictionary<CoordinateInt, Block>> chunks)
        {
            int groupNum = 1;
            List<ChunkState> chunkStates = new List<ChunkState>();

            foreach (KeyValuePair<CoordinateInt, Dictionary<CoordinateInt,Block>> pair in chunks)
            {
                MultiValueDictionary<Block, CoordinateInt> organized = new MultiValueDictionary<Block, CoordinateInt>();
                foreach (KeyValuePair<CoordinateInt, Block> block in pair.Value)
                {
                    organized.Add(block.Value, block.Key);
                }
                foreach (KeyValuePair<Block, List<CoordinateInt>> chunk in organized)
                {
                    chunkStates.Add(new ChunkState()
                    {
                        blockType = chunk.Key,
                        chunkName = $"G{++groupNum}",
                        visibleBlocks = new HashSet<CoordinateInt>(chunk.Value)
                    });
                }
            }

            return chunkStates;
        }
    }
}
