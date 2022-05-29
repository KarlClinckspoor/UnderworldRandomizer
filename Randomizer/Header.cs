namespace Randomizer;
using static Utils;

public class Header: ISaveBinary
{
    public byte[] buffer;
    public static int blockNumSize = 2;
    public static int blockOffsetSize = 4; // offsets are int32

    public int[] BlockOffsets;

    public int Size
    {
        get { return blockOffsetSize * NumEntries; }
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

    public bool SaveBuffer(string basepath = Settings.DefaultArkPath, string extrainfo = "")
    {
        if (extrainfo.Length == 0)
        {
            extrainfo = $@"_HEADER";
        }

        return StdSaveBuffer(buffer, basepath, extrainfo);
    }
}