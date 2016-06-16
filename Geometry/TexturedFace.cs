using System;
using System.Drawing;

namespace MC2UE.Geometry
{
    class TexturedFace
    {
        public FaceVertices faceVertices;
        public Size textureMapping;

        public TexturedFace(Volume volume, Face face, FaceVertices faceVertices)
        {
            this.faceVertices = faceVertices;

            switch (face)
            {
                case Face.PositiveX:
                case Face.NegativeX:
                    textureMapping = new Size(volume.ScaleY, volume.ScaleZ);
                    break;

                case Face.PositiveY:
                case Face.NegativeY:
                    textureMapping = new Size(volume.ScaleX, volume.ScaleZ);
                    break;

                case Face.PositiveZ:
                case Face.NegativeZ:
                    textureMapping = new Size(volume.ScaleX, volume.ScaleY);
                    break;

                default: throw new Exception("Unknown texture face.");
            }
        }
    }
}
