using UWRandomizerEditor.LEVdotARK;
using UWRandomizerEditor.LEVdotARK.GameObjects;
using UWRandomizerEditor.LEVdotARK.GameObjects.Specifics;

namespace UWRandomizerTools;

public class ItemTools
{
    public static List<GameObject> ExtractMovableItems(TileInfo tile, ItemRandomizationSettings settings)
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
        bool res;
        try
        {
            res = settings.MovableRules[obj.GetType()];
        }
        catch (KeyNotFoundException)
        {
            res = ItemRandomizationSettings.DefaultMovableRule;
        }

        // We have to ignore objects that are in containers, otherwise this messes things up majorly.
        if (obj.InContainer)
        {
            return false;
        }

        // Object at idx 0 is always unused, obj at idx 1 represents the avatar, so can't be moved either.
        if ((obj.IdxAtObjectArray == 0) | (obj.IdxAtObjectArray == 1))
        {
            return false;
        }

        return res;
    }
}

public class ItemRandomizationSettings
{
    public readonly Dictionary<Type, bool> MovableRules = new Dictionary<Type, bool>()
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

    public const bool DefaultMovableRule = false;
}