using UWRandomizerEditor.Interfaces;
using UWRandomizerEditor.LEV.Blocks;

namespace UWRandomizerEditor.LEV.GameObjects;

public class IdxOfFreeObj : IBufferObject
{
    public const int FixedSize = MapObjBlock.FreeListStaticObjectsEntrySize;
    public byte[] Buffer { get; set; } = new byte[FixedSize];

    /// <summary>
    /// Index of the GameObject at a TileMapMasterObjectListBlock's AllGameObjects.
    /// Therefore, it can refer either to a StaticObject or MobileObject.
    /// </summary>
    public ushort Value
    {
        get => BitConverter.ToUInt16(Buffer);
        set
        {
            Buffer = new byte[FixedSize];
            BitConverter.GetBytes(value).CopyTo(Buffer, 0);
        }
    }

    // TODO: is this field really necessary?
    public readonly int EntryNum;

    public IdxOfFreeObj(byte[] buffer, int entryNum)
    {
        if (Buffer.Length != FixedSize)
        {
            throw new ArgumentException($"Can't create Free List Object entry with buffer of size {buffer.Length} (expected {FixedSize})");
        }
        Buffer = buffer;
        EntryNum = entryNum;
    }

    public IdxOfFreeObj(ushort value, int entryNum)
    {
        Value = value;
        EntryNum = entryNum;
    }

    public bool ReconstructBuffer()
    {
        return true;
    }

    public override string ToString()
    {
        return $"FreeSlotNr{EntryNum}({Value})";
    }
}