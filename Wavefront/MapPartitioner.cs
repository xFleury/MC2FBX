using System.IO;
using System.Collections;
using System.Collections.Generic;
using NbtToObj.Geometry;
using NbtToObj.Minecraft;

namespace NbtToObj.Wavefront
{
    class MapPartitioner : IEnumerable<KeyValuePair<string, Dictionary<CoordinateInt, Block>>>
    {
        public readonly Dictionary<PhysicalMaterial, Dictionary<CoordinateInt, Block>> partitions =
            new Dictionary<PhysicalMaterial, Dictionary<CoordinateInt, Block>>();
        private readonly string outputPath;

        public MapPartitioner(string outputPath)
        {
            this.outputPath = outputPath;
        }

        public void Add(CoordinateInt coord, Block block)
        {
            PhysicalMaterial physicalMaterial = LookupPhysicalMaterial.FromBlockIdentifier(block.id);

            Dictionary<CoordinateInt, Block> blockDict;
            if (partitions.TryGetValue(physicalMaterial, out blockDict))
                blockDict.Add(coord, block);
            else
            {
                blockDict = new Dictionary<CoordinateInt, Block>();
                blockDict.Add(coord,block);
                partitions.Add(physicalMaterial, blockDict);
            }
        }

        public IEnumerator<KeyValuePair<string, Dictionary<CoordinateInt, Block>>> GetEnumerator()
        {
            string combinedPath = Path.Combine(Path.GetDirectoryName(outputPath), Path.GetFileNameWithoutExtension(outputPath));
            string extension = Path.GetExtension(outputPath);

            foreach (KeyValuePair<PhysicalMaterial, Dictionary<CoordinateInt, Block>> pair in partitions)
            {
                string newOutput = string.Format("{0}.{1}{2}",
                    combinedPath, pair.Key.ToString(), extension);
                yield return new KeyValuePair<string, Dictionary<CoordinateInt, Block>>(newOutput, pair.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}