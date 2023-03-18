using UWRandomizerEditor.Interfaces;
using UWRandomizerEditor.LEVdotARK.Blocks;

namespace UWRandomizerEditor.LEVdotARK.GameObjects;

public class FreeListObjectEntry : IBufferObject
{
    // FreeListMobileObjectEntrySize is also 4 (short).
    public const int FixedSize = TileMapMasterObjectListBlock.FreeListStaticObjectsEntrySize;
    public byte[] Buffer { get; set; } = new byte[FixedSize];

    public ushort IdxAtArray
    {
        get
        {
            return BitConverter.ToUInt16(Buffer);
        }
        set
        {
            Buffer = new byte[FixedSize];
            BitConverter.GetBytes(value).CopyTo(Buffer, 0);
        }
    }

    public readonly int EntryNum;

    public FreeListObjectEntry(byte[] buffer, int EntryNum)
    {
        if (Buffer.Length != FixedSize)
        {
            throw new ArgumentException($"Can't create Free List Object entry with buffer of size {buffer.Length} (expected {FixedSize})");
        }
        Buffer = buffer;
        this.EntryNum = EntryNum;
    }

    public FreeListObjectEntry(ushort idxAtArray, int EntryNum)
    {
        this.IdxAtArray = idxAtArray;
        this.EntryNum = EntryNum;
    }

    public bool ReconstructBuffer()
    {
        return true;
    }
}