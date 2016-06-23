using NbtToObj.Geometry;
using NbtToObj.Gui;
using NbtToObj.Helpers;
using NbtToObj.Minecraft;
using System.Collections.Generic;
using System.Diagnostics;

namespace NbtToObj.Optimizer
{
    static class UnobstructedFaces
    {
        public static void DetectHiddenFaces(WorldState worldState,
            HashSet<CoordinateInt> opaqueBlocks)
        {            
            foreach (ChunkState chunkState in worldState.chunks)
            {
                chunkState.facedVolumes = new List<FacedVolume>();

                for (int idx = 0; idx < chunkState.volumizedWorld.Count; idx++)
                {
                    Volume volume = chunkState.volumizedWorld[idx];
                    Face excludedFaces = ObstructedFaces(volume, opaqueBlocks);

                    int hiddenFaces = NumberOfSetBits.Count((int)excludedFaces);
                    Debug.Assert(hiddenFaces >= 0 && hiddenFaces <= 6);
                    int visibleFaces = 6 - hiddenFaces;
                    worldState.hiddenFaces += hiddenFaces;
                    worldState.visibleFaces += visibleFaces;

                    chunkState.facedVolumes.Add(new FacedVolume(volume, excludedFaces));
                }
            }
        }

        private static Face ObstructedFaces(Volume volume, HashSet<CoordinateInt> blockCheck)
        {
            Face faces = Face.None;
            if (ObstructedScanYZ(volume.Coord, volume.ScaleX, volume.ScaleY, volume.ScaleZ, blockCheck))
                faces |= Face.PositiveX;
            if (ObstructedScanXZ(volume.Coord, volume.ScaleX, volume.ScaleY, volume.ScaleZ, blockCheck))
                faces |= Face.PositiveY;
            if (ObstructedScanXY(volume.Coord, volume.ScaleX, volume.ScaleY, volume.ScaleZ, blockCheck))
                faces |= Face.PositiveZ;

            if (ObstructedScanYZ(volume.Coord, - 1, volume.ScaleY, volume.ScaleZ, blockCheck))
                faces |= Face.NegativeX;
            if (ObstructedScanXZ(volume.Coord, volume.ScaleX, - 1, volume.ScaleZ, blockCheck))
                faces |= Face.NegativeY;
            if (ObstructedScanXY(volume.Coord, volume.ScaleX, volume.ScaleY, - 1, blockCheck))
                faces |= Face.NegativeZ;

            return faces;
        }

        private static bool ObstructedScanYZ(CoordinateInt coord, int offsetX, int height, int length, HashSet<CoordinateInt> blockCheck)
        {
            for (int dY = 0; dY < height; dY++)
                for (int dZ = 0; dZ < length; dZ++)
                    if (!blockCheck.Contains(coord.Offset(offsetX, dY, dZ))) return false;
            return true;
        }

        private static bool ObstructedScanXY(CoordinateInt coord, int width, int height, int offsetZ, HashSet<CoordinateInt> blockCheck)
        {
            for (int dX = 0; dX < width; dX++)
                for (int dY = 0; dY < height; dY++)
                    if (!blockCheck.Contains(coord.Offset(dX, dY, offsetZ))) return false;
            return true;
        }

        private static bool ObstructedScanXZ(CoordinateInt coord, int width, int offsetY, int length, HashSet<CoordinateInt> blockCheck)
        {
            for (int dX = 0; dX < width; dX++)
                for (int dZ = 0; dZ < length; dZ++)
                    if (!blockCheck.Contains(coord.Offset(dX, offsetY, dZ))) return false;
            return true;
        }
    }
}
