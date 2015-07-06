using System;
using System.Collections.Generic;
using System.Text;

namespace MC2FBX
{
    class WavefrontObj
    {
        private readonly StringBuilder sb = new StringBuilder();
        private readonly List<CoordinateDecimal> verticies = new List<CoordinateDecimal>();
        private readonly Dictionary<string, FaceVerticies[]> collisionBoxes = new Dictionary<string, FaceVerticies[]>();
        private readonly Dictionary<BlockType, List<TexturedFace>> texturedFaces = new Dictionary<BlockType, List<TexturedFace>>();

        public WavefrontObj(Dictionary<BlockType, List<FacedVolume>> facedVolumizedWorld)
        {
            foreach (KeyValuePair<BlockType, List<FacedVolume>> pair in facedVolumizedWorld)
            {
                List<FacedVolume> volumes = pair.Value;
                for (int idx = 0; idx < volumes.Count; idx++)
                {
                    FacedVolume facedVolume = volumes[idx];

                    Iterators.FacesInVolume(verticies.Count, facedVolume.excludedFaces, (FaceVerticies faceVerticies) =>
                    {
                        //facedVolume.volume.
                    });

                    Iterators.VerticiesInVolume(volumes[idx].volume,
                        (CoordinateDecimal a) => { verticies.Add(a); });
                }
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

        public override string ToString() { return sb.ToString(); }
    }
}
