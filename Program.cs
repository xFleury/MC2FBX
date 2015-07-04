using System;

namespace MC2FBX
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: MC2Blender <world> <output>");
                return;
            }

            MinecraftWorld world = new MinecraftWorld(args[0]);
            WorldBuilder worldBuilder = new WorldBuilder(world, args[1]);
        }
    }
}
