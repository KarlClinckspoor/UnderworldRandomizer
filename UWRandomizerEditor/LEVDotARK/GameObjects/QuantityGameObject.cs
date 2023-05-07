namespace UWRandomizerEditor.LEVdotARK.GameObjects;

public class QuantityGameObject : StaticObject
{
    // is_quant is true, quantity < 512 (coins, etc)
    public ushort Quantity
    {
        // get { return (byte) ((linkSpecial >> 6) & 0b1111111111); }
        get { return QuantityOrSpecialLinkOrSpecialProperty; }
        set
        {
            if (value > 512)
            {
                throw new Exception("Cannot have a Quantity Game Object with quantity > 512");
            }

            LinkSpecial = (ushort) Utils.SetBits(LinkSpecial, value, 0b1111111111, 6);
            ReconstructBuffer();
        }
    }

    public QuantityGameObject(byte[] buffer, ushort idx) : base(buffer, idx)
    {
    }

    public QuantityGameObject(ushort objIdFlags, ushort position,
        ushort qualityChain, ushort linkSpecial, ushort idxAtObjectArray) : base(objIdFlags, position,
        qualityChain, linkSpecial, idxAtObjectArray)
    {
    }
}