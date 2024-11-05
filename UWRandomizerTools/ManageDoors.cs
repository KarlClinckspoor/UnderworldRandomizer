using UWRandomizerEditor.LEV;
using UWRandomizerEditor.LEV.GameObjects.Specifics;

namespace UWRandomizerTools;

public static partial class RandoTools
{
    /// <summary>
    /// Iterates through all GameObjects in a lev.ark file and removes links to lock objects. Reconstructs the buffer.
    /// </summary>
    /// <param name="arkFile"></param>
    /// <returns>Count of alterations performed</returns>
    public static int RemoveAllDoorReferencesToLocks(LevLoader arkFile)
    {
        int count = 0;
        foreach (var block in arkFile.MapObjBlocks)
        {
            foreach (var staticObject in block.StaticObjects)
            {
                if (staticObject is Door door)
                {
                    door.RemoveLock();
                    count++;
                }
            }
        }

        arkFile.ReconstructBuffer();
        return count;
    }
}