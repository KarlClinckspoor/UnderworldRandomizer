using System.Collections.Generic;
using UWRandomizerEditor.LEVDotARK;
using UWRandomizerEditor.LEVDotARK.Blocks;
using UWRandomizerEditor.LEVDotARK.GameObjects;

namespace UWRandomizer;

public static class ShuffleItems
{
    static void ShuffleAllLevels(ArkLoader arkFile)
    {
        foreach (var block in arkFile.TileMapObjectsBlocks)
        {
            ShuffleItemsInLevel(block);
            block.UpdateBuffer();
        }

        arkFile.ReconstructBufferFromBlocks();
    }
    
    static void ShuffleItemsInLevel(TileMapMasterObjectListBlock block)
    {
        Stack<GameObject> objectsInLevel = new Stack<GameObject>();
        foreach (var tile in block.TileInfos)
        {
            foreach (var obj in tile.ObjectChain.PopObjectsThatShouldBeMoved())
            {
                objectsInLevel.Push(obj);
            }
        }

        while (objectsInLevel.Count > 0)
        {
            int chosenTileIdx = Singletons.RandomInstance.Next(0, block.TileInfos.Length);
            TileInfo chosenTile = block.TileInfos[chosenTileIdx];
            if (!IsTileValid(chosenTile, block.LevelNumber)) 
                continue;
            chosenTile.ObjectChain.Add(objectsInLevel.Pop());
        }

        foreach (var tile in block.TileInfos)
        {
            tile.MoveObjectsToCorrectCorner();
            tile.MoveObjectsToSameZLevel();
        }
        
        block.UpdateBuffer(); // TODO: necessary?
    }
    
    private static IDictionary<int, int> LevelTextureIdxOfWater = new Dictionary<int, int>()
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
    private static IDictionary<int, int> LevelTextureIdxOfLava = new Dictionary<int, int>()
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
        
        if ((TileInfo.TileTypes) tile.TileType == TileInfo.TileTypes.solid)
        {
            return false;
        }
        
        // Can't place items on water, or else they might vanish. TODO: Need to test though!
        if ((tile.FloorTextureIdx == LevelTextureIdxOfWater[levelNumber]) | 
            tile.FloorTextureIdx == LevelTextureIdxOfLava[levelNumber])
        {
            return false;
        }

        return true;
    }
}