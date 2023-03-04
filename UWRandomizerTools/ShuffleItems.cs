using System.Collections.Generic;
using UWRandomizerEditor.LEVdotARK;
using UWRandomizerEditor.LEVdotARK.Blocks;
using UWRandomizerEditor.LEVdotARK.GameObjects;

namespace UWRandomizerTools;

public class ShuffleItems
{
    public static void ShuffleAllLevels(ArkLoader arkFile, Random RandomInstance, ItemRandomizationSettings settings)
    {
        foreach (var block in arkFile.TileMapObjectsBlocks)
        {
            ShuffleItemsInLevel(block, RandomInstance, settings);
            block.ReconstructBuffer();
        }

        arkFile.ReconstructBuffer();
    }

    public static void ShuffleItemsInLevel(TileMapMasterObjectListBlock block, Random RandomInstance, ItemRandomizationSettings settings)
    {
        Stack<GameObject> objectsInLevel = new Stack<GameObject>();
        foreach (var tile in block.TileInfos)
        {
            foreach (var obj in ItemTools.ExtractMovableItems(tile, settings))
            {
                objectsInLevel.Push(obj);
            }
        }

        while (objectsInLevel.Count > 0)
        {
            int chosenTileIdx = RandomInstance.Next(0, block.TileInfos.Length);
            TileInfo chosenTile = block.TileInfos[chosenTileIdx];
            if (!IsTileValid(chosenTile, block.LevelNumber))
                continue;
            chosenTile.ObjectChain.Add(objectsInLevel.Pop());
            chosenTile.MoveObjectsToSameZLevel();
        }

        // foreach (var tile in block.TileInfos)
        // {
        //     tile.MoveObjectsToCorrectCorner(); // This shouldn't have to do anything for now
        //     tile.MoveObjectsToSameZLevel();
        // }

        block.ReconstructBuffer();
    }

    private static readonly IDictionary<int, int> LevelTextureIdxOfWater = new Dictionary<int, int>()
    {
        {0, 8}, // lvl1
        {1, 8}, // lvl2
        {2, 8}, // lvl3
        {3, 7}, // lvl4
        {4, 8}, // lvl5
        {5, -1}, // lvl6
        {6, 8}, // lvl7
        {7, -1}, // lvl8
        {8, 7}, // lvl9, void
    };

    // TODO: Lvl8 appears to have 2 textures that are lava, right below the fire elementals beside the doors
    private static readonly IDictionary<int, int> LevelTextureIdxOfLava = new Dictionary<int, int>()
    {
        {0, -1}, // lvl1
        {1, -1}, // lvl2
        {2, -1}, // lvl3
        {3, -1}, // lvl4
        {4, 6}, // lvl5
        {5, 8}, // lvl6
        {6, -1}, // lvl7
        {7, 7}, // lvl8
        {8, 4}, // lvl9, void
    };

    private static bool IsTileValid(TileInfo tile, int levelNumber)
    {
        // Initially, I'm only putting items in open spaces!
        // I'll deal with moving them to the appropriate corners later
        if ((TileInfo.TileTypes) tile.TileType != TileInfo.TileTypes.Open)
        {
            return false;
        }
        // TODO: Don't put items in the central shaft area

        // Can't place items on water, or else they might vanish. TODO: Need to test though!
        if ((tile.FloorTextureIdx == LevelTextureIdxOfWater[levelNumber]) |
            tile.FloorTextureIdx == LevelTextureIdxOfLava[levelNumber])
        {
            return false;
        }

        return true;
    }
}