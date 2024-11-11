namespace UWRandomizerEditor.LEV.GameObjects.Specifics;

public class Potion: StaticObject, IEnchantable<PotionEnchantment>
{
    public bool IsEnchanted { get; }
    public void AddEnchantment(PotionEnchantment value)
    {
        throw new NotImplementedException();
    }

    public void RemoveEnchantment()
    {
        throw new NotImplementedException();
    }

    public PotionEnchantment Enchantment { get; }
    public int SpellValue { get; }
    public int EnchantmentNumber { get; set; }
    public ushort SpecialIdx { get; set; }
}

public enum PotionEnchantment
{
    
}