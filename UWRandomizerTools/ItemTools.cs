using UWRandomizerEditor.LEVdotARK;
using UWRandomizerEditor.LEVdotARK.Blocks;
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

    public static void ShuffleItemsInAllLevels(ArkLoader arkFile, Random RandomInstance, ItemRandomizationSettings settings)
    {
        foreach (var block in arkFile.TileMapObjectsBlocks)
        {
            ShuffleItemsInOneLevel(block, RandomInstance, settings);
            block.ReconstructBuffer();
        }

        arkFile.ReconstructBuffer();
    }

    public static void ShuffleItemsInOneLevel(TileMapMasterObjectListBlock block, Random RandomInstance, ItemRandomizationSettings settings)
    {
        Stack<GameObject> objectsInLevel = new Stack<GameObject>();
        foreach (var tile in block.Tiles)
        {
            foreach (var obj in ItemTools.ExtractMovableItems(tile, settings))
            {
                objectsInLevel.Push(obj);
            }
        }

        while (objectsInLevel.Count > 0)
        {
            int chosenTileIdx = RandomInstance.Next(0, block.Tiles.Length);
            Tile chosenTile = block.Tiles[chosenTileIdx];
            if (!ShuffleItems.IsTileValid(chosenTile))
                continue;
            chosenTile.ObjectChain.Add(objectsInLevel.Pop());
            chosenTile.MoveObjectsToSameZLevel();
        }

        block.ReconstructBuffer();
    }
}