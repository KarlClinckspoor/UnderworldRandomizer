namespace UWRandomizerEditor.LEVDotARK.GameObjects.Specifics;

public class EnchantedWand : SpecialLinkGameObject
{
    public new readonly int EnchantFlag = 1;

    public int SpellObjectLink
    {
        get { return SpecialIdx; }
        set { SpecialIdx = (short) value; }
    }

    public GameObject SpellObject;

    public EnchantedWand(byte[] buffer, short idx) : base(buffer, idx)
    {
        throw new NotImplementedException(); // TODO: Need to link to SpellObject
    }

    public EnchantedWand(ushort objid_flagsField, ushort positionField, ushort quality_chainField,
        ushort link_specialField) : base(objid_flagsField, positionField, quality_chainField, link_specialField)
    {
        throw new NotImplementedException();
    }
    

}