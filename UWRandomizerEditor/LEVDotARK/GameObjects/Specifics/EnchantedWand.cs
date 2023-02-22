﻿namespace UWRandomizerEditor.LEVdotARK.GameObjects.Specifics;

public class EnchantedWand : SpecialLinkGameObject
{
    private readonly int EnchantFlag = 1;

    public int SpellObjectLink
    {
        get { return SpecialIdx; }
        set { SpecialIdx = (ushort) value; }
    }

    public GameObject SpellObject;

    public EnchantedWand(byte[] buffer, ushort idx) : base(buffer, idx)
    {
        throw new NotImplementedException(); // TODO: Need to link to SpellObject
    }

    public EnchantedWand(ushort objIdFlags, ushort position, ushort qualityChain,
        ushort linkSpecial) : base(objIdFlags, position, qualityChain, linkSpecial)
    {
        throw new NotImplementedException();
    }
}