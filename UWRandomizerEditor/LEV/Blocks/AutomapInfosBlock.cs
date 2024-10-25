namespace UWRandomizerEditor.LEVdotARK.Blocks;

public class AutomapInfosBlock : Block
{
    public const int FixedBlockLength = 0; // found out this can be 0 in a new file

    private byte[] _buffer;
    
    public override byte[] Buffer
    {
        get
        {
            ReconstructBuffer(); 
            return _buffer;
        }
        set => _buffer = value;
    }

    public override bool ReconstructBuffer()
    {
        // Since there's no operations at the moment that can change the buffer, this will do nothing.
        return true;
    }

    public AutomapInfosBlock(byte[] buffer, int levelNumber)
    {
        _buffer = new byte[buffer.Length];
        buffer.CopyTo(_buffer, 0);
        LevelNumber = levelNumber;
    }
}