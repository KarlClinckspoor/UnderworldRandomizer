namespace UWRandomizerEditor.LEV.Blocks;

public class ObjectAnimationOverlayInfoBlock : Block
{
    public new const int FixedBlockLength = 0x0180;

    /*
    This block contains entries with length of 6 bytes with infos about
    objects with animation overlay images from "animo.gr".
    It always is 0x0180 bytes long which leads to 64 entries.
 
    0000   Int16   link1
    0002   Int16   unk2
    0004   Int8    tile x coordinate
    0005   Int8    tile y coordinate
 
    link1's most significant 10 bits contain a link into the master object
    list, to the object that should get an animation overlay.
    */

    private byte[] _buffer = new byte[FixedBlockLength];
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

    public ObjectAnimationOverlayInfoBlock(byte[] buffer, int levelNumber)
    {
        if (buffer.Length != FixedBlockLength)
        {
            throw new ArgumentException(
                $"Length of buffer ({buffer.Length}) is incompatible with expected ObjectAnimationOverlayInfo length ({FixedBlockLength})");
        }

        Buffer = new byte[FixedBlockLength];
        buffer.CopyTo(Buffer, 0);
        LevelNumber = levelNumber;
    }
}