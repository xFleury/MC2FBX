using System;
using System.Drawing;

namespace NbtToObj.Geometry
{
    class TexturedFace
    {
        public FaceVertices faceVertices;
        public TextureCoordinate textureCoord;
        public SizeD textureSize;

        public TexturedFace(Volume volume, Face face, FaceVertices faceVertices)
        {
            this.faceVertices = faceVertices;

            switch (face)
            {
                case Face.PositiveX:
                case Face.NegativeX:
                    textureCoord = FromCoordinates(volume.Coord.Y, volume.Coord.Z);
                    textureSize = new SizeD(volume.ScaleY / 2M, volume.ScaleZ / 2M);
                    break;

                case Face.PositiveY:
                case Face.NegativeY:
                    textureCoord = FromCoordinates(volume.Coord.X, volume.Coord.Z);
                    textureSize = new SizeD(volume.ScaleX / 2M, volume.ScaleZ / 2M);
                    break;

                case Face.PositiveZ:
                case Face.NegativeZ:
                    textureCoord = FromCoordinates(volume.Coord.X, volume.Coord.Y);
                    textureSize = new SizeD(volume.ScaleX / 2M, volume.ScaleY / 2M);
                    break;

                default: throw new Exception("Unknown texture face.");
            }
        }

        private static TextureCoordinate FromCoordinates(int u, int v)
        {
            const decimal offset = 0.5M;

            return new TextureCoordinate(
                u % 2 == 0 ? 0M : offset,
                v % 2 == 0 ? 0M : offset);
        }
    }
}
