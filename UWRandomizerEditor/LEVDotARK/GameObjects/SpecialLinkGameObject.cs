﻿using System.Diagnostics;

namespace UWRandomizerEditor.LEVdotARK.GameObjects;

public class SpecialLinkGameObject : StaticObject
{
    public ushort SpecialIdx
    {
        get { return QuantityOrSpecialLinkOrSpecialProperty; }
        set
        {
            LinkSpecial = (ushort) Utils.SetBits(LinkSpecial, value, 0b1111111111, 6);
            ReconstructBuffer();
        }
    }

    public SpecialLinkGameObject(byte[] buffer, ushort idx) : base(buffer, idx)
    {
        Debug.Assert(IsQuant == 0);
    }

    public SpecialLinkGameObject(ushort objIdFlags, ushort position, ushort qualityChain,
        ushort linkSpecial) : base(objIdFlags, position, qualityChain, linkSpecial)
    {
        Debug.Assert(IsQuant == 0);
    }
}