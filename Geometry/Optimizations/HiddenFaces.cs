using System.Collections.Generic;

namespace MC2UE
{
    static class HiddenFaces
    {
        public static int totalHiddenFaces = 0;

        public static Dictionary<BlockType, List<FacedVolume>> DetectHiddenFaces(Dictionary<BlockType, List<Volume>> volumizedWorld,
            Dictionary<CoordinateInt, BlockType> rawBlocks)
        {
            HashSet<CoordinateInt> opaqueBlocks = new HashSet<CoordinateInt>();
            foreach (KeyValuePair<CoordinateInt, BlockType> pair in rawBlocks)
                if (pair.Value.IsOpaque)
                    opaqueBlocks.Add(pair.Key);

            Dictionary<BlockType, List<FacedVolume>> facedVolumes = new Dictionary<BlockType, List<FacedVolume>>();
            foreach (KeyValuePair<BlockType, List<Volume>> pair in volumizedWorld)
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

            if (ObstructedScanYZ(volume.Coord, volume.Coord.X - 1, volume.ScaleY, volume.ScaleZ, blockCheck))
                faces |= Face.NegativeX;
            if (ObstructedScanXZ(volume.Coord, volume.ScaleX, volume.Coord.Y - 1, volume.ScaleZ, blockCheck))
                faces |= Face.NegativeY;
            if (ObstructedScanXY(volume.Coord, volume.ScaleX, volume.ScaleY, volume.Coord.Z - 1, blockCheck))
                faces |= Face.NegativeZ;

            #warning Disabled hidden face detection until it is fixed.
            faces = Face.None;

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
