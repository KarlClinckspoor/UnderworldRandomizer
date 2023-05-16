namespace UWRandomizerEditor.LEVdotARK.GameObjects;

// is_quant is true, quantity > 512 (special property)
public class SpecialPropertyGameObject : StaticObject
{
    public short RawSpecialLink
    {
        get => Convert.ToInt16(SpecialLink + 512);
        set => SpecialLink = Convert.ToInt16(value + 512);
    }

    public short SpecialLink
    {
        get => Convert.ToInt16(QuantityOrSpecialLinkOrSpecialProperty - 512);
        set
        {
            if (value < 512)
            {
                throw new Exception("Cannot have a SpecialLink with value < 512");
            }

            LinkSpecial = (ushort) Utils.SetBits(LinkSpecial, value, 0b1111111111, 6);
            ReconstructBuffer();
        }
    }

    public SpecialPropertyGameObject(byte[] buffer, ushort idx) : base(buffer, idx)
    {
    }

    public SpecialPropertyGameObject(ushort objIdFlags, ushort position, ushort qualityChain,
        ushort linkSpecial, ushort idxAtObjectArray) : base(objIdFlags, position, qualityChain, linkSpecial, idxAtObjectArray)
    {
    }
}