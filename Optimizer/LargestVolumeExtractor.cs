using NbtToObj.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NbtToObj.Optimizer
{
    /// <summary>Extracts the largest contigious volume within a hashset of blocks.</summary>
    static class LargestVolumeExtractor
    {
        public static Volume ExtractAndSubtract(HashSet<CoordinateInt> blocks)
        {
            Volume largestVolume = FindLargestVolume(blocks);
            Iterators.BlocksInVolume(largestVolume, a => blocks.Remove(a));
            return largestVolume;
        }

        private static Volume FindLargestVolume(HashSet<CoordinateInt> blocks)
        {
            Volume largestVolume = new Volume();
            foreach (CoordinateInt origin in blocks)
            {
                Volume volume = BruteForceSearchForLargest(origin, blocks);
                if (volume.TotalVolume >= largestVolume.TotalVolume)
                    largestVolume = volume;
            }
            return largestVolume;
        }

        private static Volume BruteForceSearchForLargest(CoordinateInt origin, HashSet<CoordinateInt> blocks)
        {
            Volume largestVolume = new Volume();

            int maxX = 0;
            while (blocks.Contains(origin.Offset(maxX, 0, 0))) maxX++;
            int maxY = 0;
            while (blocks.Contains(origin.Offset(0, maxY, 0))) maxY++;
            int maxZ = 0;
            while (blocks.Contains(origin.Offset(0, 0, maxZ))) maxZ++;

            for (int extentZ = maxZ; extentZ >= 0; extentZ--)
                for (int extentY = maxY; extentY >= 0; extentY--)
                    for (int extentX = maxX; extentX >= 0; extentX--)
                    {
                        int lengthX = extentX + 1;
                        int lengthY = extentY + 1;
                        int lengthZ = extentZ + 1;

                        /* Don't bother considering this volume if it won't get used. */
                        if (lengthX * lengthY * lengthZ <= largestVolume.TotalVolume) continue;

                        if (IsVolumeFilledWithBlocks(origin, lengthX, lengthY, lengthZ, blocks))
                        {
                            int totalVolume = lengthX * lengthY * lengthZ;
                            if (totalVolume >= largestVolume.TotalVolume)
                                largestVolume = new Volume(origin, lengthX, lengthY, lengthZ);
                        }
                    }
            return largestVolume;
        }

        private static bool IsVolumeFilledWithBlocks(CoordinateInt coord, int lengthX, int lengthY, int lengthZ, HashSet<CoordinateInt> blocks)
        {
            for (int gZ = lengthZ - 1; gZ >= 0; gZ--)
                for (int gY = lengthY - 1; gY >= 0; gY--)
                    for (int gX = lengthX - 1; gX >= 0; gX--)
                        if (!blocks.Contains(coord.Offset(gX, gY, gZ)))
                            return false;
            return true;
        }
    }
}

