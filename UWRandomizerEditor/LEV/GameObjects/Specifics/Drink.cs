namespace UWRandomizerEditor.LEV.GameObjects.Specifics;

public class Drink: StaticObject, IEnchantable<DrinkEnchantment>
{
    public bool IsEnchanted { get; }
    public void AddEnchantment(DrinkEnchantment value)
    {
        throw new NotImplementedException();
    }

    public void RemoveEnchantment()
    {
        throw new NotImplementedException();
    }

    public DrinkEnchantment Enchantment { get; }
    public int SpellValue { get; }
    public int EnchantmentNumber { get; set; }
    public ushort SpecialIdx { get; set; }
}

public enum DrinkEnchantment {}
