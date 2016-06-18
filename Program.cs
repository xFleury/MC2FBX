using System;
using System.IO;
using System.Collections.Generic;
using NbtToObj.Geometry;
using NbtToObj.Optimizer;
using NbtToObj.Wavefront;
using NbtToObj.Minecraft;

namespace NbtToObj
{
    class Program
    {
        private static BlockIdentifier[] TransparentBlocks = new BlockIdentifier[] { BlockIdentifier.Leaves };

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: MC2UE <world-directory> <output>");
                return;
            }

            Anvil anvil = new Anvil(args[0]);
            Console.WriteLine("Defining new chunk boundaries.");
            //MapPartitioner mapPartitioner = new MapPartitioner(args[1]);
            //foreach (KeyValuePair<CoordinateInt, Block> pair in anvil.blocks)
            //    mapPartitioner.Add(pair.Key, pair.Value);
            //foreach (KeyValuePair<string, Dictionary<CoordinateInt, Block>> pair in mapPartitioner)
            //    ProcessBlocks(pair.Key, pair.Value);
            ProcessBlocks(args[1], anvil.blocks);
        }

        private static void ProcessBlocks(string outputPath, Dictionary<CoordinateInt, Block> rawBlocks)
        {
            /* We need to identify any bricks that are hidden from vision. */
            Console.WriteLine("Identifying invisible blocks.");
            HashSet<CoordinateInt> invisibleBricks = new HashSet<CoordinateInt>();
            foreach (KeyValuePair<CoordinateInt, Block> pair in rawBlocks)
                if (IsInvisible(pair.Key, rawBlocks))
                    invisibleBricks.Add(pair.Key);
            foreach (CoordinateInt coord in invisibleBricks)
                rawBlocks.Remove(coord);
            Console.WriteLine("Identified {0} invisible bricks.", invisibleBricks.Count);

            /* Before we can start expanding cubes, we need to organize by block type. */
            Console.WriteLine("Extracting largest volumes.");
            Dictionary<Block, List<Volume>> volumizedWorld = new Dictionary<Block, List<Volume>>();
            foreach (KeyValuePair<Block, HashSet<CoordinateInt>> pair in OrganizeRawBlocks(rawBlocks))
                volumizedWorld.Add(pair.Key, new List<Volume>(new LargestVolumeExtractor(pair.Value, invisibleBricks)));

            /* Scan for interior faces that we can remove. */
            HiddenFaces.totalHiddenFaces = 0;
            Console.WriteLine("Identifying interior faces.");
            Dictionary<Block, List<FacedVolume>> facedVolumizedWorld = HiddenFaces.DetectHiddenFaces(volumizedWorld, rawBlocks);
            Console.WriteLine("Identified {0} interior faces.", HiddenFaces.totalHiddenFaces);

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
            
        private static Dictionary<Block, HashSet<CoordinateInt>> OrganizeRawBlocks(Dictionary<CoordinateInt, Block> rawBlocks)
        {
            Dictionary<Block, HashSet<CoordinateInt>> organizedWorld = new Dictionary<Block, HashSet<CoordinateInt>>();
            foreach (KeyValuePair<CoordinateInt, Block> pair in rawBlocks)
            {
                HashSet<CoordinateInt> coordinates;
                if (organizedWorld.TryGetValue(pair.Value, out coordinates))
                    coordinates.Add(pair.Key);
                else
                {
                    coordinates = new HashSet<CoordinateInt>();
                    coordinates.Add(pair.Key);
                    organizedWorld.Add(pair.Value, coordinates);
                }
            }
            return organizedWorld;
        }

        private static bool IsInvisible(CoordinateInt coord, Dictionary<CoordinateInt, Block> rawWorld)
        {
            bool isInvisible =
                OpaqueBrickAt(coord.Offset(-1, 0, 0), rawWorld) &&
                OpaqueBrickAt(coord.Offset(1, 0, 0), rawWorld) &&
                OpaqueBrickAt(coord.Offset(0, -1, 0), rawWorld) &&
                OpaqueBrickAt(coord.Offset(0, 1, 0), rawWorld) &&
                OpaqueBrickAt(coord.Offset(0, 0, -1), rawWorld) &&
                OpaqueBrickAt(coord.Offset(0, 0, 1), rawWorld);
            return isInvisible;
        }

        private static bool OpaqueBrickAt(CoordinateInt coord, Dictionary<CoordinateInt, Block> rawWorld)
        {
            Block blockType;
            return (rawWorld.TryGetValue(coord, out blockType)) && Array.IndexOf(TransparentBlocks, blockType.id) == -1;
        }
    }
}
