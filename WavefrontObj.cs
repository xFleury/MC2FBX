using System;
using System.Collections.Generic;
using System.Text;

namespace MC2FBX
{
    class WavefrontObj
    {
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

                    Iterators.FacesInVolume(verticies.Count, facedVolume.excludedFaces, (Face face, FaceVerticies faceVerticies) =>
                        { AppendTexturedFaces(pair.Key, facedVolume.volume, face, faceVerticies); });

                    Iterators.VerticiesInVolume(volumes[idx].volume,
                        (CoordinateDecimal a) => { verticies.Add(a); });
                }
            }
        }

        private void AppendTexturedFaces(BlockType blockType, Volume volume, Face face, FaceVerticies faceVerticies)
        {
            TexturedFace texturedFace = new TexturedFace(volume, face, faceVerticies);
            List<TexturedFace> texturedFacesList;
            if (texturedFaces.TryGetValue(blockType, out texturedFacesList))
                texturedFacesList.Add(texturedFace);
            else
            {
                texturedFacesList = new List<TexturedFace>();
                texturedFacesList.Add(texturedFace);
                texturedFaces.Add(blockType, texturedFacesList);
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

        public override string ToString()
        {
            const int maxDecimalPlaces = 6;

            StringBuilder sb = new StringBuilder();
            for (int idx = 0; idx < verticies.Count; idx++)
            {
                CoordinateDecimal coord = verticies[idx];
                sb.AppendLine(string.Format("v {0:0.0} {1:0.0} {2:0.0}", 
                    Math.Round(coord.X, maxDecimalPlaces),
                    Math.Round(coord.Y, maxDecimalPlaces), 
                    Math.Round(coord.Z, maxDecimalPlaces)));
            }

            foreach (KeyValuePair<BlockType, List<TexturedFace>> pair in texturedFaces)
            {
                sb.AppendLine("usemtl " + pair.Key.ToString());
                List<TexturedFace> listOfTexturedFaces = pair.Value;
                for (int idx = 0; idx < listOfTexturedFaces.Count; idx++)
                {
                    TexturedFace texturedFace = listOfTexturedFaces[idx];
                    sb.AppendLine(string.Format("f {0} {1} {2} {3}",
                        1 + texturedFace.faceVerticies.index1,
                        1 + texturedFace.faceVerticies.index2,
                        1 + texturedFace.faceVerticies.index3,
                        1 + texturedFace.faceVerticies.index4));
                }
            }


            return sb.ToString();
        }
    }
}
