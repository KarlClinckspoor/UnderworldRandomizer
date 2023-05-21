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

        if (UW1ItemIDsOfItemsThatBlockViewAndCantBeMoved.Contains(obj.ItemID))
            return false;

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

    private static readonly int[] UW1ItemIDsOfItemsThatBlockViewAndCantBeMoved = new int[] {
        19, // Stone
        140, // Urn
        211, // Stalactite
        215, // Anvil
        218, // Rubble
        219, // Wood chips
        220, // Pile of bones
        279, // Orb
        297, // Glowing rock (pacman easter egg)
        298, // Campfire
        302, // Fountain
        303, // Cauldron
        457, // Fountain
        339, // Large boulder
        340, // Large boulder
    };

    private const bool DefaultMovableRuleByClass = true;
}