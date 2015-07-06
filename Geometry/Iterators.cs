using System;

namespace MC2FBX
{
    static class Iterators
    {
        /// <summary>Iterates over all the cubes contained in a volume.</summary>
        public static void CubesInVolume(Volume volume, Action<CoordinateInt> action)
        {
            for (int x = 0; x < volume.Width; x++)
                for (int y = 0; y < volume.Height; y++)
                    for (int z = 0; z < volume.Length; z++)
                        action(volume.Coord.Offset(x, y, z));
        }

        /// <summary>Verticies in a volume.</summary>
        public static void VerticiesInVolume(CoordinateDecimal coord, decimal width, decimal height, decimal length, Action<CoordinateDecimal> action)
        {
            /* front four verticies */
            action(coord);
            action(coord.Offset(width, 0, 0));
            action(coord.Offset(width, height, 0));
            action(coord.Offset(0, height, 0));
            /* back four verticies */
            action(coord.Offset(0,0,length));
            action(coord.Offset(width, 0, length));
            action(coord.Offset(width, height, length));
            action(coord.Offset(0, height, length));
        }
    }
}
