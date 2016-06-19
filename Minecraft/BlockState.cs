using NbtToObj.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtToObj.Minecraft
{
    struct BlockState
    {
        public bool hasDiagonals;
        public bool isCornerstone;
        public int rotation;

        public BlockState(Dictionary<CoordinateInt, CompositeBlock> blocks,
            KeyValuePair<CoordinateInt, CompositeBlock> currentBlock)
        {
            BlockIdentifier id = currentBlock.Value.id;
            CoordinateInt loc = currentBlock.Key;

            if (IsSameBlock(blocks, id, currentBlock.Key.Offset(-1, 0, 0)) &&
               IsSameBlock(blocks, id, currentBlock.Key.Offset(0, -1, 0)))
            {
                hasDiagonals = true;
                rotation = 0;
                isCornerstone = blocks.ContainsKey(loc.Offset(-1, -1, 0));
            }
            else if (IsSameBlock(blocks, id, currentBlock.Key.Offset(1, 0, 0)) &&
                IsSameBlock(blocks, id, currentBlock.Key.Offset(0, 1, 0)))
            {
                hasDiagonals = true;
                rotation = 1;
                isCornerstone = blocks.ContainsKey(loc.Offset(1, 1, 0));
            }
            else if (IsSameBlock(blocks, id, currentBlock.Key.Offset(1, 0, 0)) &&
               IsSameBlock(blocks, id, currentBlock.Key.Offset(0, -1, 0)))
            {
                hasDiagonals = true;
                rotation = 2;
                isCornerstone = blocks.ContainsKey(loc.Offset(1, -1, 0));
            }
            else if (IsSameBlock(blocks, id, currentBlock.Key.Offset(-1, 0, 0)) &&
               IsSameBlock(blocks, id, currentBlock.Key.Offset(0, 1, 0)))
            {
                hasDiagonals = true;
                rotation = 3;
                isCornerstone = blocks.ContainsKey(loc.Offset(-1, 1, 0));
            }
            else
            {
                hasDiagonals = false;
                rotation = 0;
                isCornerstone = false;
            }
        }

        private static bool IsSameBlock(Dictionary<CoordinateInt, CompositeBlock> blocks,
            BlockIdentifier id, CoordinateInt location)
        {
            CompositeBlock block;
            if (!blocks.TryGetValue(location, out block))
                return false;
            return block.id == id;
        }
    }
}
