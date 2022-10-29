using UWRandomizerEditor.LEVDotARK;
using UWRandomizerEditor.LEVDotARK.GameObjects.Specifics;

namespace UWRandomizer;

public static partial class RandoTools
{
    public static void RemoveAllReferencesToLocks(ArkLoader ArkFile)
    {
        foreach (var block in ArkFile.TileMapObjectsBlocks)
        {
            foreach (var staticObject in block.StaticObjects)
            {
                if (staticObject is Door door)
                {
                    // door.RemoveLock();
                }
            }
        }
    }
}