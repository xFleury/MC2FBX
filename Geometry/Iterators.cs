using System;

namespace MC2FBX
{
    static class Iterators
    {
        /// <summary>Iterates over all the blocks contained in a volume.</summary>
        public static void BlocksInVolume(Volume volume, Action<CoordinateInt> action)
        {
            for (int x = 0; x < volume.ScaleX; x++)
                for (int y = 0; y < volume.ScaleY; y++)
                    for (int z = 0; z < volume.ScaleZ; z++)
                        action(volume.Coord.Offset(x, y, z));
        }

        public static void VerticiesInVolume(Volume volume, Action<CoordinateDecimal> action)
        {
            VerticiesInVolume((CoordinateDecimal)volume.Coord, volume.ScaleX, volume.ScaleY, volume.ScaleZ, action);
        }

        /// <summary>Verticies in a volume.</summary>
        public static void VerticiesInVolume(CoordinateDecimal coord, decimal width, decimal height, decimal length, Action<CoordinateDecimal> action)
        {
            /* front four verticies */
            action(coord);
            action(coord.Offset(0m, height, 0m));
            action(coord.Offset(width, height, 0m));
            action(coord.Offset(width, 0m, 0m));

            /* back four verticies */
            action(coord.Offset(0m, 0m, length));
            action(coord.Offset(0m, height, length));
            action(coord.Offset(width, height, length));
            action(coord.Offset(width, 0m, length));
        }

        /// <summary>Verticies in a volume.</summary>
        public static void FacesInVolume(int startIndexOfVerticies, Face exclusions, Action<Face, FaceVerticies> action)
        {
            int i = startIndexOfVerticies;

            /* front four verticies */
            if (!FastFlagCheck(exclusions, Face.PositiveX))
                action(Face.PositiveX, new FaceVerticies(i+3, i+2, i+6, i+7));
            if (!FastFlagCheck(exclusions, Face.PositiveY))
                action(Face.PositiveY, new FaceVerticies(i+0, i+1, i+2, i+3));
            if (!FastFlagCheck(exclusions, Face.PositiveZ))
                action(Face.PositiveZ, new FaceVerticies(i+4, i+0, i+3, i+7));

            /* back four verticies */
            if (!FastFlagCheck(exclusions, Face.NegativeX))
                action(Face.NegativeX, new FaceVerticies(i+4, i+5, i+1, i+0));
            if (!FastFlagCheck(exclusions, Face.NegativeY))
                action(Face.NegativeY, new FaceVerticies(i+7, i+6, i+5, i+4));
            if (!FastFlagCheck(exclusions, Face.NegativeZ))
                action(Face.NegativeZ, new FaceVerticies(i+1, i+5, i+6, i+2));
        }

        private static bool FastFlagCheck(Face exclusion, Face flag) { return (exclusion & flag) != 0; }
    }
}
