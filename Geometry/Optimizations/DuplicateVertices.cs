using System.Collections.Generic;

namespace MC2UE.Geometry.Optimizations
{
    static class DuplicateVertices
    {        
        public static int DetectAndErase(List<CoordinateDecimal> vertices, Dictionary<BlockFaceTexture, List<TexturedFace>> texturedFaces)
        {            
            List<int> translationIndex = new List<int>();
            List<int> translationOffset = new List<int>();
            List<bool> translationLocked = new List<bool>();

            for (int idx = 0; idx < vertices.Count; idx++)
            {
                translationIndex.Add(idx);
                translationOffset.Add(0);
                translationLocked.Add(false);
            }

            for (int transIdx = 0; transIdx < translationIndex.Count; transIdx++)
                if (!translationLocked[transIdx])
                    for (int checkIdx = transIdx + 1; checkIdx < vertices.Count; checkIdx++)
                        if (vertices[transIdx] == vertices[checkIdx])
                        {
                            translationIndex[checkIdx] = transIdx;
                            translationOffset[checkIdx] = translationOffset[transIdx];
                            translationLocked[checkIdx] = true;

                            for (int decrementIdx = checkIdx + 1; decrementIdx < vertices.Count; decrementIdx++)
                                if (!translationLocked[decrementIdx])
                                    translationOffset[decrementIdx] -= 1;
                        }

            foreach (KeyValuePair<BlockFaceTexture, List<TexturedFace>> pair in texturedFaces)
                for (int idx = 0; idx < pair.Value.Count; idx++)
                    UpdateFaceVertices(translationIndex, translationOffset, ref pair.Value[idx].faceVertices);

//            foreach (KeyValuePair<string, FaceVertices[]> pair in collisionBoxes)
//                for (int idx = 0; idx < pair.Value.Length; idx++)
//                    UpdateFaceVertices(translationIndex, translationOffset, ref pair.Value[idx]);

            int duplicatesRemoved = 0;
            for (int idx = 0; idx < translationLocked.Count; idx++)
                if (translationLocked[idx])
                {
                    vertices.RemoveAt(idx - duplicatesRemoved);
                    duplicatesRemoved++;
                }

            return duplicatesRemoved;
        }

        private static void UpdateFaceVertices(List<int> translationIndex, List<int> translationOffset, ref FaceVertices faceVertices)
        {
            faceVertices.index1 = translationIndex[faceVertices.index1] + translationOffset[faceVertices.index1];
            faceVertices.index2 = translationIndex[faceVertices.index2] + translationOffset[faceVertices.index2];
            faceVertices.index3 = translationIndex[faceVertices.index3] + translationOffset[faceVertices.index3];
            faceVertices.index4 = translationIndex[faceVertices.index4] + translationOffset[faceVertices.index4];
        }
    }
}
