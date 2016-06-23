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
            Console.WriteLine("{0} visible bricks:", worldState.visibleBlocks.Count);

            ChunkStateBuilder.Partition(worldState);

            //foreach (KeyValuePair<Block, HashSet<CoordinateInt>> pair in worldState.organizedBlocks)
                //Console.WriteLine($"  {pair.Key.ToString()}: {pair.Value.Count} blocks");

            Console.WriteLine("Voluming blocks (slow):");
            int volumedBlocks = 0;
            int totalBlocks = worldState.visibleBlocks.Count;

            foreach (ChunkState chunkState in worldState.chunks)
            {
                Volume largestVolume;

                do
                {
                    Console.Write($"\r  {volumedBlocks}/{totalBlocks}");
                    largestVolume = LargestVolumeExtractor.ExtractAndSubtract(chunkState.visibleBlocks);
                    volumedBlocks += largestVolume.TotalVolume;
                    chunkState.volumizedWorld.Add(largestVolume);
                }
                while (largestVolume.TotalVolume > 0);
            }

            /* Compile a list of all the opaque blocks on the map. */
            HashSet<CoordinateInt> opaqueBlocks = new HashSet<CoordinateInt>();
            foreach (KeyValuePair<CoordinateInt, Block> pair in worldState.visibleAndInvisibleBlocks)
                if (pair.Value.IsOpaque)
                    opaqueBlocks.Add(pair.Key);

            Console.WriteLine("\nIdentifying interior faces.");
            UnobstructedFaces.DetectHiddenFaces(worldState, opaqueBlocks);

            Console.WriteLine($"{worldState.visibleFaces} visible faces, {worldState.hiddenFaces} hidden faces");

            /* Build the textured faces from the volumes. */
            foreach (ChunkState chunkState in worldState.chunks)
            {
                List<FacedVolume> volumes = chunkState.facedVolumes;
                for (int idx = 0; idx < volumes.Count; idx++)
                {
                    FacedVolume facedVolume = volumes[idx];
                    worldState.texturedFacesOfFacedVolume.Add(facedVolume, new MultiValueDictionary<BlockFaceTexture, TexturedFace>());

                    Iterators.FacesInVolume(worldState.vertices.Count, facedVolume.excludedFaces, (Face face, FaceVertices faceVertices) =>
                    {
                        BlockFaceTexture blockFaceTexture = chunkState.blockType.GetFaceTexture(face);
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
