using UWRandomizerEditor.LEVDotARK;

namespace UWRandomizerEditor.Randomization;

static public class SwapItemTools
{
    /// <summary>
    /// Swaps items between two tiles by changing their first object indices, then
    /// places them at the correct Z level. If one tile has a texturedobject, might be disaligned later.
    /// </summary>
    /// <param name="Tile1">First Tile to replace</param>
    /// <param name="Tile2">Second tile to replace</param>
    static void SwapAllObjectsBetweenTwoTiles(TileInfo Tile1, TileInfo Tile2)
    {
        // CheckForTexturedObjects(Tile1, Tile2);
        // (Tile1.FirstObjIdx, Tile2.FirstObjIdx) = (Tile2.FirstObjIdx, Tile1.FirstObjIdx);
        // Tile1.MoveObjectsToSameZLevel();
        // Tile2.MoveObjectsToSameZLevel();
        // Tile1.MoveObjectsToCorrectCorner();
        // Tile2.MoveObjectsToCorrectCorner();
        // if (!Tile1.CheckValidityOfObjects())
        // {
        //     throw new Exception("For some reason Tile1 could not finish the object chain");
        // }
        // if (!Tile2.CheckValidityOfObjects())
        // {
        //     throw new Exception("For some reason Tile2 could not finish the object chain");
        // }
    }

    // /// <summary>
    // /// Checks if there's textured objects in either tiles, and issues a warning.
    // /// </summary>
    // /// <param name="Tile1">First Tile to check</param>
    // /// <param name="Tile2">Second tile to check</param>
    // /// <returns></returns>
    // static bool CheckForTexturedObjects(TileInfo Tile1, TileInfo Tile2)
    // {
    //     if (Tile1.HasTexturedObject | Tile2.HasTexturedObject)
    //     {
    //         Console.WriteLine("Warning, a tile has TexturedObjects. They might look bad.");
    //         return true;
    //     }
    //     return false;
    // }

}
