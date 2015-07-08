using System;
using System.IO;
using System.Collections.Generic;

namespace MC2UE
{
    class WorldBuilder
    {
        private static BlockIdentifier[] TransparentBlocks = new BlockIdentifier[] { BlockIdentifier.Leaves };

        public WorldBuilder(MinecraftWorld world, string outputPath)
        {
            Dictionary<CoordinateInt, Block> rawBlocks = world.blocks;

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
            Console.WriteLine("Identifying interior faces.");
            Dictionary<Block, List<FacedVolume>> facedVolumizedWorld = HiddenFaces.DetectHiddenFaces(volumizedWorld, rawBlocks);
            Console.WriteLine("Identified {0} interior faces.", HiddenFaces.totalHiddenFaces);
            
            /* Export the geometry to Wavefront's OBJ format. */
            WavefrontObj objFile = new WavefrontObj(facedVolumizedWorld);

            //foreach (Volume volume in volumes)
            //    script.AddBlock(volume.Coord.X, volume.Coord.Y, volume.Coord.Z,
            //        "Cube" + ++cubeNum, volume.Width, volume.Height, volume.Length);
            //script.CreateCollisionBoxes("dirt", volumes);
            //script.CreateBoxes("dirt", volumes);
            Console.Write("\n");

            File.WriteAllText(outputPath, objFile.ToString());
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
