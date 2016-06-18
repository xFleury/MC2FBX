using System.IO;
using System.Collections;
using System.Collections.Generic;
using NbtToObj.Geometry;
using NbtToObj.Minecraft;

namespace NbtToObj.Wavefront
{
    class PhysicalMaterialPartitioner : IEnumerable<KeyValuePair<PhysicalMaterial, Dictionary<CoordinateInt, Block>>>
    {
        private readonly Dictionary<PhysicalMaterial, Dictionary<CoordinateInt, Block>> partitions =
            new Dictionary<PhysicalMaterial, Dictionary<CoordinateInt, Block>>();

        public PhysicalMaterialPartitioner(Dictionary<CoordinateInt, Block> blocks)
        {
            foreach (KeyValuePair<CoordinateInt, Block> pair in blocks)
                Add(pair.Key, pair.Value);
        }

        private void Add(CoordinateInt coord, Block block)
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

        public IEnumerator<KeyValuePair<PhysicalMaterial, Dictionary<CoordinateInt, Block>>> GetEnumerator()
        {
            return partitions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}