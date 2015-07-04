using System;
using System.Collections;
using System.Collections.Generic;

namespace MC2FBX
{
    /// <summary>Extracts the largest box within a volume.</summary>
    public class BoxExtractor : IEnumerable<Volume>
    {
        HashSet<Coordinate> visibleBlocks;
        HashSet<Coordinate> invisibleBlocks;

        public BoxExtractor(HashSet<Coordinate> visibleBlocks, HashSet<Coordinate> invisibleBlocks)
        {
            this.visibleBlocks = new HashSet<Coordinate>(visibleBlocks);
            this.invisibleBlocks = new HashSet<Coordinate>(invisibleBlocks);
        }

        public IEnumerator<Volume> GetEnumerator()
        {
            while (visibleBlocks.Count > 0)
            {
                Console.Write("\rOptimizing [" + visibleBlocks.Count + " remaining]");

                Volume limitedVolume = LargestVolume_OnlyVisible();
                //KeyValuePair<int, Volume> noLimitVolume = LargestVolume_AnyVisibility();

                //if (limitedVolume.TotalVolume >= noLimitVolume.Key)
                //{
                foreach (Coordinate coord in limitedVolume)
                    visibleBlocks.Remove(coord);
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
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        private bool CoordinateIsVisible(Coordinate coord) { return visibleBlocks.Contains(coord); }

        private int CountVisibleVolume(Volume volume)
        {
            int totalVisible = 0;
            for (int x = 0; x < volume.Width; x++)
                for (int y = 0; y < volume.Height; y++)
                    for (int z = 0; z < volume.Length; z++)
                        if (CoordinateIsVisible(volume.Coord.Offset(x, y, z)))
                            totalVisible++;
            return totalVisible;
        }

        private bool LargestVolume_Valid(Coordinate coord, int width, int height, int length, bool allowInvisible)
        {
            for (int gX = 0; gX < width; gX++)
                for (int gY = 0; gY < height; gY++)
                    for (int gZ = 0; gZ < length; gZ++)
                        if (!SearchAllBlocks(coord.Offset(gX, gY, gZ), allowInvisible))
                            return false;
            return true;
        }

        private Volume LargestVolume_Iterate(Coordinate origin, bool allowInvisible)
        {
            Volume largestVolume = new Volume();

            int maxX = 0;
            while (SearchAllBlocks(origin.Offset(maxX, 0, 0), allowInvisible)) maxX++;
            int maxY = 0;
            while (SearchAllBlocks(origin.Offset(0, maxY, 0), allowInvisible)) maxY++;
            int maxZ = 0;
            while (SearchAllBlocks(origin.Offset(0, 0, maxZ), allowInvisible)) maxZ++;

            for (int extentX = 0; extentX <= maxX; extentX++)
                for (int extentY = 0; extentY <= maxY; extentY++)
                    for (int extentZ = 0; extentZ <= maxZ; extentZ++)
                    {
                        int width = extentX + 1;
                        int height = extentY + 1;
                        int length = extentZ + 1;
                        if (LargestVolume_Valid(origin, width, height, length, allowInvisible))
                        {
                            int totalVolume = width * height * length;
                            if (totalVolume >= largestVolume.TotalVolume)
                                largestVolume = new Volume(origin, width, height, length);
                        }
                        else
                            break;
                    }
            return largestVolume;
        }

        private Volume LargestVolume_OnlyVisible()
        {
            Volume largestVolume = new Volume();
            foreach (Coordinate origin in visibleBlocks)
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
            foreach (Coordinate origin in visibleBlocks)
                largestVolumeVisibleOrigin = LargestVolume_Iterate(origin, true);
            foreach (Coordinate origin in invisibleBlocks)
                largestVolumeInvisibleOrigin = LargestVolume_Iterate(origin, true);
            int actualVolumeVisible = CountVisibleVolume(largestVolumeVisibleOrigin);
            int actualVolumeInvisible = CountVisibleVolume(largestVolumeInvisibleOrigin);

            if (actualVolumeVisible > actualVolumeInvisible)
                return new KeyValuePair<int, Volume>(actualVolumeVisible, largestVolumeVisibleOrigin);
            else
                return new KeyValuePair<int, Volume>(actualVolumeInvisible, largestVolumeInvisibleOrigin);
        }

        private bool SearchAllBlocks(Coordinate coord, bool allowInvisible)
        {
            return visibleBlocks.Contains(coord) || (allowInvisible && invisibleBlocks.Contains(coord));
        }
    }
}

