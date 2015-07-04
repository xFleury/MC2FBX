using System;
using System.Collections;
using System.Collections.Generic;

namespace MC2FBX
{
    struct FacedVolume
    {
        public readonly Volume volume;
        public Faces faces;

        public FacedVolume(Volume volume, Faces faces)
        {
            this.volume = volume;
            this.faces = faces;
        }
    }
}
