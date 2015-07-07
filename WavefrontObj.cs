using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace MC2UE
{
    class WavefrontObj
    {
        private readonly List<CoordinateDecimal> vertices = new List<CoordinateDecimal>();
        private readonly Dictionary<string, FaceVertices[]> collisionBoxes = new Dictionary<string, FaceVertices[]>();
        private readonly Dictionary<BlockType, List<TexturedFace>> texturedFaces = new Dictionary<BlockType, List<TexturedFace>>();
        private readonly TextureCoordinateDictionary textureCoordinates = new TextureCoordinateDictionary();

        public WavefrontObj(Dictionary<BlockType, List<FacedVolume>> facedVolumizedWorld)
        {
            foreach (KeyValuePair<BlockType, List<FacedVolume>> pair in facedVolumizedWorld)
            {
                List<FacedVolume> volumes = pair.Value;
                for (int idx = 0; idx < volumes.Count; idx++)
                {
                    FacedVolume facedVolume = volumes[idx];

                    Iterators.FacesInVolume(vertices.Count, facedVolume.excludedFaces, (Face face, FaceVertices faceVertices) =>
                        { AppendTexturedFaces(pair.Key, facedVolume.volume, face, faceVertices); });
                    
                    Iterators.VerticesInVolume(volumes[idx].volume,
                        (CoordinateDecimal a) => { vertices.Add(a); });
                }
            }
            Console.WriteLine(textureCoordinates.mappingList.Count + " unique texture coordinates.");
        }

        private void AppendTexturedFaces(BlockType blockType, Volume volume, Face face, FaceVertices faceVertices)
        {
            TexturedFace texturedFace = new TexturedFace(volume, face, faceVertices);
            List<TexturedFace> texturedFacesList;
            if (texturedFaces.TryGetValue(blockType, out texturedFacesList))
                texturedFacesList.Add(texturedFace);
            else
            {
                texturedFacesList = new List<TexturedFace>();
                texturedFacesList.Add(texturedFace);
                texturedFaces.Add(blockType, texturedFacesList);
            }
            textureCoordinates.EnsureExists(texturedFace.textureMapping);
        }

        private void AppendVertices(List<FacedVolume> volumes)
        {
            for (int idx = 0; idx < volumes.Count; idx++)
            {
                Volume volume = volumes[idx].volume;
                vertices.Add(new CoordinateDecimal(
                    volume.Coord.X, volume.Coord.Y, volume.Coord.Z));
            }
        }

        public override string ToString()
        {
            const int maxDecimalPlaces = 6;

            StringBuilder sb = new StringBuilder();
            for (int idx = 0; idx < vertices.Count; idx++)
            {
                CoordinateDecimal coord = vertices[idx];
                sb.AppendLine(string.Format("v {0:0.0} {1:0.0} {2:0.0}", 
                    Math.Round(coord.X, maxDecimalPlaces),
                    Math.Round(coord.Y, maxDecimalPlaces), 
                    Math.Round(coord.Z, maxDecimalPlaces)));
            }

            for (int idx = 0; idx < textureCoordinates.mappingList.Count; idx++)
            {
                Point point = textureCoordinates.mappingList[idx];
                sb.AppendLine(string.Format("vt {0:0.0} {1:0.0}", 
                    point.X, point.Y));
            }

            foreach (KeyValuePair<BlockType, List<TexturedFace>> pair in texturedFaces)
            {
                sb.AppendLine("g " + pair.Key.ToString());
                sb.AppendLine("usemtl " + pair.Key.ToString());
                List<TexturedFace> listOfTexturedFaces = pair.Value;
                for (int idx = 0; idx < listOfTexturedFaces.Count; idx++)
                {
                    TexturedFace texturedFace = listOfTexturedFaces[idx];

                    int texIndex1;
                    int texIndex2;
                    int texIndex3;
                    int texIndex4;
                    textureCoordinates.GetMapping(texturedFace.textureMapping,
                        out texIndex1, out texIndex2, out texIndex3, out texIndex4);

                    sb.AppendLine(string.Format("f {0}/{4} {1}/{5} {2}/{6} {3}/{7}",
                        1 + texturedFace.faceVertices.index1,
                        1 + texturedFace.faceVertices.index2,
                        1 + texturedFace.faceVertices.index3,
                        1 + texturedFace.faceVertices.index4,
                        1 + texIndex1,
                        1 + texIndex2,
                        1 + texIndex3,
                        1 + texIndex4
                    ));
                }
            }
                
            return sb.ToString();
        }
    }
}
