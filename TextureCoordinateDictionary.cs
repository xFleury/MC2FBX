using System;
using System.Collections.Generic;
using System.Drawing;

namespace MC2UE
{
    /// <summary>Attempts to use the fewest possible "vt" texture coordinates.</summary>
    class TextureCoordinateDictionary
    {
        public readonly List<Point> mappingList = new List<Point>{Point.Empty};
        private readonly Dictionary<int, int> mappingDX = new Dictionary<int, int>();
        private readonly Dictionary<int, int> mappingDY = new Dictionary<int, int>();
        private readonly Dictionary<Size, int> mappingDXY = new Dictionary<Size, int>();

        /// <summary>Remember to add +1 to the index when writing OBJ.</summary>
        public void GetMapping(Size mappingSize, out int index1, out int index2, out int index3, out int index4)
        {
            index1 = 0;
            index2 = mappingDY[mappingSize.Height];
            index3 = mappingDXY[mappingSize];
            index4 = mappingDX[mappingSize.Width];
        }

        public void EnsureExists(Size mappingSize)
        {
            if (!mappingDX.ContainsKey(mappingSize.Width))
            {
                mappingList.Add(new Point(mappingSize.Width, 0));
                mappingDX.Add(mappingSize.Width, mappingList.Count - 1);
            }

            if (!mappingDY.ContainsKey(mappingSize.Height))
            {
                mappingList.Add(new Point(0, mappingSize.Height));
                mappingDY.Add(mappingSize.Height, mappingList.Count - 1);
            }

            if (!mappingDXY.ContainsKey(mappingSize))
            {
                mappingList.Add(new Point(mappingSize.Width, mappingSize.Height));
                mappingDXY.Add(mappingSize, mappingList.Count - 1);
            }
        }
    }
}

