using UWRandomizerEditor.LEVDotARK;
using UWRandomizerEditor.LEVDotARK.GameObjects.Specifics;

namespace UWRandomizer;

public static partial class RandoTools
{
    public static void RemoveAllDoorReferencesToLocks(ArkLoader arkFile)
    {
        foreach (var block in arkFile.TileMapObjectsBlocks)
        {
            foreach (var staticObject in block.StaticObjects)
            {
                if (staticObject is Door door)
                {
                    door.RemoveLock();
                }
            }
            block.UpdateStaticObjectInfoBuffer();
            block.UpdateBuffer();
        }
        arkFile.ReconstructBufferFromBlocks();
    }
}