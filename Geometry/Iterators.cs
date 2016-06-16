using System;

namespace MC2UE.Geometry
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

        public static void VerticesInVolume(Volume volume, Action<CoordinateDecimal> action)
        {
            VerticesInVolume((CoordinateDecimal)volume.Coord, volume.ScaleX, volume.ScaleY, volume.ScaleZ, action);
        }

        /// <summary>Vertices in a volume.</summary>
        public static void VerticesInVolume(CoordinateDecimal coord, decimal width, decimal height, decimal length, Action<CoordinateDecimal> action)
        {
            /* front four vertices */
            action(coord.Offset(0m, 0m, length));
            action(coord);
            action(coord.Offset(width, 0m, 0m));
            action(coord.Offset(width, 0m, length));


            /* back four vertices */
            action(coord.Offset(0m, height, length));
            action(coord.Offset(0m, height, 0m));
            action(coord.Offset(width, height, 0m));
            action(coord.Offset(width, height, length));
        }

        /// <summary>Vertices in a volume.</summary>
        public static void FacesInVolume(int startIndexOfVertices, Face exclusions, Action<Face, FaceVertices> action)
        {
            int i = startIndexOfVertices;

            /* front four vertices */
            if (!FastFlagCheck(exclusions, Face.PositiveX))
                action(Face.PositiveX, new FaceVertices(i+3, i+2, i+6, i+7));
            if (!FastFlagCheck(exclusions, Face.NegativeY))
                action(Face.PositiveY, new FaceVertices(i+0, i+1, i+2, i+3));
            if (!FastFlagCheck(exclusions, Face.PositiveZ))
                action(Face.PositiveZ, new FaceVertices(i+4, i+0, i+3, i+7));

            /* back four vertices */
            if (!FastFlagCheck(exclusions, Face.NegativeX))
                action(Face.NegativeX, new FaceVertices(i+4, i+5, i+1, i+0));
            if (!FastFlagCheck(exclusions, Face.PositiveY))
                action(Face.NegativeY, new FaceVertices(i+7, i+6, i+5, i+4));
            if (!FastFlagCheck(exclusions, Face.NegativeZ))
                action(Face.NegativeZ, new FaceVertices(i+1, i+5, i+6, i+2));
        }

        private static bool FastFlagCheck(Face exclusion, Face flag) { return (exclusion & flag) != 0; }
    }
}
