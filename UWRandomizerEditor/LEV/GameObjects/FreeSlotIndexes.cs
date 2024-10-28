using UWRandomizerEditor.Interfaces;
using UWRandomizerEditor.LEV.Blocks;

namespace UWRandomizerEditor.LEV.GameObjects;

public class FreeSlotIndexes : IBufferObject
{
    public const int FixedSize = TileMapMasterObjectListBlock.FreeListStaticObjectsEntrySize;
    public byte[] Buffer { get; set; } = new byte[FixedSize];

    /// <summary>
    /// Index of the GameObject at a TileMapMasterObjectListBlock's AllGameObjects.
    /// Therefore, it can refer either to a StaticObject or MobileObject.
    /// </summary>
    public ushort IdxAtFullArray
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

    public FreeSlotIndexes(ushort idxAtFullArray, int entryNum)
    {
        IdxAtFullArray = idxAtFullArray;
        EntryNum = entryNum;
    }

    public bool ReconstructBuffer()
    {
        return true;
    }

    public override string ToString()
    {
        return $"FreeSlotNr{EntryNum}({IdxAtFullArray})";
    }
}