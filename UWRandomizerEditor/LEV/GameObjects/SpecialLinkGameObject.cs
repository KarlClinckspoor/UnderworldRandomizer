using System.Diagnostics;

namespace UWRandomizerEditor.LEV.GameObjects;

public class SpecialLinkGameObject : StaticObject
{
    public ushort SpecialIdx
    {
        get => QuantityOrSpecialLinkOrSpecialProperty;
        // TODO: Should't 'LinkSpecial' be 'QuantityOrSpecialLinkOrSpecialProperty'?
        set => LinkSpecial = (ushort) Utils.SetBits(LinkSpecial, value, 0b1111111111, 6);
    }

    public SpecialLinkGameObject(byte[] buffer, ushort idx) : base(buffer, idx)
    {
        Debug.Assert(IsQuant == 0);
    }

    public SpecialLinkGameObject(ushort objIdFlags, ushort position, ushort qualityChain,
        ushort linkSpecial, ushort idxAtObjectArray) : base(objIdFlags, position, qualityChain, linkSpecial, idxAtObjectArray)
    {
        Debug.Assert(IsQuant == 0);
    }
}