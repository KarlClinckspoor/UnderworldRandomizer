using UWRandomizerEditor.Interfaces;
using UWRandomizerEditor.LEVDotARK.Blocks;

namespace UWRandomizerEditor.LEVDotARK.GameObjects;

public class FreeListObjectEntry : IBufferObject
{
    // FreeListMobileObjectEntrySize is also 4 (short).
    public const int FixedSize = TileMapMasterObjectListBlock.FreeListStaticObjectsEntrySize;
    public byte[] Buffer { get; set; } = new byte[FixedSize];
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
        ReconstructBuffer();
    }

    public FreeListObjectEntry() // For non-entries.
    {
    }

    public bool ReconstructBuffer()
    {
        Buffer = BitConverter.GetBytes(Entry);
        return true;
    }

    public void UpdateEntry()
    {
        Entry = BitConverter.ToInt16(Buffer);
    }
}