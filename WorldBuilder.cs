using System;
using System.IO;
using System.Collections.Generic;

namespace MC2Blender
{
    class WorldBuilder
    {
        private static BlockID[] TransparentBlocks = new BlockID[] { BlockID.Leaves };
        private readonly PythonScript script = new PythonScript();

        public WorldBuilder(MinecraftWorld world, string outputPath)
        {
            Dictionary<Coordinate, MinecraftBlock> rawWorld = world.blocks;

            /* We need to identify any bricks that are hidden from vision. */
            Console.WriteLine("Identifying invisible blocks");
            HashSet<Coordinate> invisibleBricks = new HashSet<Coordinate>();
            foreach (KeyValuePair<Coordinate, MinecraftBlock> pair in rawWorld)
                if (IsInvisible(pair.Key, rawWorld))
                    invisibleBricks.Add(pair.Key);
            foreach (Coordinate coord in invisibleBricks)
                rawWorld.Remove(coord);
            Console.WriteLine("Identified {0} invisible bricks.", invisibleBricks.Count);

            /* Before we can start expanding cubes, we need to organize by block type. */
            int cubeNum = 0;
            Dictionary<MinecraftBlock, HashSet<Coordinate>> organizedWorld = OrganizeRawWorld(rawWorld);
            foreach (KeyValuePair<MinecraftBlock, HashSet<Coordinate>> pair in organizedWorld)
            {
                List<Volume> volumes = new List<Volume>(new BoxExtractor(pair.Value, invisibleBricks));
                //foreach (Volume volume in volumes)
                //    script.AddBlock(volume.Coord.X, volume.Coord.Y, volume.Coord.Z,
                //        "Cube" + ++cubeNum, volume.Width, volume.Height, volume.Length);
                //script.CreateCollisionBoxes("dirt", volumes);
                script.CreateBoxes("dirt", volumes);
            }
            Console.Write("\n");

            File.WriteAllText(outputPath, script.ToString());
        }

        private static Dictionary<MinecraftBlock, HashSet<Coordinate>> OrganizeRawWorld(Dictionary<Coordinate, MinecraftBlock> rawWorld)
        {
            Dictionary<MinecraftBlock, HashSet<Coordinate>> organizedWorld = new Dictionary<MinecraftBlock, HashSet<Coordinate>>();
            foreach (KeyValuePair<Coordinate, MinecraftBlock> pair in rawWorld)
            {
                HashSet<Coordinate> coordinates;
                if (organizedWorld.TryGetValue(pair.Value, out coordinates))
                    coordinates.Add(pair.Key);
                else
                {
                    coordinates = new HashSet<Coordinate>();
                    coordinates.Add(pair.Key);
                    organizedWorld.Add(pair.Value, coordinates);
                }
            }
            return organizedWorld;
        }

        private static bool IsInvisible(Coordinate coord, Dictionary<Coordinate, MinecraftBlock> rawWorld)
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

        private static bool OpaqueBrickAt(Coordinate coord, Dictionary<Coordinate, MinecraftBlock> rawWorld)
        {
            MinecraftBlock blockType;
            return (rawWorld.TryGetValue(coord, out blockType)) && Array.IndexOf(TransparentBlocks, blockType.id) == -1;
        }
    }
}
