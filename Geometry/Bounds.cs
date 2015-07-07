using System;

namespace MC2UE
{
    class Bounds
    {
        const int chunkSize = 16;

        public int minX = int.MaxValue / 2;
        public int minY = int.MaxValue / 2;
        public int minZ = int.MaxValue / 2;
        public int maxX = int.MinValue / 2;
        public int maxY = int.MinValue / 2;
        public int maxZ = int.MinValue / 2;

        public int Width { get { return maxX - minX; } }

        public int Height { get { return maxY - minY; } }

        public int Length { get { return maxZ - minZ; } }

        public bool ContainsChunk(CoordinateInt chunkCoord)
        {
            return !(
               chunkCoord.X * chunkSize + 16 < minX || chunkCoord.X * chunkSize > maxX ||
               chunkCoord.Y * chunkSize + 16 < minY || chunkCoord.Y * chunkSize > maxY ||
               chunkCoord.Z * chunkSize + 16 < minZ || chunkCoord.Z * chunkSize > maxZ);
        }

        public bool Contains(int x, int y, int z)
        {
            return !(x < minX || x > maxX || y < minY || y > maxY || z < minZ || z > maxZ);
        }

        public CoordinateInt GetOrigin()
        {
            return new CoordinateInt((maxX + minX) / 2, (maxY + minY) / 2, (maxZ + minZ) / 2);
        }

        public void AddPoint(int x, int y, int z)
        {
            minX = Math.Min(minX, x);
            minY = Math.Min(minY, y);
            minZ = Math.Min(minZ, z);
            maxX = Math.Max(maxX, x);
            maxY = Math.Max(maxY, y);
            maxZ = Math.Max(maxZ, z);
        }

        public void Shrink(int amount)
        {
            minX += amount;
            minY += amount;
            minZ += amount;
            maxX -= amount;
            maxY -= amount;
            maxZ -= amount;
        }

        public override string ToString()
        {
            return string.Format("({0}x{1}x{2} to {3}x{4}x{5})",
                minX, minY, minZ, maxX, maxY, maxZ);
        }
    }
}
