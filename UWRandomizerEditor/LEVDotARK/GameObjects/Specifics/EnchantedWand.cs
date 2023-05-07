namespace UWRandomizerEditor.LEVdotARK.GameObjects.Specifics;

public class EnchantedWand : SpecialLinkGameObject
{

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
        ushort linkSpecial, ushort idxAtObjectArray) : base(objIdFlags, position, qualityChain, linkSpecial, idxAtObjectArray)
    {
        throw new NotImplementedException();
    }
}