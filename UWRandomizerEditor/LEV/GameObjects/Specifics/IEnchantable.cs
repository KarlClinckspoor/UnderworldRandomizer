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
}