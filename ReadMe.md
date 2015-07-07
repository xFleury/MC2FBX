# Minecraft to Unreal Engine (MC2UE)

This tool is designed to assist modders in importing Minecraft worlds into Unreal Engine powered games. This tool optimizes the geometry for Unreal Engine and generates custom collision volumes.



# Limitations

##Face usage can be reduced further.
Interior faces get removed, but we can go a step further and start combining faces from neighbouring volumes that were not merged.
![](https://github.com/xFleury/minecraft-to-unreal/blob/master/wiki-images/nonIdealFaces.png)