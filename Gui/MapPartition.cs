using NbtToObj.Geometry;
using NbtToObj.Minecraft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtToObj.Gui
{
    class MapPartition
    {
        public string outputName;

        public PhysicalMaterial physicalMaterial;

        public Dictionary<CoordinateInt, Block> rawBlocks;

        public Dictionary<Block, HashSet<CoordinateInt>> organizedBlocks;

        public static HashSet<CoordinateInt> invisibleBricks;
    }
}
