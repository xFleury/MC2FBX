using NbtToObj.Geometry;
using NbtToObj.Helpers;
using NbtToObj.Minecraft;
using NbtToObj.Wavefront;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtToObj.Gui
{
    class ChunkState
    {
        public string chunkName;

        public Block blockType;

        public HashSet<CoordinateInt> visibleBlocks;

        public readonly List<Volume> volumizedWorld = new List<Volume>();

        public List<FacedVolume> facedVolumes;

        //public List<FacedVolume> facedVolumes;
    }
}
