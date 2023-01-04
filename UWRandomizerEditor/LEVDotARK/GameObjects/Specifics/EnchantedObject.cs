namespace UWRandomizerEditor.LEVDotARK.GameObjects.Specifics;

// Todo: Get enchantment name in strings chunk 5
// Todo: create an enum or something with spell names, and their indices, to use here.
public class EnchantedObject : SpecialLinkGameObject
{
    public new readonly int EnchantFlag = 1;

    public int Enchantment
    {
        get { return SpecialIdx - 512; }
        set { SpecialIdx = (short) (value + 512); }
    }

    public int Spell
    {
        get
        {
            if ((Enchantment >= 0) | (Enchantment <= 63))
                return Enchantment + 256;
            else if (ItemID == 0x01c9) // a fountain
                return Enchantment;
            else
                return Enchantment + 144;
        }
        set
        {
            if ((Enchantment >= 0) | (Enchantment <= 63))
                Enchantment = value - 256;
            else if (ItemID == 0x01c9) // a fountain
                Enchantment = value;
            else
                Enchantment = value - 144;
        } // todo: these will ReconstructBuffer too right?
    }

    public EnchantedObject(byte[] buffer, ushort idx) : base(buffer, idx)
    {
    }

    public EnchantedObject(ushort objIdFlags, ushort position, ushort qualityChain,
        ushort linkSpecial) : base(objIdFlags, position, qualityChain, linkSpecial)
    {
    }
}