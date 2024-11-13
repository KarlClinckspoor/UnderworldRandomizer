﻿namespace UWRandomizerEditor.LEV.GameObjects.Specifics;

public class Treasure: StaticObject
{
    public Treasure(byte[] buffer, ushort idx) : base(buffer, idx)
    {
    }

    public Treasure(ushort objIdFlags, ushort position, ushort qualityChain,
        ushort linkSpecial, ushort idxAtObjectArray) : base(objIdFlags, position, qualityChain, linkSpecial, idxAtObjectArray)
    {
    }
    
}