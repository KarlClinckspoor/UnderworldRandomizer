using System.Text.Json.Serialization;

namespace UWRandomizerEditor.CMBdotDAT;

/// <summary>
/// An entry is a short where the first bit is whether it's destroyed or not and the remaining bits are the ItemID
/// </summary>
public class ItemDescriptor
{
    [JsonIgnore] public const int size = 2; // In bytes
    [JsonIgnore] public byte[] buffer;
    [JsonIgnore] private ushort entry;

    // TODO is Item ID 9 or 10 bits?
    public ushort itemID
    {
        get { return (ushort) Utils.GetBits(entry, 0x3FF, 0); }
        set { entry = (ushort) Utils.SetBits(entry, value, 0x3FF, 0); }
    }

    public bool IsDestroyed
    {
        get { return entry >> 15 == 1; }
        set { entry = (ushort) Utils.SetBits(entry, value ? 1 : 0, 0b1, 15); }
    }

    [JsonConstructor]
    public ItemDescriptor(ushort itemID, bool isDestroyed)
    {
        this.itemID = itemID;
        this.IsDestroyed = isDestroyed;
        buffer = BitConverter.GetBytes(entry);
    }

    public ItemDescriptor(byte[] buffer)
    {
        this.buffer = buffer;
        entry = BitConverter.ToUInt16(buffer);
    }

    public ItemDescriptor()
    {
        buffer = new byte[] {0, 0};
        entry = 0;
        itemID = 0;
        IsDestroyed = false;
    }
}
