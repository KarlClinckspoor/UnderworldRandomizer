﻿namespace UWRandomizerEditor.LEVdotARK.GameObjects.Specifics;

public class EnchantedArmor : SpecialLinkGameObject
{
    public new readonly int EnchantFlag = 1;

    public int Enchantment
    {
        get => SpecialIdx - 512;
        set => SpecialIdx = (ushort) (value + 512);
    }

    // Oh boy. This is more complicated. Need to have logic to differentiate between Acc/Dam/Prot/Tough and other spells
    public int Spell
    {
        get => Enchantment + 256 + 16;
        set => Enchantment = value - 256 - 16;
    }

    public EnchantedArmor(byte[] buffer, ushort idx) : base(buffer, idx)
    {
    }

    public EnchantedArmor(ushort objIdFlags, ushort position, ushort qualityChain,
        ushort linkSpecial, ushort idxAtObjectArray) : base(objIdFlags, position, qualityChain, linkSpecial, idxAtObjectArray)
    {
    }
}