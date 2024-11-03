using UWRandomizerEditor.LEV;
using UWRandomizerEditor.LEV.Blocks;
using UWRandomizerEditor.LEV.GameObjects;
using UWRandomizerEditor.LEV.GameObjects.Specifics;

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

    public static void ShuffleItemsInAllLevels(LevLoader arkFile, Random RandomInstance,
        ItemRandomizationSettings settings)
    {
        foreach (var block in arkFile.TileMapObjectsBlocks)
        {
            ShuffleItemsInOneLevel(block, RandomInstance, settings);
            block.ReconstructBuffer();
        }

        arkFile.ReconstructBuffer();
    }

    private static void ShuffleItemsInOneLevel(MapObjBlock block, Random RandomInstance,
        ItemRandomizationSettings settings)
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

    private static bool RevealEnchantmentOfItem(GameObject obj, bool verbose = false)
    {
        if (obj is MobileObject | obj is TexturedGameObject | obj is Furniture | obj is Door | obj is Trigger |
            obj is Trap | obj is Key | obj is Lock) return false;
        
        if (verbose)
        {
            Console.WriteLine($"Changed object {{obj}} enchantment reveal status from {obj.Heading} to 2 (identified)");
        }
        obj.Heading = 7;
        return true;
    }

    public static void RevealEnchantmentOfAllItems(LevLoader arkFile)
    {
        var ctrLevel = 0;
        var ctrModifications = 0;
        foreach (var level in arkFile.TileMapObjectsBlocks)
        {
            Console.WriteLine($"Evaluating level {ctrLevel}");
            foreach (var obj in level.StaticObjects)
            {
                ctrModifications += RevealEnchantmentOfItem(obj, verbose: true) ? 1 : 0;
            } 
        }
    }
}