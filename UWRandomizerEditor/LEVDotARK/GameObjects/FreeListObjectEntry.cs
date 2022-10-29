using UWRandomizerEditor.LEVDotARK.Blocks;

namespace UWRandomizerEditor.LEVDotARK.GameObjects;

public class FreeListObjectEntry
{
    // FreeListMobileObjectEntrySize is also 4 (short).
    public const int EntrySize = TileMapMasterObjectListBlock.FreeListStaticObjectsEntrySize;
    public byte[] Buffer = new byte[EntrySize];
    public short Entry;
    public int EntryNum;

    public FreeListObjectEntry(byte[] buffer, int EntryNum)
    {
        // Debug.Assert(buffer.Length == TileMapMasterObjectListBlock.FreeListMobileObjectsEntrySize);
        this.Buffer = buffer;
        this.EntryNum = EntryNum;
        UpdateEntry();
    }

    public FreeListObjectEntry(short Entry, int EntryNum)
    {
        this.Entry = Entry;
        this.EntryNum = EntryNum;
        UpdateBuffer();
    }

    public FreeListObjectEntry() // For non-entries.
    { }

    public void UpdateBuffer()
    {
        Buffer = BitConverter.GetBytes(Entry);
    }

    public void UpdateEntry()
    {
        Entry = BitConverter.ToInt16(Buffer);
    }
}