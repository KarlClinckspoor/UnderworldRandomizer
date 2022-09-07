namespace Randomizer.LEVDotARK.GameObjects;

public class SpecialLinkGameObject : GameObject
{
    // is_quant is false. Enchantments, wands, etc
    public new readonly int IsQuant = 0;
    public short SpecialIdx
    {
        get { return QuantityOrSpecialLinkOrSpecialProperty; }
        set { link_specialField = (short)Utils.SetBits(link_specialField, value, 0b1111111111, 6); UpdateBuffer(); }
    }
        
    public SpecialLinkGameObject(byte[] buffer, short idx) : base(buffer, idx)
    { }

    public SpecialLinkGameObject(short objid_flagsField, short positionField, short quality_chainField,
        short link_specialField) : base(objid_flagsField, positionField, quality_chainField, link_specialField)
    { }
        
}