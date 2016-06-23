using NbtToObj.Geometry;
using NbtToObj.Minecraft;
using System.Linq;
using System.Collections.Generic;
using System;

namespace NbtToObj.Gui
{
    static class GroupStateBuilder
    {
        const int volumesPerGroup = 50;

        public static List<GroupState> Partition(WorldState worldState)
        {
            List<GroupState> groupStates = DivideVolumes(worldState);

            Console.WriteLine(groupStates.Count + " static meshes.");

            return groupStates;
        }

        private static List<GroupState> DivideVolumes(WorldState worldState)
        {
            int groupNum = 1;
            List<GroupState> groupStates = new List<GroupState>();

            foreach (KeyValuePair<Block, List<FacedVolume>> pair in worldState.facedVolumes)
            {
                for (int range = 0; range < pair.Value.Count; range += volumesPerGroup)
                {
                    GroupState groupState = new GroupState();
                    groupState.groupName = $"G{++groupNum}{pair.Key.ToString()}";
                    groupState.facedVolumes = pair.Value
                        .Skip(range)
                        .Take(volumesPerGroup)
                        .ToList();
                    groupStates.Add(groupState);
                }
            }
            return groupStates;
        }
    }
}
