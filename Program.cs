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
                Console.WriteLine("Usage: MC2UE <world-directory> <output>");
                return;
            }

            Anvil anvil = new Anvil(args[0]);

            Dictionary<CoordinateInt, Block> visibleBlocks = InvisibleBlockDetection.DetectAndFilterInvisible(anvil.blocks);
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
            foreach (KeyValuePair<CoordinateInt, Block> pair in anvil.blocks)
                if (pair.Value.IsOpaque)
                    opaqueBlocks.Add(pair.Key);

            foreach (MapPartition mapPartition in mapPartitions)
            {
                Console.WriteLine("Identifying interior faces.");
                MultiValueDictionary<Block, FacedVolume> facedVolumizedWorld = UnobstructedFaces.DetectHiddenFaces(mapPartition, opaqueBlocks);                
            }

            int totalVisibleFaces = mapPartitions.Sum(s => s.visibleFaces);
            int totalHiddenFaces = mapPartitions.Sum(s => s.hiddenFaces);

            Console.WriteLine($"{totalVisibleFaces} visible faces, {totalHiddenFaces} hidden faces");

            //    ProcessBlocks(pair.Key, pair.Value);
            //ProcessBlocks(args[1], anvil.blocks);
        }

        private static void ProcessBlocks(string outputPath, Dictionary<CoordinateInt, Block> rawBlocks)
        {
            /* Scan for interior faces that we can remove. */
            //UnobstructedFaces.totalHiddenFaces = 0;
            Console.WriteLine("Identifying interior faces.");
            Dictionary<Block, List<FacedVolume>> facedVolumizedWorld = null;// UnobstructedFaces.DetectHiddenFaces(volumizedWorld, rawBlocks);
            //Console.WriteLine("Identified {0} interior faces.", UnobstructedFaces.totalHiddenFaces);

            /* Storage for the actual 3D geometry. */
            List<CoordinateDecimal> vertices = new List<CoordinateDecimal>();
            Dictionary<string, List<FaceVertices>> collisionBoxes = new Dictionary<string, List<FaceVertices>>();
            Dictionary<BlockFaceTexture, List<TexturedFace>> texturedFaces = new Dictionary<BlockFaceTexture, List<TexturedFace>>();
            TextureCoordinateDictionary textureCoordinates = new TextureCoordinateDictionary();

            /* Build the textured faces from the volumes. */
            foreach (KeyValuePair<Block, List<FacedVolume>> pair in facedVolumizedWorld)
            {
                List<FacedVolume> volumes = pair.Value;
                for (int idx = 0; idx < volumes.Count; idx++)
                {
                    FacedVolume facedVolume = volumes[idx];
                    Iterators.FacesInVolume(vertices.Count, facedVolume.excludedFaces, (Face face, FaceVertices faceVertices) =>
                        { AppendTexturedFaces(texturedFaces, textureCoordinates, pair.Key, facedVolume.volume, face, faceVertices); });
                    Iterators.VerticesInVolume(volumes[idx].volume,
                        (CoordinateDecimal a) => { vertices.Add(a); });
                }
            }
            Console.WriteLine(textureCoordinates.mappingList.Count + " unique texture coordinates.");

            /* Delete duplicate vertices before adding collision UBXs because the UBX vertices will all be unique. */
            Console.WriteLine("Detecting duplicate vertices.");
            int duplicatesRemoved = DuplicateVertices.DetectAndErase(vertices, texturedFaces);
            Console.WriteLine(duplicatesRemoved + " duplicate vertices removed.");

            Console.WriteLine("Generate UBX collision volumes.");
            foreach (KeyValuePair<Block, List<FacedVolume>> pair in facedVolumizedWorld)
            {
                List<FacedVolume> volumes = pair.Value;
                for (int idx = 0; idx < volumes.Count; idx++)
                    #warning Potential for collision meshes to be far from visual mesh!
                    MakeCollisionUBX("UBX_" + pair.Key.ToString() + string.Format("_{0:00}", idx), volumes[idx].volume, vertices, collisionBoxes);
            }

            /* Export the geometry to Wavefront's OBJ format. */
            WavefrontObj objFile = new WavefrontObj(vertices, collisionBoxes, texturedFaces, textureCoordinates, facedVolumizedWorld);

            /* Save the OBJ file to the specified destination. */
            File.WriteAllText(outputPath, objFile.ToString());
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
