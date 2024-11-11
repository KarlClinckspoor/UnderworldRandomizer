namespace UWRandomizerEditor.LEV.GameObjects.Specifics;

public class Scrolls: StaticObject, IEnchantable<ScrollEnchantment>
{
    public bool IsEnchanted { get; }
    public void AddEnchantment(ScrollEnchantment value)
    {
        throw new NotImplementedException();
    }

    public void RemoveEnchantment()
    {
        throw new NotImplementedException();
    }

    public ScrollEnchantment Enchantment { get; }
    public int SpellValue { get; }
    public int EnchantmentNumber { get; set; }
    public ushort SpecialIdx { get; set; }
}

public enum ScrollEnchantment
{
    
}