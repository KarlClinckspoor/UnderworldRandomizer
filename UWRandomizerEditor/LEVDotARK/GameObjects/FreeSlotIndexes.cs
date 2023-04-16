using UWRandomizerEditor.Interfaces;
using UWRandomizerEditor.LEVdotARK.Blocks;

namespace UWRandomizerEditor.LEVdotARK.GameObjects;

public class FreeSlotIndexes : IBufferObject
{
    public const int FixedSize = TileMapMasterObjectListBlock.FreeListStaticObjectsEntrySize;
    public byte[] Buffer { get; set; } = new byte[FixedSize];

    public ushort IdxAtArray
    {
        get => BitConverter.ToUInt16(Buffer);
        set
        {
            Buffer = new byte[FixedSize];
            BitConverter.GetBytes(value).CopyTo(Buffer, 0);
        }
    }

    public readonly int EntryNum;

    public FreeSlotIndexes(byte[] buffer, int entryNum)
    {
        if (Buffer.Length != FixedSize)
        {
            throw new ArgumentException($"Can't create Free List Object entry with buffer of size {buffer.Length} (expected {FixedSize})");
        }
        Buffer = buffer;
        EntryNum = entryNum;
    }

    public FreeSlotIndexes(ushort idxAtArray, int entryNum)
    {
        IdxAtArray = idxAtArray;
        EntryNum = entryNum;
    }

    public bool ReconstructBuffer()
    {
        return true;
    }
}