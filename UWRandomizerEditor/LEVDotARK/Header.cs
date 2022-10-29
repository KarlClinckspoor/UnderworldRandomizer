using UWRandomizerEditor.Interfaces;

namespace UWRandomizerEditor.LEVDotARK;
using static Utils;

/// <summary>
/// A small class that contains the header of a lev.ark file. The header is composed of a short with the number of
/// entries (blocks) followed by ints with the offsets to the start of each block in lev.ark
/// </summary>
public class Header: ISaveBinary
{
    public byte[] buffer;
    public static int blockNumSize = 2;    // a short
    public static int blockOffsetSize = 4; // int32

    public int[] BlockOffsets;

    public int Size
    {
        get { return blockOffsetSize * NumEntries + blockNumSize; }
    }

    public static short NumEntriesFromBuffer(byte[] buffer)
    {
        return BitConverter.ToInt16(buffer, 0);  
    }

    public short NumEntries
    {
        get { return NumEntriesFromBuffer(buffer); }
        set
        {
            if (value <= 0)
            {
                throw new ArgumentException("Number of entries has to be greater than 0!");
            }
            Console.WriteLine("Changing number of entries. Be careful!");
            byte[] newbuffer = new byte[value];
            BitConverter.GetBytes(value).CopyTo(newbuffer, 0);
            buffer.CopyTo(newbuffer, 0);
            buffer = newbuffer;
        }
    }

    public int GetOffsetForBlock(int blockNum)
    {
        // int blockOffset = BitConverter.ToInt32(buffer[(2 + 4 * blockNum)..(6 + 4 * blockNum)]);
        int blockOffset =  BitConverter.ToInt32(buffer, (blockNumSize + blockOffsetSize * blockNum));
        return blockOffset;
    }

    public void CalculateOffsets()
    {
        BlockOffsets = new int[NumEntries];
        for (int i = 0; i < NumEntries; i++)
        {
            BlockOffsets[i] = GetOffsetForBlock(i);
        }
    }

    public Header(byte[] buffer)
    {
        this.buffer = buffer;
        CalculateOffsets();
    }

    public string SaveBuffer(string basePath = "D:\\Dropbox\\UnderworldStudy\\studies\\LEV.ARK", string filename = "")
    {
        if (filename.Length == 0)
        {
            filename = $@"_HEADER";
        }

        return StdSaveBuffer(buffer, basePath, filename);
    }
}