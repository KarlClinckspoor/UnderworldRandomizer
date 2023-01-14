using UWRandomizerEditor.LEVDotARK;
using UWRandomizerEditor.LEVDotARK.GameObjects;
using UWRandomizerEditor.LEVDotARK.GameObjects.Specifics;

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
        // todo:
        // if (obj.IsInContainer)
        //  return false;
        // 
        bool res;
        try
        {
            res = settings.MovableRules[obj.GetType()];
        }
        catch (KeyNotFoundException)
        {
            res = settings.DefaultMovableRule;
        }

        return res;
    }
}

public class ItemRandomizationSettings
{
    public Dictionary<Type, bool> MovableRules = new Dictionary<Type, bool>()
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
    };

    public bool DefaultMovableRule = false;
}