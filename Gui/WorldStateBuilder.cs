using NbtToObj.Geometry;
using NbtToObj.Helpers;
using NbtToObj.Minecraft;
using NbtToObj.Optimizer;
using NbtToObj.Wavefront;
using System;
using System.Collections.Generic;

namespace NbtToObj.Gui
{
    static class WorldStateBuilder
    {
        public static WorldState Build(string anvilPath, string outputName)
        {
            WorldState worldState = new WorldState()
            {
                outputName = outputName
            };
            {
                Anvil anvil = new Anvil(anvilPath);
                worldState.visibleAndInvisibleBlocks = CompositeBlockConverter.Convert(anvil.blocks);
            }
            InvisibleBlockDetection.DetectAndFilterInvisible(worldState);

            Console.WriteLine("{0} invisible bricks.", worldState.invisibleBricks.Count);

            BlockPartitioner.Organize(worldState);

            Console.WriteLine("{0} visible bricks:", worldState.visibleBlocks.Count);
            foreach (KeyValuePair<Block, HashSet<CoordinateInt>> pair in worldState.organizedBlocks)
                Console.WriteLine($"  {pair.Key.ToString()}: {pair.Value.Count} blocks");

            Console.WriteLine("Voluming blocks (slow):");
            int volumedBlocks = 0;
            int totalBlocks = worldState.visibleBlocks.Count;

            foreach (KeyValuePair<Block, HashSet<CoordinateInt>> organizedBlocks in worldState.organizedBlocks)
            {
                Volume largestVolume;

                do
                {
                    Console.Write($"\r  {volumedBlocks}/{totalBlocks}");
                    largestVolume = LargestVolumeExtractor.ExtractAndSubtract(organizedBlocks.Value);
                    volumedBlocks += largestVolume.TotalVolume;
                    worldState.volumizedWorld.Add(organizedBlocks.Key, largestVolume);
                }
                while (largestVolume.TotalVolume > 0);
            }

            /* Compile a list of all the opaque blocks on the map. */
            HashSet<CoordinateInt> opaqueBlocks = new HashSet<CoordinateInt>();
            foreach (KeyValuePair<CoordinateInt, Block> pair in worldState.visibleAndInvisibleBlocks)
                if (pair.Value.IsOpaque)
                    opaqueBlocks.Add(pair.Key);

            Console.WriteLine("\nIdentifying interior faces.");
            worldState.facedVolumes = UnobstructedFaces.DetectHiddenFaces(worldState, opaqueBlocks);

            Console.WriteLine($"{worldState.visibleFaces} visible faces, {worldState.hiddenFaces} hidden faces");

            /* Build the textured faces from the volumes. */
            foreach (KeyValuePair<Block, List<FacedVolume>> pair in worldState.facedVolumes)
            {
                List<FacedVolume> volumes = pair.Value;
                for (int idx = 0; idx < volumes.Count; idx++)
                {                    
                    FacedVolume facedVolume = volumes[idx];
                    worldState.texturedFacesOfFacedVolume.Add(facedVolume, new MultiValueDictionary<BlockFaceTexture, TexturedFace>());

                    Iterators.FacesInVolume(worldState.vertices.Count, facedVolume.excludedFaces, (Face face, FaceVertices faceVertices) =>
                    {
                        BlockFaceTexture blockFaceTexture = pair.Key.GetFaceTexture(face);
                        TexturedFace texturedFace = new TexturedFace(facedVolume.volume, face, faceVertices);
                        worldState.texturedFaces.Add(blockFaceTexture, texturedFace);
                        worldState.textureCoordinates.EnsureExists(texturedFace.textureCoord, texturedFace.textureSize);
                        worldState.texturedFacesOfFacedVolume[facedVolume].Add(blockFaceTexture, texturedFace);
                    });
                    Iterators.VerticesInVolume(volumes[idx].volume,
                        (CoordinateDecimal a) => { worldState.vertices.Add(a); });
                }
            }
            Console.WriteLine(worldState.textureCoordinates.mappingDict.Count + " unique texture coordinates.");

            /* Delete duplicate vertices before adding collision UBXs because the UBX vertices will all be unique. */
            int duplicatesRemoved = DuplicateVertices.DetectAndErase(worldState.vertices, worldState.texturedFaces);
            Console.WriteLine(duplicatesRemoved + " duplicate vertices removed.");

            return worldState;
        }
    }
}
