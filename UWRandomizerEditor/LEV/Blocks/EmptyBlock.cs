namespace UWRandomizerEditor.LEVdotARK.Blocks;

public class EmptyBlock : Block
{
    public override byte[] Buffer
    {
        get => Array.Empty<byte>();
        set => throw new BlockOperationException("Can't set a buffer to EmptyBlock");
    }
    public override bool ReconstructBuffer()
    {
        return true;
    }
}