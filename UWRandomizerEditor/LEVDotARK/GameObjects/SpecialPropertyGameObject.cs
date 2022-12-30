namespace UWRandomizerEditor.LEVDotARK.GameObjects;

// is_quant is true, quantity > 512 (special property)
public class SpecialPropertyGameObject : StaticObject
{
    public short RawSpecialLink
    {
        get { return Convert.ToInt16(SpecialLink + 512); }
        set { SpecialLink = Convert.ToInt16(value + 512); }
    }

    public short SpecialLink
    {
        // get { return (byte) ((link_specialField >> 6) & 0b1111111111); }
        get { return Convert.ToInt16(QuantityOrSpecialLinkOrSpecialProperty - 512); }
        set
        {
            if (value < 512)
            {
                throw new Exception("Cannot have a SpecialLink with value < 512");
            }

            link_specialField = (ushort) Utils.SetBits(link_specialField, value, 0b1111111111, 6);
            ReconstructBuffer();
        }
    }

    public SpecialPropertyGameObject(byte[] buffer, short idx) : base(buffer, idx)
    {
    }

    public SpecialPropertyGameObject(ushort objid_flagsField, ushort positionField, ushort quality_chainField,
        ushort link_specialField) : base(objid_flagsField, positionField, quality_chainField, link_specialField)
    {
    }
}