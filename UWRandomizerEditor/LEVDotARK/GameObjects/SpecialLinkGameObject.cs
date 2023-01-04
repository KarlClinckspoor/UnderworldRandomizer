namespace UWRandomizerEditor.LEVDotARK.GameObjects;

public class SpecialLinkGameObject : StaticObject
{
    // is_quant is false. Enchantments, wands, etc
    public new readonly int IsQuant = 0;

    public short SpecialIdx
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
    }

    public SpecialLinkGameObject(ushort objIdFlags, ushort position, ushort qualityChain,
        ushort linkSpecial) : base(objIdFlags, position, qualityChain, linkSpecial)
    {
    }
}