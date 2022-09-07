﻿namespace Randomizer.LEVDotARK.GameObjects;

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
    { }

    public EnchantedWand(short objid_flagsField, short positionField, short quality_chainField,
        short link_specialField) : base(objid_flagsField, positionField, quality_chainField, link_specialField)
    { }

}