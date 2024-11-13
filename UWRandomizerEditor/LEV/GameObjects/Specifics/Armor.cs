namespace UWRandomizerEditor.LEV.GameObjects.Specifics;

public class Armor : StaticObject, IEnchantable<ArmorEnchantment>
{
    public bool IsEnchanted { get; private set; }

    public ushort SpecialIdx
    {
        get => QuantityOrSpecialLinkOrSpecialProperty;
        // TODO: Should't 'LinkSpecial' be 'QuantityOrSpecialLinkOrSpecialProperty'?
        set => LinkSpecial = (ushort)Utils.SetBits(LinkSpecial, value, 0b1111111111, 6);
    }

    public IdentificationStatus IDStatus
    {
        get => (IdentificationStatus)(Heading & 0b111);
        set => Heading = (byte) Utils.SetBits(Heading, (byte)value, 0b111, 0);
    }

    public int EnchantmentNumber
    {
        get => SpecialIdx - 512;
        set => SpecialIdx = (ushort)(value + 512);
    }

    // Oh boy. This is more complicated. Need to have logic to differentiate between Acc/Dam/Prot/Tough and other spells
    public int SpellValue
    {
        get => EnchantmentNumber >= 192 && EnchantmentNumber < 208 ? EnchantmentNumber + 272 : EnchantmentNumber;
    }

    public ArmorEnchantment Enchantment => IsEnchanted ? (ArmorEnchantment)SpellValue : ArmorEnchantment.Nothing;

    public void AddEnchantment(ArmorEnchantment enchant)
    {
        EnchantmentNumber = ((int)enchant - 256);
        IsEnchanted = true;
    }

    public void ForceAddEnchantment(int value)
    {
        EnchantmentNumber = value;
        IsEnchanted = true;
    }

    public void RemoveEnchantment()
    {
        EnchantmentNumber = 1;
        IsEnchanted = false;
    }

    public Armor(byte[] buffer, ushort idx) : base(buffer, idx)
    {
    }

    public Armor(ushort objIdFlags, ushort position, ushort qualityChain,
        ushort linkSpecial, ushort idxAtObjectArray) : base(objIdFlags, position, qualityChain, linkSpecial,
        idxAtObjectArray)
    {
    }
}

public class Bauble : Armor
{
    public Bauble(byte[] buffer, ushort idx) : base(buffer, idx)
    {
    }

    public Bauble(ushort objIdFlags, ushort position, ushort qualityChain, ushort linkSpecial, ushort idxAtObjectArray)
        : base(objIdFlags, position, qualityChain, linkSpecial, idxAtObjectArray)
    {
    }
}

public class Shield : Armor
{
    public Shield(byte[] buffer, ushort idx) : base(buffer, idx)
    {
    }

    public Shield(ushort objIdFlags, ushort position, ushort qualityChain, ushort linkSpecial, ushort idxAtObjectArray)
        : base(objIdFlags, position, qualityChain, linkSpecial, idxAtObjectArray)
    {
    }
}

// TODO: Add and test other enchantments
public enum ArmorEnchantment
{
    MinorProtection = 464,
    Protection = 465,
    AdditionalProtection = 466,
    MajorProtection = 467,
    GreatProtection = 468,
    VeryGreatProtection = 469,
    TremendousProtection = 470,
    UnsurpassedProtection = 471,
    MinorToughness = 472,
    Toughness = 473,
    AdditionalToughness = 474,
    MajorToughness = 475,
    GreatToughness = 476,
    VeryGreatToughness = 477,
    TremendousToughness = 478,
    UnsurpassedToughness = 479,
    Nothing = 1,
}
