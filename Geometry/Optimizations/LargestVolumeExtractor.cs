using System;
using System.Collections;
using System.Collections.Generic;

namespace MC2UE.Geometry.Optimizations
{
    /// <summary>Extracts the largest volume contigious volume within a hashset of blocks.</summary>
    class LargestVolumeExtractor : IEnumerable<Volume>
    {
        HashSet<CoordinateInt> visibleBlocks;
        HashSet<CoordinateInt> invisibleBlocks;

        public LargestVolumeExtractor(HashSet<CoordinateInt> visibleBlocks, HashSet<CoordinateInt> invisibleBlocks)
        {
            this.visibleBlocks = new HashSet<CoordinateInt>(visibleBlocks);
            this.invisibleBlocks = new HashSet<CoordinateInt>(invisibleBlocks);
        }

        public IEnumerator<Volume> GetEnumerator()
        {
            while (visibleBlocks.Count > 0)
            {
                Console.Write("\r" + visibleBlocks.Count + " remaining.");
                Volume limitedVolume = LargestVolume_OnlyVisible();
                //KeyValuePair<int, Volume> noLimitVolume = LargestVolume_AnyVisibility();

                //if (limitedVolume.TotalVolume >= noLimitVolume.Key)
                //{

                Iterators.BlocksInVolume(limitedVolume, (CoordinateInt a) => { visibleBlocks.Remove(a); });

                yield return limitedVolume;
                //}
                //else
                //{
                //    foreach (Coordinate coord in noLimitVolume.Value)
                //        if (CoordinateIsVisible(coord))
                //            visibleBlocks.Remove(coord);
                //        else
                //            invisibleBlocks.Remove(coord);
                //    yield return noLimitVolume.Value;
                //}
            }
            Console.Write(Environment.NewLine);
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        private bool CoordinateIsVisible(CoordinateInt coord) { return visibleBlocks.Contains(coord); }

        private int CountVisibleVolume(Volume volume)
        {
            int totalVisible = 0;
            for (int x = 0; x < volume.ScaleX; x++)
                for (int y = 0; y < volume.ScaleY; y++)
                    for (int z = 0; z < volume.ScaleZ; z++)
                        if (CoordinateIsVisible(volume.Coord.Offset(x, y, z)))
                            totalVisible++;
            return totalVisible;
        }

        private bool LargestVolume_Valid(CoordinateInt coord, int lengthX, int lengthY, int lengthZ, bool allowInvisible)
        {
            for (int gZ = lengthZ-1; gZ >= 0; gZ--)
                for (int gY = lengthY - 1; gY >= 0; gY--)
                    for (int gX = lengthX - 1; gX >= 0; gX--)
                        if (!SearchAllBlocks(coord.Offset(gX, gY, gZ), allowInvisible))
                            return false;
            return true;
        }

        private Volume LargestVolume_Iterate(CoordinateInt origin, bool allowInvisible)
        {
            Volume largestVolume = new Volume();

            int maxX = 0;
            while (SearchAllBlocks(origin.Offset(maxX, 0, 0), allowInvisible)) maxX++;
            int maxY = 0;
            while (SearchAllBlocks(origin.Offset(0, maxY, 0), allowInvisible)) maxY++;
            int maxZ = 0;
            while (SearchAllBlocks(origin.Offset(0, 0, maxZ), allowInvisible)) maxZ++;

            for (int extentZ = maxZ; extentZ >= 0; extentZ--)
                for (int extentY = maxY; extentY >= 0; extentY--)
                    for (int extentX = maxX; extentX >= 0; extentX--)
                    {
                        int lengthX = extentX + 1;
                        int lengthY = extentY + 1;
                        int lengthZ = extentZ + 1;

                        /* Don't bother considering this volume if it won't get used. */
                        if (lengthX * lengthY * lengthZ <= largestVolume.TotalVolume) continue;

                        if (LargestVolume_Valid(origin, lengthX, lengthY, lengthZ, allowInvisible))
                        {
                            int totalVolume = lengthX * lengthY * lengthZ;
                            if (totalVolume >= largestVolume.TotalVolume)
                                largestVolume = new Volume(origin, lengthX, lengthY, lengthZ);
                        }
                        //else
                          //  break;
                    }
            return largestVolume;
        }

        private Volume LargestVolume_OnlyVisible()
        {
            Volume largestVolume = new Volume();
            foreach (CoordinateInt origin in visibleBlocks)
            {
                Volume volume = LargestVolume_Iterate(origin, false);
                if (volume.TotalVolume >= largestVolume.TotalVolume)
                    largestVolume = volume;
            }
            return largestVolume;
        }

        private KeyValuePair<int, Volume> LargestVolume_AnyVisibility()
        {
            Volume largestVolumeVisibleOrigin = new Volume();
            Volume largestVolumeInvisibleOrigin = new Volume();
            foreach (CoordinateInt origin in visibleBlocks)
                largestVolumeVisibleOrigin = LargestVolume_Iterate(origin, true);
            foreach (CoordinateInt origin in invisibleBlocks)
                largestVolumeInvisibleOrigin = LargestVolume_Iterate(origin, true);
            int actualVolumeVisible = CountVisibleVolume(largestVolumeVisibleOrigin);
            int actualVolumeInvisible = CountVisibleVolume(largestVolumeInvisibleOrigin);

            if (actualVolumeVisible > actualVolumeInvisible)
                return new KeyValuePair<int, Volume>(actualVolumeVisible, largestVolumeVisibleOrigin);
            else
                return new KeyValuePair<int, Volume>(actualVolumeInvisible, largestVolumeInvisibleOrigin);
        }

        private bool SearchAllBlocks(CoordinateInt coord, bool allowInvisible)
        {
            return visibleBlocks.Contains(coord) || (allowInvisible && invisibleBlocks.Contains(coord));
        }
    }
}

