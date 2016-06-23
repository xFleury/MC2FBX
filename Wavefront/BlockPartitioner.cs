using NbtToObj.Geometry;
using NbtToObj.Gui;
using NbtToObj.Minecraft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtToObj.Wavefront
{
    static class BlockPartitioner
    {
        public static void Organize(WorldState worldState)
        {
            worldState.organizedBlocks = new Dictionary<Block, HashSet<CoordinateInt>>();
            foreach (KeyValuePair<CoordinateInt, Block> pair in worldState.visibleBlocks)
            {
                HashSet<CoordinateInt> coordinates;
                if (worldState.organizedBlocks.TryGetValue(pair.Value, out coordinates))
                    coordinates.Add(pair.Key);
                else
                {
                    coordinates = new HashSet<CoordinateInt>();
                    coordinates.Add(pair.Key);
                    worldState.organizedBlocks.Add(pair.Value, coordinates);
                }
            }
        }
    }
}
