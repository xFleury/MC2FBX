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
    class WorldState
    {
        public Dictionary<CoordinateInt, Block> visibleAndInvisibleBlocks;
        public Dictionary<CoordinateInt, Block> visibleBlocks;
        public HashSet<CoordinateInt> invisibleBricks;

        public List<ChunkState> chunks;


        
        

        public int hiddenFaces;

        public int visibleFaces;

        public string outputName;

        /* Storage for the actual 3D geometry. */

        public readonly List<CoordinateDecimal> vertices = new List<CoordinateDecimal>();

        public readonly Dictionary<string, List<FaceVertices>> collisionBoxes = new Dictionary<string, List<FaceVertices>>();

        public readonly MultiValueDictionary<BlockFaceTexture, TexturedFace> texturedFaces = new MultiValueDictionary<BlockFaceTexture, TexturedFace>();

        public readonly TextureCoordinateDictionary textureCoordinates = new TextureCoordinateDictionary();

        /* For querying what textured faces pertain to a particular faced volume. */
        public readonly Dictionary<FacedVolume, MultiValueDictionary<BlockFaceTexture, TexturedFace>> texturedFacesOfFacedVolume =
            new Dictionary<FacedVolume, MultiValueDictionary<BlockFaceTexture, TexturedFace>>();
    }
}
