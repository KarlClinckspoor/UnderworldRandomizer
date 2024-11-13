namespace UWRandomizerEditor.LEV.GameObjects.Specifics;

public interface IEnchantable<T> where T: Enum
{
    public bool IsEnchanted { get; }
    public void AddEnchantment(T value);
    public void ForceAddEnchantment(int value);
    public void RemoveEnchantment();
    public T Enchantment { get; }
    public int SpellValue { get; }
    public int EnchantmentNumber { get; set; }
    public ushort SpecialIdx { get; set; }

    public IdentificationStatus IDStatus { get; set; }
}

/// <summary>
/// Bit 2: 1 means attempt was made, 0 means no attempt was made. Gets reset to 0 if Lore increases
/// Bits 0 and 1: 00 means no ID, 10 means partial, 11 means full.
/// </summary>
public enum IdentificationStatus: byte
{
    UnidentifiedWithGuessAvailable = 0b000,
    UnidentifiedWithNoGuessAvailable = 0b100,
    PartiallyIdentifiedWithNoGuessAvailable = 0b110,
    PartiallyIdentifiedWithGuessAvailable = 0b010,
    FullyIdentified = 0b111,
}