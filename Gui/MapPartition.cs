using NbtToObj.Geometry;
using NbtToObj.Helpers;
using NbtToObj.Minecraft;
using NbtToObj.Wavefront;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtToObj.Gui
{
    class MapPartition
    {
        public string outputName;

        public PhysicalMaterial physicalMaterial;

        public Dictionary<CoordinateInt, Block> rawBlocks;

        public Dictionary<Block, HashSet<CoordinateInt>> organizedBlocks;

        public static HashSet<CoordinateInt> invisibleBricks;

        public readonly MultiValueDictionary<Block, Volume> volumizedWorld = new MultiValueDictionary<Block, Volume>();

        public MultiValueDictionary<Block, FacedVolume> facedVolumes;

        public int hiddenFaces;

        public int visibleFaces;

        /* Storage for the actual 3D geometry. */

        public readonly List<CoordinateDecimal> vertices = new List<CoordinateDecimal>();

        public readonly Dictionary<string, List<FaceVertices>> collisionBoxes = new Dictionary<string, List<FaceVertices>>();

        public readonly Dictionary<BlockFaceTexture, List<TexturedFace>> texturedFaces = new Dictionary<BlockFaceTexture, List<TexturedFace>>();

        public readonly TextureCoordinateDictionary textureCoordinates = new TextureCoordinateDictionary();
    }
}
