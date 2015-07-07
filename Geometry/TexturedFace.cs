using System;

namespace MC2UE
{
    class TexturedFace
    {
        public FaceVerticies faceVerticies;
        public int mappingWidth;
        public int mappingHeight;

        public TexturedFace(Volume volume, Face face, FaceVerticies faceVerticies)
        {
            this.faceVerticies = faceVerticies;

            switch (face)
            {
                case Face.PositiveX:
                case Face.NegativeX:
                    mappingWidth = volume.ScaleY;
                    mappingHeight = volume.ScaleZ;
                    break;

                case Face.PositiveY:
                case Face.NegativeY:
                    mappingWidth = volume.ScaleX;
                    mappingHeight = volume.ScaleZ;
                    break;

                case Face.PositiveZ:
                case Face.NegativeZ:
                    mappingWidth = volume.ScaleX;
                    mappingHeight = volume.ScaleY;
                    break;

                default: throw new Exception("Unknown texture face.");
            }
        }
    }
}
