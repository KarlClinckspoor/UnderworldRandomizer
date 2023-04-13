using UWRandomizerEditor.Interfaces;

namespace UWRandomizerEditor.LEVdotARK.Blocks;

public abstract class Block : IBufferObject
{
    public abstract byte[] Buffer { get; set; }
    public abstract bool ReconstructBuffer();
    public int LevelNumber = -1;
}