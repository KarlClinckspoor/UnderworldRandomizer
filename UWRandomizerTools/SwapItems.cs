using System.Diagnostics;
using UWRandomizerEditor.LEVdotARK;

namespace UWRandomizerTools;

public static partial class RandoTools
{
    /// <summary>
    /// Swaps items between two tiles by changing their first object indices, then
    /// places them at the correct Z level. If one tile has a texturedObject, might be misaligned later.
    /// </summary>
    /// <param name="tile1">First Tile to replace</param>
    /// <param name="tile2">Second tile to replace</param>
    /// <param name="settings">Configuration item that controls what is considered a movable item</param>
    /// <param name="r">random instance that's passed to the functions that move objects to the correct corner of tiles</param>
    public static void SwapAllObjectsBetweenTwoTiles(TileInfo tile1, TileInfo tile2, ItemRandomizationSettings settings, Random r)
    {
        var obj1 = ItemTools.ExtractMovableItems(tile1, settings);
        var obj2 = ItemTools.ExtractMovableItems(tile2, settings);
        
        tile1.ObjectChain.AppendItems(obj2);
        tile2.ObjectChain.AppendItems(obj1);
        
        tile1.MoveObjectsToCorrectCorner(r);
        tile1.MoveObjectsToSameZLevel();
        tile2.MoveObjectsToCorrectCorner(r);
        tile2.MoveObjectsToSameZLevel();
        
        Debug.Assert(tile1.ObjectChain.CheckIntegrity());
        Debug.Assert(tile2.ObjectChain.CheckIntegrity());
    }

}
