using NbtToObj.Geometry;
using NbtToObj.Minecraft;
using System.Collections.Generic;

namespace NbtToObj.Optimizer
{
    static class HiddenFaces
    {
        public static int totalHiddenFaces = 0;

        public static Dictionary<Block, List<FacedVolume>> DetectHiddenFaces(Dictionary<Block, List<Volume>> volumizedWorld,
            Dictionary<CoordinateInt, Block> rawBlocks)
        {
            HashSet<CoordinateInt> opaqueBlocks = new HashSet<CoordinateInt>();
            foreach (KeyValuePair<CoordinateInt, Block> pair in rawBlocks)
                if (pair.Value.IsOpaque)
                    opaqueBlocks.Add(pair.Key);

            Dictionary<Block, List<FacedVolume>> facedVolumes = new Dictionary<Block, List<FacedVolume>>();
            foreach (KeyValuePair<Block, List<Volume>> pair in volumizedWorld)
            {
                List<FacedVolume> faceVolumeList = new List<FacedVolume>();
                for (int idx = 0; idx < pair.Value.Count; idx++)
                {
                    Volume volume = pair.Value[idx];
                    Face excludedFaces = ObstructedFaces(volume, opaqueBlocks);
                    faceVolumeList.Add(new FacedVolume(volume, excludedFaces));
                }
                facedVolumes.Add(pair.Key, faceVolumeList);
            }

            return facedVolumes;
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
            totalHiddenFaces++;
            return true;
        }

        private static bool ObstructedScanXY(CoordinateInt coord, int width, int height, int offsetZ, HashSet<CoordinateInt> blockCheck)
        {
            for (int dX = 0; dX < width; dX++)
                for (int dY = 0; dY < height; dY++)
                    if (!blockCheck.Contains(coord.Offset(dX, dY, offsetZ))) return false;
            totalHiddenFaces++;
            return true;
        }

        private static bool ObstructedScanXZ(CoordinateInt coord, int width, int offsetY, int length, HashSet<CoordinateInt> blockCheck)
        {
            for (int dX = 0; dX < width; dX++)
                for (int dZ = 0; dZ < length; dZ++)
                    if (!blockCheck.Contains(coord.Offset(dX, offsetY, dZ))) return false;
            totalHiddenFaces++;
            return true;
        }
    }
}
