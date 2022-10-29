namespace UWRandomizerEditor.LEVDotARK.GameObjects;

public class QuantityGameObject : StaticObject
{
    // is_quant is true, quantity < 512 (coins, etc)
    public short Quantity
    {
        // get { return (byte) ((link_specialField >> 6) & 0b1111111111); }
        get { return QuantityOrSpecialLinkOrSpecialProperty; }
        set
        {
            if (value > 512)
            {
                throw new Exception("Cannot have a Quantity Game Object with quantity > 512");
            }
            link_specialField = (short)Utils.SetBits(link_specialField, value, 0b1111111111, 6); UpdateBuffer();
        }
    }

    public QuantityGameObject(byte[] buffer, short idx) : base(buffer, idx)
    { }

    public QuantityGameObject(short objid_flagsField, short positionField,
        short quality_chainField, short link_specialField) : base(objid_flagsField, positionField,
        quality_chainField, link_specialField)
    { }
        
        
}