using System.Diagnostics.CodeAnalysis;
using UWRandomizerEditor.Interfaces;
using UWRandomizerEditor.LEVdotARK.Blocks;

namespace UWRandomizerEditor.LEVdotARK;

/// <summary>
/// A small class that contains the header of a lev.ark file. The header is composed of a short with the number of
/// entries (blocks) followed by ints with the offsets to the start of each block in lev.ark
/// </summary>
public class Header : IBufferObject
{
    public byte[] Buffer { get; set; }
    public static int blockNumSize = 2; // a short
    public static int blockOffsetSize = 4; // int32

    public int[] BlockOffsets;
    public int[] OrderedBlockOffsets;

    public bool ReconstructBuffer()
    {
        // TODO: Ideally, this should get all the blocks... But I'm leaving it like this for now, for simplicity.
        // It's very unlikely that I'll change the block sizes in the near future.
        return true;
    }

    public int Size
    {
        get { return blockOffsetSize * NumEntries + blockNumSize; }
    }

    public static ushort NumEntriesFromBuffer(byte[] buffer)
    {
        return BitConverter.ToUInt16(buffer, 0);
    }

    public ushort NumEntries
    {
        get { return NumEntriesFromBuffer(Buffer); }
        set
        {
            if (value <= 0)
            {
                throw new ArgumentException("Number of entries has to be greater than 0!");
            }

            Console.WriteLine("Changing number of entries. Be careful!");
            byte[] newbuffer = new byte[value];
            BitConverter.GetBytes(value).CopyTo(newbuffer, 0);
            Buffer.CopyTo(newbuffer, 0);
            Buffer = newbuffer;
        }
    }

    public int GetOffsetForBlock(int blockNum)
    {
        // int blockOffset = BitConverter.ToInt32(buffer[(2 + 4 * blockNum)..(6 + 4 * blockNum)]);
        int blockOffset = BitConverter.ToInt32(Buffer, (blockNumSize + blockOffsetSize * blockNum));
        return blockOffset;
    }

    [MemberNotNull("BlockOffsets"), MemberNotNull("OrderedBlockOffsets")]
    private void CalculateOffsets()
    {
        BlockOffsets = new int[NumEntries];
        for (int i = 0; i < NumEntries; i++)
        {
            BlockOffsets[i] = GetOffsetForBlock(i);
        }

        var temp = new List<int>(BlockOffsets);
        temp.Sort();
        OrderedBlockOffsets = temp.ToArray();

    }

    public Header(byte[] buffer)
    {
        Buffer = buffer;
        CalculateOffsets();
    }
}