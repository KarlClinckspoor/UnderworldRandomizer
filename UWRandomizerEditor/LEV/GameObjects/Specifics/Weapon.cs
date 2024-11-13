namespace UWRandomizerEditor.LEV.GameObjects.Specifics;

public class Weapon : StaticObject, IEnchantable<WeaponEnchantment>
{
    public bool IsEnchanted { get; private set; }

    public ushort SpecialIdx
    {
        get => QuantityOrSpecialLinkOrSpecialProperty;
        // TODO: Should't 'LinkSpecial' be 'QuantityOrSpecialLinkOrSpecialProperty'?
        set => LinkSpecial = (ushort) Utils.SetBits(LinkSpecial, value, 0b1111111111, 6);
    }

    public int EnchantmentNumber
    {
        get => SpecialIdx - 512;
        set => SpecialIdx = (ushort) (value + 512);
    }

    public int SpellValue
    {
        get => EnchantmentNumber >= 192 ? EnchantmentNumber + 256 : SpecialIdx; // UWE
    }

    public WeaponEnchantment Enchantment => IsEnchanted ? (WeaponEnchantment)SpellValue : WeaponEnchantment.Nothing;

    public void AddEnchantment(WeaponEnchantment enchant)
    {
        EnchantmentNumber = ((int)enchant - 256); // TODO: test
        IsEnchanted = true;
    }

    public void ForceAddEnchantment(int value)
    {
        EnchantmentNumber = value;
        IsEnchanted = true;
    }

    public void RemoveEnchantment()
    {
        IsEnchanted = false;
        EnchantmentNumber = 1;
    }
    
    public IdentificationStatus IDStatus
    {
        get => (IdentificationStatus)(Heading & 0b111);
        set => Heading = (byte) Utils.SetBits(Heading, (byte)value, 0b111, 0);
    }

    public Weapon(byte[] buffer, ushort idx) : base(buffer, idx)
    {
    }

    public Weapon(ushort objIdFlags, ushort position, ushort qualityChain,
        ushort linkSpecial, ushort idxAtObjectArray) : base(objIdFlags, position, qualityChain, linkSpecial, idxAtObjectArray)
    {
    }
}

public class MeleeWeapon : Weapon
{
    public MeleeWeapon(byte[] buffer, ushort idx) : base(buffer, idx) { }

    public MeleeWeapon(ushort objIdFlags, ushort position, ushort qualityChain, ushort linkSpecial, ushort idxAtObjectArray) : base(objIdFlags, position, qualityChain, linkSpecial, idxAtObjectArray) { }
}

public class RangedWeapon: Weapon
{
    public RangedWeapon(byte[] buffer, ushort idx) : base(buffer, idx)
    {
    }

    public RangedWeapon(ushort objIdFlags, ushort position, ushort qualityChain, ushort linkSpecial, ushort idxAtObjectArray) : base(objIdFlags, position, qualityChain, linkSpecial, idxAtObjectArray)
    {
    }
}

public class SpecialRangedWeapon: Weapon
{
    public SpecialRangedWeapon(byte[] buffer, ushort idx) : base(buffer, idx)
    {
    }

    public SpecialRangedWeapon(ushort objIdFlags, ushort position, ushort qualityChain, ushort linkSpecial, ushort idxAtObjectArray) : base(objIdFlags, position, qualityChain, linkSpecial, idxAtObjectArray)
    {
    }
}

public enum WeaponEnchantment
{
    MinorAccuracy=448,
    Accuracy=449,
    AdditionalAccuracy=450,
    MajorAccuracy=451,
    GreatAccuracy=452,
    VeryGreatAccuracy=453,
    TremendousAccuracy=454,
    UnsurpassedAccuracy=455,
    MinorDamage=456,
    Damage=457,
    AdditionalDamage=458,
    MajorDamage=459,
    GreatDamage=460,
    VeryGreatDamage=461,
    TremendousDamage=462,
    UnsurpassedDamage=463,
    Nothing=1,
}
    

