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
            link_specialField = (ushort) Utils.SetBits(link_specialField, value, 0b1111111111, 6);
            ReconstructBuffer();
        }
    }

    public SpecialLinkGameObject(byte[] buffer, short idx) : base(buffer, idx)
    {
    }

    public SpecialLinkGameObject(ushort objid_flagsField, ushort positionField, ushort quality_chainField,
        ushort link_specialField) : base(objid_flagsField, positionField, quality_chainField, link_specialField)
    {
    }
}