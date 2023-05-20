using UWRandomizerEditor.LEVdotARK;
using UWRandomizerEditor.LEVdotARK.GameObjects;

namespace UWRandomizerTools;

public static class ItemTools
{
    public static List<GameObject> ExtractMovableItems(Tile tile, ItemRandomizationSettings settings)
    {
        return ExtractMovableItems(tile.ObjectChain, settings);
    }

    public static List<GameObject> ExtractMovableItems(UWLinkedList list, IRandoSettings settings)
    {
        var tempList = list.Where(obj => settings.ShouldBeMoved(obj)).ToList();

        foreach (var removedObject in tempList)
        {
            list.Remove(removedObject);
        }

        return tempList;
    }
}