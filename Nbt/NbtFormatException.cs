﻿using System;

namespace MC2UE.Nbt
{
    /// <summary> Exception thrown when a format violation is detected while
    /// parsing or serializing an NBT file. </summary>
    [Serializable]
    public sealed class NbtFormatException : Exception
    {
        internal NbtFormatException(string message)
            : base(message)
        { }
    }
}
