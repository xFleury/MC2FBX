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
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: MC2UE <world-directory> <output-directory> <output-name>");
                return;
            }

            WorldState worldState = WorldStateBuilder.Build(args[0], args[2]);

            List<GroupState> groupStates = GroupStateBuilder.Partition(worldState);

            foreach (GroupState groupState in groupStates)
            {
                List<FacedVolume> volumes = groupState.facedVolumes;
                for (int idx = 0; idx < volumes.Count; idx++)
                    UnrealBoxCollision.MakeCollisionUBX("UBX_" + groupState.groupName + string.Format("_{0:00}", idx), 
                        volumes[idx].volume, worldState.vertices, worldState.collisionBoxes);
            }

            /* Export the geometry to Wavefront's OBJ format. */
            File.WriteAllText(Path.Combine(args[1], $"{args[2]}.obj"), WavefrontObj.Generate(worldState, groupStates));

            /* Export the unreal editor clipboard txt. */
            File.WriteAllText(Path.Combine(args[1], $"{args[2]}.txt"), UnrealEditorClipboard.GetClipoard(worldState, groupStates));

            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }
    }
}
