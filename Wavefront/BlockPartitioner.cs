using NbtToObj.Geometry;
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
        public static Dictionary<Block, HashSet<CoordinateInt>> Organize(Dictionary<CoordinateInt, Block> blocks)
        {
            Dictionary<Block, HashSet<CoordinateInt>> organizedWorld = new Dictionary<Block, HashSet<CoordinateInt>>();
            foreach (KeyValuePair<CoordinateInt, Block> pair in blocks)
            {
                HashSet<CoordinateInt> coordinates;
                if (organizedWorld.TryGetValue(pair.Value, out coordinates))
                    coordinates.Add(pair.Key);
                else
                {
                    coordinates = new HashSet<CoordinateInt>();
                    coordinates.Add(pair.Key);
                    organizedWorld.Add(pair.Value, coordinates);
                }
            }
            return organizedWorld;
        }
    }
}
