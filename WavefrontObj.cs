using System;
using System.Collections.Generic;
using System.Text;

namespace MC2FBX
{
    class WavefrontObj
    {
        readonly StringBuilder sb = new StringBuilder();
        readonly List<CoordinateDecimal> verticies = new List<CoordinateDecimal>();

        public WavefrontObj(Dictionary<BlockType, List<FacedVolume>> facedVolumizedWorld)
        {
            foreach (KeyValuePair<BlockType, List<FacedVolume>> pair in facedVolumizedWorld)
            {



            }
        }

        private void AppendVerticies(List<FacedVolume> volumes)
        {
            for (int idx = 0; idx < volumes.Count; idx++)
            {
                Volume volume = volumes[idx].volume;
                verticies.Add(new CoordinateDecimal(
                    volume.Coord.X, volume.Coord.Y, volume.Coord.Z));
            }
        }
    }
}
