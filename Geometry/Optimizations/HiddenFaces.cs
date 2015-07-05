using System.Collections.Generic;

namespace MC2FBX
{
    static class HiddenFaces
    {
        public static int totalHiddenFaces = 0;

        public static Dictionary<BlockType, List<FacedVolume>> DetectHiddenFaces(Dictionary<BlockType, List<Volume>> volumizedWorld,
            Dictionary<Coordinate, BlockType> rawBlocks)
        {
            HashSet<Coordinate> opaqueBlocks = new HashSet<Coordinate>();
            foreach (KeyValuePair<Coordinate, BlockType> pair in rawBlocks)
                if (pair.Value.IsOpaque)
                    opaqueBlocks.Add(pair.Key);

            Dictionary<BlockType, List<FacedVolume>> facedVolumes = new Dictionary<BlockType, List<FacedVolume>>();
            foreach (KeyValuePair<BlockType, List<Volume>> pair in volumizedWorld)
            {
                List<FacedVolume> faceVolumeList = new List<FacedVolume>();
                for (int idx = 0; idx < pair.Value.Count; idx++)
                {
                    Volume volume = pair.Value[idx];
                    Faces faces = Faces.All ^ ObstructedFaces(volume, opaqueBlocks);
                    faceVolumeList.Add(new FacedVolume(volume, faces));
                }
                facedVolumes.Add(pair.Key, faceVolumeList);
            }

            return facedVolumes;
        }

        private static Faces ObstructedFaces(Volume volume, HashSet<Coordinate> blockCheck)
        {
            Faces faces = Faces.None;
            if (ObstructedScanXZ(volume.Coord, volume.Width, volume.Height, volume.Length, blockCheck))
                faces |= Faces.PositiveX;
            if (ObstructedScanYZ(volume.Coord, volume.Width, volume.Height, volume.Length, blockCheck))
                faces |= Faces.PositiveY;
            if (ObstructedScanXY(volume.Coord, volume.Width, volume.Height, volume.Length, blockCheck))
                faces |= Faces.PositiveZ;

            if (ObstructedScanXZ(volume.Coord, volume.Width, volume.Coord.Y - 1, volume.Length, blockCheck))
                faces |= Faces.NegativeX;
            if (ObstructedScanYZ(volume.Coord, volume.Coord.X - 1, volume.Height, volume.Length, blockCheck))
                faces |= Faces.NegativeY;
            if (ObstructedScanXY(volume.Coord, volume.Width, volume.Height, volume.Coord.Z - 1, blockCheck))
                faces |= Faces.NegativeZ;

            return faces;
        }

        private static bool ObstructedScanYZ(Coordinate coord, int offsetX, int height, int length, HashSet<Coordinate> blockCheck)
        {
            for (int dY = 0; dY < height; dY++)
                for (int dZ = 0; dZ < length; dZ++)
                    if (!blockCheck.Contains(coord.Offset(offsetX, dY, dZ))) return false;
            totalHiddenFaces++;
            return true;
        }

        private static bool ObstructedScanXY(Coordinate coord, int width, int height, int offsetZ, HashSet<Coordinate> blockCheck)
        {
            for (int dX = 0; dX < width; dX++)
                for (int dY = 0; dY < height; dY++)
                    if (!blockCheck.Contains(coord.Offset(dX, dY, offsetZ))) return false;
            totalHiddenFaces++;
            return true;
        }

        private static bool ObstructedScanXZ(Coordinate coord, int width, int offsetY, int length, HashSet<Coordinate> blockCheck)
        {
            for (int dX = 0; dX < width; dX++)
                for (int dZ = 0; dZ < length; dZ++)
                    if (!blockCheck.Contains(coord.Offset(dX, offsetY, dZ))) return false;
            totalHiddenFaces++;
            return true;
        }
    }
}
