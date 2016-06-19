using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtToObj.Minecraft
{
    static class LookupOpaqueBlock
    {
        private static BlockIdentifier[] nonOpaqueBlocks = new BlockIdentifier[] {
            BlockIdentifier.Air, BlockIdentifier.Glass};

        public static bool IsOpaque(BlockIdentifier blockIdentifier)
        {
            return Array.IndexOf(nonOpaqueBlocks, blockIdentifier) == -1;
        }
    }
}
