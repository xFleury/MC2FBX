using System;
using System.IO;
using System.Collections.Generic;

namespace MC2Blender
{
    class WorldBuilder
    {
        private static BlockTypes[] TransparentBlocks = new BlockTypes[] { BlockTypes.Leaves };

        readonly Bounds bounds;
        readonly MinecraftWorld world;
        readonly PythonScript script = new PythonScript();

        public WorldBuilder(MinecraftWorld world, Bounds bounds, string outputPath)
        {
            this.world = world;
            this.bounds = bounds;

            Coordinate origin = bounds.GetOrigin();

            /* Populate the rawWorld with block data. */
            Console.WriteLine("Populating optimization grid");
            Dictionary<Coordinate, BlockTypes> rawWorld = new Dictionary<Coordinate, BlockTypes>();
            for (int x = bounds.minX; x <= bounds.maxX; ++x)
                for (int z = bounds.minZ; z <= bounds.maxZ; ++z)
                    for (int y = bounds.minY; y <= bounds.maxY; ++y)
                    {
                        int blockID = blockManager.GetID(x, y, z);
                        switch (blockID)
                        {
                            case BlockID.Dirt:
                            case BlockID.Grass:
                                rawWorld.Add(new Coordinate(x - origin.X, z - origin.Z, y - origin.Y), (BlockTypes)blockID);
                                break;
                        }
                    }

            /* We need to identify any bricks that are hidden from vision. */
            Console.WriteLine("Identifying invisible blocks");
            HashSet<Coordinate> invisibleBricks = new HashSet<Coordinate>();
            foreach (KeyValuePair<Coordinate, BlockTypes> pair in rawWorld)
                if (IsInvisible(pair.Key, rawWorld))
                    invisibleBricks.Add(pair.Key);

            foreach (Coordinate coord in invisibleBricks)
                rawWorld.Remove(coord);

            /* Before we can start expanding cubes, we need to organize by block type. */

            int cubeNum = 0;
            Dictionary<BlockTypes, HashSet<Coordinate>> organizedWorld = OrganizeRawWorld(rawWorld);
            foreach (KeyValuePair<BlockTypes, HashSet<Coordinate>> pair in organizedWorld)
                foreach (Volume volume in new BoxExtractor(pair.Value, invisibleBricks))
                    script.AddBlock(volume.Coord.X, volume.Coord.Y, volume.Coord.Z,
                        "Cube" + ++cubeNum, volume.Width, volume.Height, volume.Length);
            Console.Write("\n");

            File.WriteAllText(outputPath, script.ToString());
        }

        private static Dictionary<BlockTypes, HashSet<Coordinate>> OrganizeRawWorld(Dictionary<Coordinate, BlockTypes> rawWorld)
        {
            Dictionary<BlockTypes, HashSet<Coordinate>> organizedWorld = new Dictionary<BlockTypes, HashSet<Coordinate>>();
            foreach (KeyValuePair<Coordinate, BlockTypes> pair in rawWorld)
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

        private static bool IsInvisible(Coordinate coord, Dictionary<Coordinate, BlockTypes> rawWorld)
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

        private static bool OpaqueBrickAt(Coordinate coord, Dictionary<Coordinate, BlockTypes> rawWorld)
        {
            BlockTypes blockType;
            return rawWorld.TryGetValue(coord, out blockType) && Array.IndexOf(TransparentBlocks, blockType) == -1;
        }
    }
}
