using System.IO;
using System.Collections;
using System.Collections.Generic;
using NbtToObj.Geometry;

namespace NbtToObj
{
    class ChunkDivisor : IEnumerable<KeyValuePair<string, Dictionary<CoordinateInt, Block>>>
    {
        const int chunkSize = 64;

        public readonly Dictionary<CoordinateInt, Dictionary<CoordinateInt, Block>> chunks =
            new Dictionary<CoordinateInt, Dictionary<CoordinateInt, Block>>();
        private readonly string outputPath;

        public ChunkDivisor(string outputPath)
        {
            this.outputPath = outputPath;
        }

        public void Add(CoordinateInt coord, Block block)
        {
            CoordinateInt chunkIndex = new CoordinateInt(
                coord.X / chunkSize,
                coord.Y / chunkSize,
                coord.Z / chunkSize);
            Dictionary<CoordinateInt, Block> blockDict;
            if (chunks.TryGetValue(chunkIndex, out blockDict))
                blockDict.Add(coord, block);
            else
            {
                blockDict = new Dictionary<CoordinateInt, Block>();
                blockDict.Add(coord,block);
                chunks.Add(chunkIndex, blockDict);
            }
        }

        public IEnumerator<KeyValuePair<string, Dictionary<CoordinateInt, Block>>> GetEnumerator()
        {
            string combinedPath = Path.Combine(Path.GetDirectoryName(outputPath), Path.GetFileNameWithoutExtension(outputPath));
            string extension = Path.GetExtension(outputPath);

            foreach (KeyValuePair<CoordinateInt, Dictionary<CoordinateInt, Block>> pair in chunks)
            {
                string newOutput = string.Format("{0}.{1}.{2}.{3}{4}",
                    combinedPath, pair.Key.X, pair.Key.Y, pair.Key.Z, extension);
                yield return new KeyValuePair<string, Dictionary<CoordinateInt, Block>>(newOutput, pair.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}