namespace UWRandomizerEditor.LEV.Blocks;

public sealed class TextureMappingBlock : Block
{
    public new const int FixedBlockLength = 0x007a;

    private byte[] _buffer;
    public override byte[] Buffer 
    {
        get
        {
            ReconstructBuffer();
            return _buffer;
        }
        set
        {
            if (value.Length != FixedBlockLength)
            {
                throw new BlockOperationException(
                    $"New buffer length of {value.Length} is incompatible with required length of {FixedBlockLength}");
            }

            _buffer = value;
        } 
    }

    public override bool ReconstructBuffer()
    {
        // Since there's no operations that can change this block, this doesn't do anything.
        return true;
    }

    public TextureMappingBlock(byte[] buffer, int levelNumber)
    {
        if (buffer.Length != FixedBlockLength)
        {
            throw new ArgumentException(
                $"Length of buffer ({buffer.Length}) is incompatible with expected TextureMappingBlock length ({FixedBlockLength})");
        }

        _buffer = new byte[FixedBlockLength];
        buffer.CopyTo(_buffer, 0);
        LevelNumber = levelNumber;
    }
}