using System.IO;
using System.Drawing;
using System.Collections.Generic;

namespace MC2UE
{
    static class Heightmap
    {
        public static bool IsHeightMap(string filePath) { return Path.GetExtension(filePath) == ".png"; }

        public static Dictionary<CoordinateInt, Block> ConvertToBlocks(string filePath, Block block)
        {
            Dictionary<CoordinateInt, Block> blocks = new Dictionary<CoordinateInt, Block>();

            using (Bitmap bitmap = new Bitmap(filePath))
            {
                using (FastPixelEditor pixelEditor = new FastPixelEditor(bitmap))
                {
                    int width = bitmap.Width;
                    int height = bitmap.Height;
                    Dictionary<byte, int> heightLookup = GetHeightLookup(width, height, pixelEditor);

                    for (int x = 0; x < width; x++)
                        for (int y = 0; y < height; y++)
                        {
                            byte gray = pixelEditor.GetPixel(x, y).R;
                            PaintIn2x2x1(x, y, heightLookup[gray], blocks, block);
                        }
                }
            }
            return blocks;
        }

        private static void PaintIn2x2x1(int xx, int yy, int z, Dictionary<CoordinateInt, Block> blocks, Block block)
        {
            for (int dX = 0; dX < 2; dX++)
                for (int dY = 0; dY < 2; dY++)
                    blocks.Add(new CoordinateInt(xx * 2 + dX, yy * 2 + dY, z), block);
        }

        private static Dictionary<byte, int> GetHeightLookup(int width, int height, FastPixelEditor pixelEditor)
        {
            SortedSet<byte> uniqueGrays = new SortedSet<byte>();
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    byte gray = pixelEditor.GetPixel(x, y).R;
                    if (!uniqueGrays.Contains(gray))
                        uniqueGrays.Add(gray);
                }

            Dictionary<byte, int> heightLookup = new Dictionary<byte, int>();
              int z = 0;
            foreach (byte gray in uniqueGrays)
            {
                heightLookup.Add(gray, z);
                z++;
            }
            return heightLookup;
        }
    }
}
