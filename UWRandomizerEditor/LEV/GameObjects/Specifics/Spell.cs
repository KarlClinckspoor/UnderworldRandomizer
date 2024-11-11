namespace UWRandomizerEditor.LEV.GameObjects.Specifics;

public class Spell: StaticObject, IEnchantable<SpellEnchantment>
{
    public bool IsEnchanted { get; }
    public void AddEnchantment(SpellEnchantment value)
    {
        throw new NotImplementedException();
    }

    public void RemoveEnchantment()
    {
        throw new NotImplementedException();
    }

    public SpellEnchantment Enchantment { get; }
    public int SpellValue { get; }
    public int EnchantmentNumber { get; set; }
    public ushort SpecialIdx { get; set; }
}

public enum SpellEnchantment
{
    
}

