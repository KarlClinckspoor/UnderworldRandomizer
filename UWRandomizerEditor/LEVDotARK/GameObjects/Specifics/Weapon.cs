namespace UWRandomizerEditor.LEVDotARK.GameObjects.Specifics;

public class EnchantedWeapon : SpecialLinkGameObject
{
    public new readonly int EnchantFlag = 1;

    public int Enchantment
    {
        get { return SpecialIdx - 512; }
        set { SpecialIdx = (short) (value + 512); }
    }

    // Oh boy. This is more complicated. Need to have logic to differentiate between Acc/Dam/Prot/Tough and other spells
    public int Spell
    {
        get { return Enchantment + 256; }
        set { Enchantment = value - 256; } // todo: these will ReconstructBuffer too right?
    }

    public EnchantedWeapon(byte[] buffer, ushort idx) : base(buffer, idx)
    {
    }

    public EnchantedWeapon(ushort objIdFlags, ushort position, ushort qualityChain,
        ushort linkSpecial) : base(objIdFlags, position, qualityChain, linkSpecial)
    {
    }
}