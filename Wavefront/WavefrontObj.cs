using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using NbtToObj.Geometry;
using NbtToObj.Optimizer;
using NbtToObj.Minecraft;
using NbtToObj.Helpers;
using NbtToObj.Gui;

namespace NbtToObj.Wavefront
{
    static class WavefrontObj
    {
        public static string Generate(WorldState worldState)
        {
            const int maxDecimalPlaces = 6;

            StringBuilder sb = new StringBuilder();
            for (int idx = 0; idx < worldState.vertices.Count; idx++)
            {
                CoordinateDecimal coord = worldState.vertices[idx];
                sb.AppendLine(string.Format("v {0:0.0#####} {1:0.0#####} {2:0.0#####}", 
                    Math.Round(coord.X, maxDecimalPlaces),
                    Math.Round(coord.Y, maxDecimalPlaces), 
                    Math.Round(coord.Z, maxDecimalPlaces)));
            }

            foreach(TextureCoordinate texCoord in worldState.textureCoordinates.mappingDict
                .OrderBy(o => o.Value)
                .Select(s => s.Key))
            {
                sb.AppendLine(string.Format("vt {0:0.0#####} {1:0.0#####}",
                    texCoord.U, texCoord.V));
            }

            foreach (KeyValuePair<string, List<FaceVertices>> pair in worldState.collisionBoxes)
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

            foreach (ChunkState chunkState in worldState.chunks)
            {
                sb.AppendLine("g " + chunkState.chunkName);

                foreach (FacedVolume facedVolume in chunkState.facedVolumes)
                {
                    foreach (KeyValuePair<BlockFaceTexture, List<TexturedFace>> texturedFaces in worldState.texturedFacesOfFacedVolume[facedVolume])
                    {
                        sb.AppendLine("usemtl " + texturedFaces.Key.ToString());
                        List<TexturedFace> listOfTexturedFaces = texturedFaces.Value;
                        for (int idx = 0; idx < listOfTexturedFaces.Count; idx++)
                        {
                            TexturedFace texturedFace = listOfTexturedFaces[idx];

                            int texIndex1;
                            int texIndex2;
                            int texIndex3;
                            int texIndex4;
                            worldState.textureCoordinates.GetMapping(texturedFace.textureCoord, texturedFace.textureSize,
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
                }
            }
                
            return sb.ToString();
        }
    }
}
