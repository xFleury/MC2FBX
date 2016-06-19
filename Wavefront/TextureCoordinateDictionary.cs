using NbtToObj.Geometry;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

namespace NbtToObj.Wavefront
{
    /// <summary>Attempts to use the fewest possible "vt" texture coordinates.</summary>
    class TextureCoordinateDictionary
    {
        public readonly Dictionary<TextureCoordinate, int> mappingDict = new Dictionary<TextureCoordinate, int>();

        public void GetMapping(TextureCoordinate texCoord, SizeD texSize, out int index1, out int index2, out int index3, out int index4)
        {
            TextureCoordinate[] coords = IterateTextureCoordinates(texCoord, texSize).ToArray();
            /* UV mapping has 0,0 the bottom-left, but we defined it top-left. */
            index1 = mappingDict[coords[0]];
            index2 = mappingDict[coords[1]];
            index3 = mappingDict[coords[2]];
            index4 = mappingDict[coords[3]];
        }

        public void EnsureExists(TextureCoordinate texCoord, SizeD texSize)
        {
            foreach (TextureCoordinate coord in IterateTextureCoordinates(texCoord, texSize))
                if (!mappingDict.ContainsKey(coord))
                {
                    mappingDict.Add(coord, mappingDict.Count);
                }
        }

        private static IEnumerable<TextureCoordinate> IterateTextureCoordinates(TextureCoordinate texCoord, SizeD texSize)
        {
            /* UV mapping has 0,0 the bottom-left, but we defined it top-left. */
            yield return new TextureCoordinate(texCoord.U, texCoord.V + texSize.Height);
            yield return texCoord;
            yield return new TextureCoordinate(texCoord.U + texSize.Width, texCoord.V);
            yield return new TextureCoordinate(texCoord.U + texSize.Width, texCoord.V + texSize.Height);
        }
    }
}

