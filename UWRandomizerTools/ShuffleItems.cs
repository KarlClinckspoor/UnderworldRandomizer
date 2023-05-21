using UWRandomizerEditor.LEVdotARK;
using UWRandomizerEditor.LEVdotARK.Blocks;
using UWRandomizerEditor.LEVdotARK.GameObjects;

namespace UWRandomizerTools;

public static class ShuffleItems
{
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

    public static bool IsTileValid(Tile tile)
    {
        // Initially, I'm only putting items in open spaces!
        // I'll deal with moving them to the appropriate corners later
        if ((Tile.TileTypes) tile.TileType != Tile.TileTypes.Open)
        {
            return false;
        }
        
        // Preventing items from being placed in the central shaft area
        if (tile.LevelNum < 6) // Lvls 1-4 have the shaft aligned in the middle. Lvl 5,6 don't have a shaft
        {
            if (InRectangle(tile, 30, 33, 33, 30))
                return false;
        }
        else if (tile.LevelNum == 6) // Lvl 7. Has a weird shape. Divided into rectangles...
        {
            if (
                InRectangle(tile, 28, 32, 35, 32) |
                InRectangle(tile, 28, 33, 35, 33) |
                InRectangle(tile, 28, 31, 35, 31) |
                InRectangle(tile, 29, 30, 34, 30) |
                InRectangle(tile, 30, 29, 33, 29) |
                InRectangle(tile, 29, 34, 34, 34) |
                InRectangle(tile, 30, 35, 33, 35)
            )
                return false;
        }
        else if (tile.LevelNum == 7) // Lvl 8. Rectangle + Strip
        {
            if (
                InRectangle(tile, 29, 35, 35, 30) |
                InRectangle(tile, 31, 29, 33, 29)
            )
                return false;
        }

        // Can't place items on water, or else they might vanish. TODO: Need to test though!
        if ((tile.FloorTextureIdx == LevelTextureIdxOfWater[tile.LevelNum]) |
            tile.FloorTextureIdx == LevelTextureIdxOfLava[tile.LevelNum])
        {
            return false;
        }

        return true;
    }

    private static bool InRectangle(Tile tile,
        uint topLeftX, uint topLeftY,
        uint topRightX, uint bottomLeftY)
    {
        if (
            (tile.XPos >= topLeftX & tile.XPos <= topRightX) &
            (tile.YPos >= bottomLeftY & tile.YPos <= topLeftY)
        )
        {
            return true;
        }

        return false;
    }
    
}