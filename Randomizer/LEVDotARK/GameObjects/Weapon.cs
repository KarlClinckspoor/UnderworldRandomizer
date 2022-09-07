namespace Randomizer.LEVDotARK.GameObjects;

public class EnchantedWeapon : SpecialLinkGameObject
{
        
    public new readonly int EnchantFlag = 1;

    public int Enchantment
    {
        get { return SpecialIdx - 512; }
        set { SpecialIdx = (short) (value + 512); }
    }

    // Oh boy. This is more complicated. Need to have logic to differentiate between Acc/Dam/Prot/Tough and other spells
    public int Spell
    {
        get
        {
            return Enchantment + 256;
        }
        set
        {
            Enchantment = value - 256;
        } // todo: these will UpdateBuffer too right?
    }

    public EnchantedWeapon(byte[] buffer, short idx) : base(buffer, idx)
    { }

    public EnchantedWeapon(short objid_flagsField, short positionField, short quality_chainField,
        short link_specialField) : base(objid_flagsField, positionField, quality_chainField, link_specialField)
    { }
        
}