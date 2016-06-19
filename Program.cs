using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using NbtToObj.Geometry;
using NbtToObj.Optimizer;
using NbtToObj.Wavefront;
using NbtToObj.Minecraft;
using NbtToObj.Gui;
using NbtToObj.Helpers;

namespace NbtToObj
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: MC2UE <world-directory> <output-directory>");
                return;
            }

            Dictionary<CoordinateInt, Block> visibleAndInvisibleBlocks;
            {
                Anvil anvil = new Anvil(args[0]);
                visibleAndInvisibleBlocks = CompositeBlockConverter.Convert(anvil.blocks);
            }

            Dictionary<CoordinateInt, Block> visibleBlocks = InvisibleBlockDetection.DetectAndFilterInvisible(visibleAndInvisibleBlocks);
            Console.WriteLine("{0} invisible bricks.", MapPartition.invisibleBricks.Count);

            List<MapPartition> mapPartitions = new List<MapPartition>();
            PhysicalMaterialPartitioner mapPartitioner = new PhysicalMaterialPartitioner(visibleBlocks);
            
            foreach (KeyValuePair<PhysicalMaterial, Dictionary<CoordinateInt, Block>> pair in mapPartitioner)
                mapPartitions.Add(new MapPartition() { physicalMaterial = pair.Key, rawBlocks = pair.Value });
            foreach (MapPartition mapPartition in mapPartitions)
                mapPartition.organizedBlocks = BlockPartitioner.Organize(mapPartition.rawBlocks);

            Console.WriteLine("{0} visible bricks:", mapPartitions.Sum(s => s.rawBlocks.Count));

            foreach (MapPartition mapPartition in mapPartitions)
            {
                Console.WriteLine($"  {mapPartition.physicalMaterial.ToString()} ({mapPartition.rawBlocks.Count} total)");
                foreach (KeyValuePair<Block, HashSet<CoordinateInt>> pair in mapPartition.organizedBlocks)
                    Console.WriteLine($"    {pair.Key.ToString()}: {pair.Value.Count} blocks");
            }

            Console.WriteLine("Voluming blocks (slow):");
            int volumedBlocks = 0;
            int totalBlocks = mapPartitions.Sum(s => s.rawBlocks.Count);

            foreach (MapPartition mapPartition in mapPartitions)
                foreach (KeyValuePair<Block, HashSet<CoordinateInt>> organizedBlocks in mapPartition.organizedBlocks)
                {
                    Volume largestVolume;

                    do
                    {
                        Console.Write($"\r  {volumedBlocks}/{totalBlocks}");
                        largestVolume = LargestVolumeExtractor.ExtractAndSubtract(organizedBlocks.Value);
                        volumedBlocks += largestVolume.TotalVolume;
                        mapPartition.volumizedWorld.Add(organizedBlocks.Key, largestVolume);
                    }
                    while (largestVolume.TotalVolume > 0);
                }

            /* Compile a list of all the opaque blocks on the map. */
            HashSet<CoordinateInt> opaqueBlocks = new HashSet<CoordinateInt>();
            foreach (KeyValuePair<CoordinateInt, Block> pair in visibleAndInvisibleBlocks)
                if (pair.Value.IsOpaque)
                    opaqueBlocks.Add(pair.Key);

            foreach (MapPartition mapPartition in mapPartitions)
            {
                Console.WriteLine("\nIdentifying interior faces.");
                mapPartition.facedVolumes = UnobstructedFaces.DetectHiddenFaces(mapPartition, opaqueBlocks);                
            }

            int totalVisibleFaces = mapPartitions.Sum(s => s.visibleFaces);
            int totalHiddenFaces = mapPartitions.Sum(s => s.hiddenFaces);

            Console.WriteLine($"{totalVisibleFaces} visible faces, {totalHiddenFaces} hidden faces");

            /* Build the textured faces from the volumes. */
            foreach (MapPartition mapPartition in mapPartitions)
                foreach (KeyValuePair<Block, List<FacedVolume>> pair in mapPartition.facedVolumes)
                {
                    List<FacedVolume> volumes = pair.Value;
                    for (int idx = 0; idx < volumes.Count; idx++)
                    {
                        FacedVolume facedVolume = volumes[idx];
                        Iterators.FacesInVolume(mapPartition.vertices.Count, facedVolume.excludedFaces, (Face face, FaceVertices faceVertices) =>
                            { AppendTexturedFaces(mapPartition.texturedFaces, mapPartition.textureCoordinates, pair.Key, facedVolume.volume, face, faceVertices); });
                        Iterators.VerticesInVolume(volumes[idx].volume,
                            (CoordinateDecimal a) => { mapPartition.vertices.Add(a); });
                    }
                }
            Console.WriteLine(mapPartitions.Sum(s => s.textureCoordinates.mappingList.Count) + " unique texture coordinates.");

            int duplicatesRemoved = 0;
            foreach (MapPartition mapPartition in mapPartitions)
            {
                /* Delete duplicate vertices before adding collision UBXs because the UBX vertices will all be unique. */
                duplicatesRemoved += DuplicateVertices.DetectAndErase(mapPartition.vertices, mapPartition.texturedFaces);                
            }
            Console.WriteLine(duplicatesRemoved + " duplicate vertices removed.");

            foreach (MapPartition mapPartition in mapPartitions)
            {
                foreach (KeyValuePair<Block, List<FacedVolume>> pair in mapPartition.facedVolumes)
                {
                    List<FacedVolume> volumes = pair.Value;
                    for (int idx = 0; idx < volumes.Count; idx++)
                        MakeCollisionUBX("UBX_" + pair.Key.ToString() + string.Format("_{0:00}", idx), 
                            volumes[idx].volume, mapPartition.vertices, mapPartition.collisionBoxes);
                }
            }

            foreach (MapPartition mapPartition in mapPartitions)
            {
                /* Export the geometry to Wavefront's OBJ format. */
                WavefrontObj objFile = new WavefrontObj(mapPartition.vertices, mapPartition.collisionBoxes, 
                    mapPartition.texturedFaces, mapPartition.textureCoordinates, mapPartition.facedVolumes);

                /* Save the OBJ file to the specified destination. */
                File.WriteAllText(Path.Combine(args[1], $"World.{mapPartition.physicalMaterial.ToString()}.obj"), objFile.ToString());
            }
        }

        private static void MakeCollisionUBX(string name, Volume volume, List<CoordinateDecimal> vertices,
            Dictionary<string, List<FaceVertices>> collisionBoxes)
        {
            const decimal collisionPadding = 0.05m;
            List<FaceVertices> listOfFaceVertices = new List<FaceVertices>();

            Iterators.FacesInVolume(vertices.Count, Face.None, (Face face, FaceVertices faceVertices) =>
                { listOfFaceVertices.Add(faceVertices); });
                    
            Iterators.VerticesInVolume(((CoordinateDecimal)volume.Coord).Offset(
                collisionPadding, collisionPadding, collisionPadding),
                volume.ScaleX - 2*collisionPadding, 
                volume.ScaleY - 2*collisionPadding,
                volume.ScaleZ - 2*collisionPadding,
                (CoordinateDecimal a) => { vertices.Add(a); });

            collisionBoxes.Add(name, listOfFaceVertices);
        }

        private static void AppendTexturedFaces(Dictionary<BlockFaceTexture, List<TexturedFace>> texturedFaces, 
            TextureCoordinateDictionary textureCoordinates,
            Block blockType, Volume volume, Face face, FaceVertices faceVertices)
        {
            BlockFaceTexture blockFaceTexture = blockType.GetFaceTexture(face);

            TexturedFace texturedFace = new TexturedFace(volume, face, faceVertices);
            List<TexturedFace> texturedFacesList;
            if (texturedFaces.TryGetValue(blockFaceTexture, out texturedFacesList))
                texturedFacesList.Add(texturedFace);
            else
            {
                texturedFacesList = new List<TexturedFace>();
                texturedFacesList.Add(texturedFace);
                texturedFaces.Add(blockFaceTexture, texturedFacesList);
            }
            textureCoordinates.EnsureExists(texturedFace.textureMapping);
        }

        private static void AppendVertices(List<CoordinateDecimal> vertices, List<FacedVolume> volumes)
        {
            for (int idx = 0; idx < volumes.Count; idx++)
            {
                Volume volume = volumes[idx].volume;
                vertices.Add(new CoordinateDecimal(
                    volume.Coord.X, volume.Coord.Y, volume.Coord.Z));
            }
        }
    }
}
