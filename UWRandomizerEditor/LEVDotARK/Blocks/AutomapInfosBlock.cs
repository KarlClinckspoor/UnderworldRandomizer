using System.Diagnostics;

namespace UWRandomizerEditor.LEVdotARK.Blocks;

public class AutomapInfosBlock : Block
{
    public new const int FixedBlockLength = 0; // found out this can be 0 in a new file

    public override bool ReconstructBuffer()
    {
        // Since there's no operations at the moment that can change the buffer, this will do nothing.
        return true;
    }

    public AutomapInfosBlock(byte[] buffer, int levelnumber)
    {
        Buffer = new byte[buffer.Length];
        buffer.CopyTo(Buffer, 0);
        LevelNumber = levelnumber;
    }
}