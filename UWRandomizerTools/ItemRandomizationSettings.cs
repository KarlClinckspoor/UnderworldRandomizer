using UWRandomizerEditor.LEVdotARK.GameObjects;
using UWRandomizerEditor.LEVdotARK.GameObjects.Specifics;

namespace UWRandomizerTools;

public class ItemRandomizationSettings : IRandoSettings
{
    public bool ShouldBeMoved(GameObject obj)
    {
        if ((obj.IdxAtObjectArray == 0) | (obj.IdxAtObjectArray == 1))
        {
            return false;
        }

        // Check by obj type
        var type = obj.GetType();
        bool filterByClass;
        if (!MovableRulesByClass.TryGetValue(type, out filterByClass))
        {
            filterByClass = DefaultMovableRuleByClass;
        }

        return filterByClass;

    }

    private readonly Dictionary<Type, bool> MovableRulesByClass = new Dictionary<Type, bool>()
    {
        {typeof(Furniture), false},
        {typeof(Door), false},
        {typeof(Trigger), false},
        {typeof(TexturedGameObject), false},
        {typeof(MobileObject), false},
        {typeof(Lock), false},
        {typeof(Trap), false},
        {typeof(GameObject), false},
        {typeof(StaticObject), true},
        {typeof(QuantityGameObject), true},
        {typeof(EnchantedArmor), true},
        {typeof(EnchantedObject), true},
        {typeof(EnchantedWand), true},
        {typeof(EnchantedWeapon), true},
        {typeof(Key), true},
        {typeof(Container), true},
    };

    private static readonly ushort[] UW1itemIDsThatShouldntBeMoved = new ushort[] {
        1, 2, 3
    };

    private const bool DefaultMovableRuleByClass = true;
}