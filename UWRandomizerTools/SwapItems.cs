﻿using System.Diagnostics;
using UWRandomizerEditor.LEVdotARK;

namespace UWRandomizerTools;

public static partial class RandoTools
{
    /// <summary>
    /// Swaps items between two tiles by changing their first object indices, then
    /// places them at the correct Z level. If one tile has a texturedobject, might be disaligned later.
    /// </summary>
    /// <param name="Tile1">First Tile to replace</param>
    /// <param name="Tile2">Second tile to replace</param>
    public static void SwapAllObjectsBetweenTwoTiles(TileInfo Tile1, TileInfo Tile2, ItemRandomizationSettings settings)
    {
        var obj1 = ItemTools.ExtractMovableItems(Tile1, settings);
        var obj2 = ItemTools.ExtractMovableItems(Tile2, settings);
        
        Tile1.ObjectChain.AppendItems(obj2);
        Tile2.ObjectChain.AppendItems(obj1);
        
        Tile1.MoveObjectsToCorrectCorner();
        Tile1.MoveObjectsToSameZLevel();
        Tile2.MoveObjectsToCorrectCorner();
        Tile2.MoveObjectsToSameZLevel();
        
        Debug.Assert(Tile1.ObjectChain.CheckIntegrity());
        Debug.Assert(Tile2.ObjectChain.CheckIntegrity());
    }

}
