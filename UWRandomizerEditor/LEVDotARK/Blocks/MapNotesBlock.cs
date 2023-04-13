namespace UWRandomizerEditor.LEVdotARK.Blocks;

/// <summary>
/// Block that stores map notes. Its length is 0 if there are no block notes
/// </summary>
public class MapNotesBlock : Block
{
    public new uint FixedBlockLength = 0;
    public MapNotesBlock(byte[] buffer, int levelnumber)
    {
        _buffer = new byte[buffer.Length];
        buffer.CopyTo(_buffer, 0);
        LevelNumber = levelnumber;
    }

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
        // Since there's no operations that can change the buffer, this won't do anything.
        return true;
    }
}