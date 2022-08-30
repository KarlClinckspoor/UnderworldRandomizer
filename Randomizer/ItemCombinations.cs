using System.Diagnostics.CodeAnalysis;
using static Randomizer.Utils;
namespace Randomizer;

public class ItemCombinations
{
    
}

/// <summary>
/// An entry is a short where the first bit is whether it's destroyed or not and the remaining bits are the ItemID
/// </summary>
public class ItemCombinationEntry
{
    public int length = 2;
    public byte[] buffer;
    private short entry;

    private short itemID;
    private bool IsDestroyed;

    public ItemCombinationEntry(short itemID, bool isDestroyed)
    {
        this.itemID = itemID;
        this.IsDestroyed = isDestroyed;

        entry |= itemID;

        if (isDestroyed)
        {
            entry |= 0x80;
        }
        
        UpdateBuffer();
    }

    public ItemCombinationEntry(byte[] buffer)
    {
        this.buffer = buffer;
        UpdateEntry();
    }

    public ItemCombinationEntry()
    {
        buffer = new byte[] {0, 0};
        entry = 0;
        itemID = 0;
        IsDestroyed = false;
    }

    [MemberNotNull(nameof(buffer))] // Tells compiler UpdateBuffer assures buffer isn't null.
    public void UpdateBuffer()
    {
        buffer = BitConverter.GetBytes(entry);
    }

    public void UpdateEntry()
    {
        entry = BitConverter.ToInt16(buffer);

        // IsDestroyed = (buffer[0] & 0x8) == 1;
        IsDestroyed = GetBits(entry, 0x80, 0) == 1; // TODO: check this, I always forget how my functions work.

        // Removing first bit by shifting left and right. Note that shifts convert into ints!
        itemID = (short) ( ((short) (entry << 1)) >> 1);

        // Is this easier?
        itemID = (short) GetBits(entry, 0x7FFF, 0);
    }
}

public class FinalEntry : ItemCombinationEntry
{
    public new byte[] buffer = {0, 0};
    private short entry = 0;
    private short itemID = 0;
    private bool IsDestroyed = false;

    public FinalEntry(short itemID, bool isDestroyed): this()
    { }

    public FinalEntry(): base()
    { }

}