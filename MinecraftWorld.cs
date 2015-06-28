using System;
using System.Collections.Generic;
using fNbt;
using System.IO;

namespace MC2Blender
{
    class MinecraftWorld
    {
        public readonly Dictionary<Coordinate, MinecraftBlock> blocks = new Dictionary<Coordinate, MinecraftBlock>();
        public readonly int boundaryWidth;
        public readonly int boundaryHeight;
        public readonly int boundaryLength;  

        private int offsetX;
        private int offsetY;
        private int offsetZ;       

        private readonly Coordinate spawnLocation;
        private readonly Dictionary<Coordinate, MinecraftBlock[]> chunks = new Dictionary<Coordinate, MinecraftBlock[]>();
        private const int sectorSize = 1024 * 4;
        private const BlockID matchType = BlockID.Wool;
        private const int matchData = 14; //red

        private Bounds ScanForBounds()
        {
            Console.WriteLine("Scanning {0} chunks for bounds.", chunks.Count);
            Bounds bounds = new Bounds();
            int matchesFound = 0;
            foreach (KeyValuePair<Coordinate, MinecraftBlock[]> pair in chunks)
            {
                Coordinate chunkCoord = pair.Key;
                MinecraftBlock[] blocks = pair.Value;
                for (int blockIdx = 0; blockIdx < blocks.Length; blockIdx++)
                    if (blocks[blockIdx].id == matchType && blocks[blockIdx].data == matchData)
                    {
                        matchesFound++;
                        int relativeX = blockIdx % 16;
                        int relativeZ = (blockIdx / 16) % 16;
                        int relativeY = blockIdx / (16 * 16);
                        bounds.AddPoint(chunkCoord.X * 16 + relativeX, chunkCoord.Y * 16 + relativeY, chunkCoord.Z * 16 + relativeZ);
                    }
            }
            bounds.Shrink(1);
            Console.WriteLine("Found {0} bounds markers.\nBoundary is {1}.", matchesFound, bounds);

            return bounds;
        }

        public int AbsoluteCoordinatesToIndex(int x, int y, int z)
        {
            return (z + offsetZ) * 16 * 16 + (y + offsetY) * 16 + (x + offsetX);
        }

        public MinecraftWorld(string path)
        {
            NbtTag levelDat = new NbtFile(Path.Combine(path, "level.dat")).RootTag["Data"];
            string levelName = levelDat["LevelName"].StringValue;
            spawnLocation = new Coordinate(
                levelDat["SpawnX"].IntValue,
                levelDat["SpawnY"].IntValue,
                levelDat["SpawnZ"].IntValue);
            string[] regionFiles = Directory.GetFiles(Path.Combine(path, "region"), "*.mca");
            for (int idx = 0; idx < regionFiles.Length; idx++)
                ParseRegion(regionFiles[idx]);
            Bounds bounds = ScanForBounds();
            boundaryWidth = bounds.Width;
            boundaryHeight = bounds.Height;
            boundaryLength = bounds.Length;
            offsetX = -bounds.minX;
            offsetY = -bounds.minY;
            offsetZ = -bounds.minZ;
            Console.WriteLine("Extracting blocks within boundary.");
            foreach (KeyValuePair<Coordinate, MinecraftBlock[]> pair in chunks)
            {
                Coordinate chunkCoord = pair.Key;
                if (bounds.ContainsChunk(chunkCoord))
                    for (int idx = 0; idx < pair.Value.Length; idx++)
                        if(BlockTypeIsSupported(pair.Value[idx].id))
                        {
                            int absoluteX = chunkCoord.X * 16 + idx % 16;
                            int absoluteY = chunkCoord.Y * 16 + (idx / 16) % 16;
                            int absoluteZ = chunkCoord.Z * 16 + idx / (16 * 16);
                            if (bounds.Contains(absoluteX, absoluteY, absoluteZ))
                                blocks[new Coordinate(absoluteX + offsetX, absoluteZ+offsetZ, absoluteY+offsetY)] = pair.Value[idx];
                        }
            }
        }

        private static bool BlockTypeIsSupported(BlockID blockID)
        {
            return
                blockID == BlockID.Dirt ||
                blockID == BlockID.Grass;
        }


        //private static int ToRelativeChunkPosition(int x, int y, int z)
        //{
        //    return z * 32 * 32 + y * 32 + x;
        //}

        private byte MaskTo4Bit(byte[] array, int idx)
        {
            if (idx % 2 == 0)
                return (byte)(array[idx / 2] & 0x0F);
            else
                return (byte)(array[idx / 2] >> 4);
        }

        private void ParseRegion(string regionPath)
        {
            string fileName = Path.GetFileName(regionPath);
            string[] regionCoords = Path.GetFileNameWithoutExtension(regionPath).Split('.');
            //int regionX = int.Parse(regionCoords[1]);
            //int regionZ = int.Parse(regionCoords[2]);
            int chunkCount = 0;

            using (BinaryReader binaryReader = new BinaryReader(File.Open(regionPath, FileMode.Open)))
            {
                List<LocationData> locationList = new List<LocationData>();
                for (int idx = 0; idx < 1024; idx++)
                {
                    int row = Endian.ToBig(binaryReader.ReadInt32());
                    int offset = (row >> 8);
                    byte sectorCount = (byte)(row & 0x000000FF);
                    if (offset != 0 || sectorCount != 0)
                        locationList.Add(new LocationData(idx % 32, idx / 32, offset, sectorCount));
                }

                for (int locationIdx = 0; locationIdx < locationList.Count; locationIdx++)
                {
                    binaryReader.BaseStream.Seek(sectorSize * locationList[locationIdx].offset, SeekOrigin.Begin);
                    int length = Endian.ToBig(binaryReader.ReadInt32());
                    byte compressionType = binaryReader.ReadByte();
                    byte[] dataBuffer = binaryReader.ReadBytes(length - 1);
                    NbtFile region = new NbtFile();
                    region.LoadFromBuffer(dataBuffer, 0, dataBuffer.Length, NbtCompression.ZLib);
                    NbtTag levelTag = region.RootTag["Level"];
                    int chunkX = levelTag["xPos"].IntValue;
                    int chunkZ = levelTag["zPos"].IntValue;
                    NbtList sections = levelTag["Sections"] as NbtList;
                    for (int sectionIdx = 0; sectionIdx < sections.Count; sectionIdx++)
                    {
                        MinecraftBlock[] blocks = new MinecraftBlock[16 * 16 * 16];
                        NbtTag sectionTag = sections[sectionIdx];
                        int offsetY = sectionTag["Y"].ByteValue;
                        byte[] blockIDs = sectionTag["Blocks"].ByteArrayValue;
                        byte[] blockData = sectionTag["Data"].ByteArrayValue;
                        for (int blockIdx = 0; blockIdx < blockIDs.Length; blockIdx++)
                            blocks[blockIdx] = new MinecraftBlock((BlockID)blockIDs[blockIdx], MaskTo4Bit(blockData, blockIdx));
                        Coordinate coord = new Coordinate(chunkX, offsetY, chunkZ);
                        chunks.Add(coord, blocks);
                        chunkCount++;
                    }
                }
                Console.WriteLine("Parsed {0} ({1} chunks)", fileName, chunkCount);
            }
        }

        private struct LocationData
        {
            public int x;
            public int z;
            public int offset;
            public byte sectorCount;
            public LocationData(int x, int z, int offset, byte sectorCount)
            {
                this.x = x;
                this.z = z;
                this.offset = offset;
                this.sectorCount = sectorCount;
            }
        }
    }
}
