using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC2Blender
{
    enum BlockID : byte
    {
        Air = 0,
        Stone = 1,
        Grass = 2,
        Dirt = 3,
        Cobblestone = 4,
        WoodPlank = 5,
        Sapling = 6,
        Bedrock = 7,
        Water = 8,
        StationaryWater = 9,
        Lava = 10,
        StationaryLava = 11,
        Sand = 12,
        Gravel = 13,
        GoldOre = 14,
        IronOre = 15,
        CoalOre = 16,
        Wood = 17,
        Leaves = 18,
        Sponge = 19,
        Glass = 20,
        LapisOre = 21,
        LapisBlock = 22,
        Dispenser = 23,
        Sandstone = 24,
        NoteBlock = 25,
        Bed = 26,
        PoweredRail = 27,
        DetectorRail = 28,
        StickyPiston = 29,
        Cobweb = 30,
        TallGrass = 31,
        DeadShrub = 32,
        Piston = 33,
        PistonHead = 34,
        Wool = 35,
        PistonMoving = 36,
        YellowFlower = 37,
        RedRose = 38,
        BrownMushroom = 39,
        RedMushroom = 40,
        GoldBlock = 41,
        IronBlock = 42,
        DoubleStoneSlab = 43,
        StoneSlab = 44,
        BrickBlock = 45,
        Tnt = 46,
        Bookshelf = 47,
        MossStone = 48,
        Obsidian = 49,
        Torch = 50,
        Fire = 51,
        MonsterSpawner = 52,
        WoodStairs = 53,
        Chest = 54,
        RedstoneWire = 55,
        DiamondOre = 56,
        DiamondBlock = 57,
        CraftingTable = 58,
        Crops = 59,
        Farmland = 60,
        Furnace = 61,
        BurningFurnace = 62,
        SignPost = 63,
        WoodDoor = 64,
        Ladder = 65,
        Rails = 66,
        CobblestoneStairs = 67,
        WallSign = 68,
        Lever = 69,
        StonePlate = 70,
        IronDoor = 71,
        WoodPlate = 72,
        RedstoneOre = 73,
        GlowingRedstoneOre = 74,
        RedstoneTorchOff = 75,
        RedstoneTorchOn = 76,
        StoneButton = 77,
        Snow = 78,
        Ice = 79,
        SnowBlock = 80,
        Cactus = 81,
        ClayBlock = 82,
        SugarCane = 83,
        Jukebox = 84,
        Fence = 85,
        Pumpkin = 86,
        Netherrack = 87,
        SoulSand = 88,
        GlowstoneBlock = 89,
        Portal = 90,
        JackOLantern = 91,
        CakeBlock = 92,
        RedstoneRepeaterOff = 93,
        RedstoneRepeaterOn = 94,
        LockedChest = 95,
        StainedGlass = 95,
        Trapdoor = 96,
        SilverfishStone = 97,
        StoneBrick = 98,
        HugeRedMushroom = 99,
        HugeBrownMushroom = 100,
        IronBars = 101,
        GlassPane = 102,
        Melon = 103,
        PumpkinStem = 104,
        MelonStem = 105,
        Vines = 106,
        FenceGate = 107,
        BrickStairs = 108,
        StoneBrickStairs = 109,
        Mycelium = 110,
        LillyPad = 111,
        NetherBrick = 112,
        NetherBrickFence = 113,
        NetherBrickStairs = 114,
        NetherWart = 115,
        EnchantmentTable = 116,
        BrewingStand = 117,
        Cauldron = 118,
        EndPortal = 119,
        EndPortalFrame = 120,
        EndStone = 121,
        DragonEgg = 122,
        RedstoneLampOff = 123,
        RedstoneLampOn = 124,
        DoubleWoodSlab = 125,
        WoodSlab = 126,
        CocoaPlant = 127,
        SandstoneStairs = 128,
        EmeraldOre = 129,
        EnderChest = 130,
        TripwireHook = 131,
        Tripwire = 132,
        EmeraldBlock = 133,
        SpruceWoodStairs = 134,
        BirchWoodStairs = 135,
        JungleWoodStairs = 136,
        CommandBlock = 137,
        BeaconBlock = 138,
        CobblestoneWall = 139,
        FlowerPot = 140,
        Carrots = 141,
        Potatoes = 142,
        WoodButton = 143,
        Heads = 144,
        Anvil = 145,
        TrappedChest = 146,
        WeightedPressurePlateLight = 147,
        WeightedPressurePlateHeavy = 148,
        RedstoneComparatorInactive = 149,
        RedstoneComparatorActive = 150,
        DaylightSensor = 151,
        RedstoneBlock = 152,
        NetherQuartzOre = 153,
        Hopper = 154,
        QuartzBlock = 155,
        QuartzStairs = 156,
        ActivatorRail = 157,
        Dropper = 158,
        StainedClay = 159,
        StainedGlassPane = 160,
        HayBlock = 170,
        Carpet = 171,
        HardenedClay = 172,
        CoalBlock = 173
    }

    struct MinecraftBlock : IEquatable<MinecraftBlock>
    {
        public BlockID id;
        public byte data;
        public MinecraftBlock(BlockID id, byte data)
        {
            this.id = id; this.data = data;
        }

        public bool Equals(MinecraftBlock other)
        {
            return id == other.id && data == other.data;
        }

        public override bool Equals(object obj)
        {
            if (obj is MinecraftBlock)
                return Equals((MinecraftBlock)obj);
            else return false;
        }

        public static bool operator ==(MinecraftBlock a, MinecraftBlock b) { return a.Equals(b); }

        public static bool operator !=(MinecraftBlock a, MinecraftBlock b) { return !a.Equals(b); }

        public override int GetHashCode()
        {
            return ((int)id << 8) | data;
        }
    }
}
