using UWRandomizerEditor.LEVdotARK;
using UWRandomizerEditor.LEVdotARK.GameObjects;
using UWRandomizerEditor.LEVdotARK.GameObjects.Specifics;

namespace UWRandomizerTools;

public class ItemTools
{
    public static List<GameObject> ExtractMovableItems(Tile tile, ItemRandomizationSettings settings)
    {
        return ExtractMovableItems(tile.ObjectChain, settings);
    }

    public static List<GameObject> ExtractMovableItems(UWLinkedList list, ItemRandomizationSettings settings)
    {
        var tempList = new List<GameObject>();
        foreach (var obj in list)
        {
            if (ShouldBeMoved(obj, settings))
            {
                tempList.Add(obj);
            }
        }

        foreach (var removedObject in tempList)
        {
            list.Remove(removedObject);
        }

        return tempList;
    }

    private static bool ShouldBeMoved(GameObject obj, ItemRandomizationSettings settings)
    {
        // If the object is invalid (idx 0, 1 or in "Free" slot, it should be ignored)
        if (obj.Invalid) return false;
        
        // We have to ignore objects that are in containers
        if (obj.InContainer) return false;
        
        // Then filter by the class
        bool res;
        try
        {
            res = settings.MovableRulesByClass[obj.GetType()];
        }
        catch (KeyNotFoundException)
        {
            res = ItemRandomizationSettings.DefaultMovableRuleByClass;
        }

        return res;
    }
}

public class ItemRandomizationSettings
{
    public readonly Dictionary<Type, bool> MovableRulesByClass = new Dictionary<Type, bool>()
    {
        { typeof(Furniture), false },
        { typeof(Door), false },
        { typeof(Trigger), false },
        { typeof(TexturedGameObject), false },
        { typeof(MobileObject), false },
        { typeof(Lock), false },
        { typeof(Trap), false },
        { typeof(GameObject), false },
        { typeof(StaticObject), true },
        { typeof(QuantityGameObject), true },
        { typeof(EnchantedArmor), true },
        { typeof(EnchantedObject), true },
        { typeof(EnchantedWand), true },
        { typeof(EnchantedWeapon), true },
        { typeof(Key), true },
        { typeof(Container), true},
    };

    public const bool DefaultMovableRuleByClass = true;
}