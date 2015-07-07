using System;
using System.Collections;
using System.Collections.Generic;

namespace MC2UE
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
