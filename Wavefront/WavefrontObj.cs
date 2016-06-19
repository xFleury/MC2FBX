using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using NbtToObj.Geometry;
using NbtToObj.Optimizer;
using NbtToObj.Minecraft;
using NbtToObj.Helpers;

namespace NbtToObj.Wavefront
{
    class WavefrontObj
    {
        private readonly List<CoordinateDecimal> vertices;
        private readonly Dictionary<string, List<FaceVertices>> collisionBoxes;
        private readonly Dictionary<BlockFaceTexture, List<TexturedFace>> texturedFaces;
        private readonly TextureCoordinateDictionary textureCoordinates;

        public WavefrontObj(
            List<CoordinateDecimal> vertices,
            Dictionary<string, List<FaceVertices>> collisionBoxes,
            Dictionary<BlockFaceTexture, List<TexturedFace>> texturedFaces,
            TextureCoordinateDictionary textureCoordinates,
            MultiValueDictionary<Block, FacedVolume> facedVolumizedWorld)
        {
            this.vertices = vertices;
            this.collisionBoxes = collisionBoxes;
            this.texturedFaces = texturedFaces;
            this.textureCoordinates = textureCoordinates;
        }


        public override string ToString()
        {
            const int maxDecimalPlaces = 6;

            StringBuilder sb = new StringBuilder();
            for (int idx = 0; idx < vertices.Count; idx++)
            {
                CoordinateDecimal coord = vertices[idx];
                sb.AppendLine(string.Format("v {0:0.0#####} {1:0.0#####} {2:0.0#####}", 
                    Math.Round(coord.X, maxDecimalPlaces),
                    Math.Round(coord.Y, maxDecimalPlaces), 
                    Math.Round(coord.Z, maxDecimalPlaces)));
            }

            for (int idx = 0; idx < textureCoordinates.mappingList.Count; idx++)
            {
                Point point = textureCoordinates.mappingList[idx];
                sb.AppendLine(string.Format("vt {0:0.0#####} {1:0.0#####}", 
                    point.X, point.Y));
            }

            foreach (KeyValuePair<string, List<FaceVertices>> pair in collisionBoxes)
            {
                sb.AppendLine("g " + pair.Key);
                List<FaceVertices> listOfTexturedFaces = pair.Value;
                for (int idx = 0; idx < listOfTexturedFaces.Count; idx++)
                {
                    FaceVertices faceVertices = listOfTexturedFaces[idx];


                    sb.AppendLine(string.Format("f {0} {1} {2} {3}",
                        1 + faceVertices.index1,
                        1 + faceVertices.index2,
                        1 + faceVertices.index3,
                        1 + faceVertices.index4
                    ));
                }
            }

            foreach (KeyValuePair<BlockFaceTexture, List<TexturedFace>> pair in texturedFaces)
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
