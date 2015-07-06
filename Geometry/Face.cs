using System;

namespace MC2FBX
{
    [Flags]
    enum Face
    {
        None = 0,
        PositiveX = 1,
        PositiveY = 2,
        PositiveZ = 4,
        NegativeX = 8,
        NegativeY = 16,
        NegativeZ = 32,
        All = PositiveX + PositiveY + PositiveZ + NegativeX + NegativeY + NegativeZ
    }
}
