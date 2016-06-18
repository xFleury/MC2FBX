namespace NbtToObj.Geometry
{
    struct FacedVolume
    {
        public readonly Volume volume;
        public Face excludedFaces;

        public FacedVolume(Volume volume, Face excludedFaces)
        {
            this.volume = volume;
            this.excludedFaces = excludedFaces;
        }
    }
}
