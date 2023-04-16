using System.Data;
using System.Diagnostics;
using UWRandomizerEditor.LEVdotARK.GameObjects;
using UWRandomizerEditor.LEVdotARK.GameObjects.Specifics;
// ReSharper disable AccessToStaticMemberViaDerivedType

namespace UWRandomizerEditor.LEVdotARK.Blocks;

public partial class TileMapMasterObjectListBlock : Block
{
    /// <summary>
    /// Contains all tiles in a continuous array
    /// </summary>
    public Tile[] Tiles = new Tile[TileMapLength / TileMapEntrySize];
    
    /// <summary>
    /// Reshapes <see cref="Tiles"/> into a <see cref="TileWidth"/> by <see cref="TileHeight"/> array on the fly, for convenience.
    /// </summary>
    public Tile[,] Tiles2D => Utils.ReshapeArray(Tiles, TileWidth, TileHeight);

    /// <summary>
    /// This conveniently joins the Mobile Object and Static Object arrays into one, from 0 to <see cref="MobileObjectNum"/>+<see cref="StaticObjectNum"/>
    /// </summary>
    public GameObject[] AllGameObjects
    {
        get
        {
            var temp = new List<GameObject>();
            temp.AddRange(MobileObjects);
            temp.AddRange(StaticObjects);
            return temp.ToArray();
        }
    }

    public MobileObject[] MobileObjects = new MobileObject[MobileObjectNum];
    public StaticObject[] StaticObjects = new StaticObject[StaticObjectNum];

    public FreeListObjectEntry[] FreeListMobileObjects = new FreeListObjectEntry[FreeListMobileObjectsNum];
    public FreeListObjectEntry[] FreeListStaticObjects = new FreeListObjectEntry[FreeListStaticObjectsNum];

    private byte[] _buffer;
    /// <summary>
    /// Controls the byte buffer of the object and keeps it updated with the function ReconstructBuffer.
    /// </summary>
    /// <exception cref="BlockOperationException">thrown if the buffer being set has an incorrect length (<see cref="FixedBlockLength"/>)</exception>
    public override byte[] Buffer
    {
        get { 
            ReconstructBuffer();
            return _buffer;
        }
        set
        {
            if (value.Length != FixedBlockLength)
            {
                throw new BlockOperationException(
                    $"New buffer length of {value.Length} is incompatible with required length of {FixedBlockLength}");
            }

            _buffer = value;
        }
    }

    public ushort FirstFreeSlotInMobileList
    {
        get => BitConverter.ToUInt16(_buffer, NumEntriesMobileFreeListAdjOffset);
        set => BitConverter.GetBytes(value).CopyTo(_buffer, NumEntriesMobileFreeListAdjOffset);
    }

    public ushort FirstFreeSlotInStaticList
    {
        get { return BitConverter.ToUInt16(_buffer, NumEntriesStaticFreeListAdjOffset); }
        set { BitConverter.GetBytes(value).CopyTo(_buffer, NumEntriesStaticFreeListAdjOffset); }
    }

    public ushort FirstFreeMobileObjectIdx => FreeListMobileObjects[FirstFreeSlotInMobileList].IdxAtArray;
    public ushort FirstFreeStaticObjectIdx => FreeListStaticObjects[FirstFreeSlotInStaticList].IdxAtArray; // TODO: Is this +1?

    // todo: Recheck and make sure the number of entries is correct.
    public override bool ReconstructBuffer()
    {
        if (_buffer.Length != FixedBlockLength)
        {
            throw new ConstraintException(
                $"Somehow the length of TileMapMasterObjectListBlock has the invalid length of {_buffer.Length}");
        }

        ReconstructSubBuffers();

        TileMapBuffer.CopyTo(_buffer, TileMapOffset);
        MobileObjectInfoBuffer.CopyTo(_buffer, MobileObjectInfoOffset);
        StaticObjectInfoBuffer.CopyTo(_buffer, StaticObjectInfoOffset);
        FreeListMobileObjectBuffer.CopyTo(_buffer, FreeListMobileObjectsOffset);
        FreeListStaticObjectBuffer.CopyTo(_buffer, FreeListStaticObjectsOffset);
        UnknownBuffer.CopyTo(_buffer, UnknownOffset);
        Unknown2Buffer.CopyTo(_buffer, Unknown2Offset);
        // todo: do I really need these 2? Seems this is always kept updated.
        BitConverter.GetBytes(FirstFreeSlotInMobileList).CopyTo(_buffer, NumEntriesMobileFreeListAdjOffset);
        BitConverter.GetBytes(FirstFreeSlotInStaticList).CopyTo(_buffer, NumEntriesStaticFreeListAdjOffset);
        BitConverter.GetBytes(EndOfBlockConfirmationValue).CopyTo(_buffer, EndOfBlockConfirmationOffset);
        return true;
    }

    private void ReconstructSubBuffers()
    {
        ReconstructTileMapBuffer();
        // TODO: These should ideally check the validity of each object and rearrange them so they're ordered
        // from the end, and the free lists should accomodate that.
        ReconstructMobileObjectInfoBuffer();
        ReconstructStaticObjectInfoBuffer();
        ReconstructFreeListMobileObjectBuffer();
        ReconstructFreeListStaticObjectBuffer();
    }

    private void ReconstructMobileObjectInfoBuffer()
    {
        // Let's use the object's own index to decide where it goes...
        foreach (var mobj in MobileObjects)
        {
            mobj.ReconstructBuffer();
            mobj.Buffer.CopyTo(MobileObjectInfoBuffer, mobj.IdxAtObjectArray * MobileObject.FixedTotalLength);
        }
    }

    private void ReconstructStaticObjectInfoBuffer()
    {
        foreach (var obj in StaticObjects)
        {
            obj.ReconstructBuffer();
            obj.Buffer.CopyTo(StaticObjectInfoBuffer,
                (obj.IdxAtObjectArray - MobileObjectNum) * StaticObject.FixedTotalLength);
        }
    }

    private void ReconstructFreeListStaticObjectBuffer()
    {
        foreach (var obj in FreeListStaticObjects)
        {
            obj.ReconstructBuffer();
            obj.Buffer.CopyTo(FreeListStaticObjectBuffer, obj.EntryNum * FreeListObjectEntry.FixedSize);
        }
    }

    private void ReconstructFreeListMobileObjectBuffer()
    {
        foreach (var obj in FreeListMobileObjects)
        {
            obj.ReconstructBuffer();
            obj.Buffer.CopyTo(FreeListMobileObjectBuffer, obj.EntryNum * FreeListObjectEntry.FixedSize);
        }
    }

    private void ReconstructTileMapBuffer()
    {
        // Todo: Check that Tiles is the required length AND there aren't repeat indices.
        foreach (Tile currInfo in Tiles)
        {
            currInfo.ReconstructBuffer();
            currInfo.Buffer.CopyTo(TileMapBuffer, currInfo.Offset);
        }
    }

    private void Populate_StaticObjectsFromBuffer()
    {
        for (short i = 0; i < StaticObjectNum; i++)
        {
            byte[] currbuffer =
                StaticObjectInfoBuffer[
                    (i * StaticObject.FixedTotalLength)..((i + 1) * StaticObject.FixedTotalLength)];
            var currobj =
                (StaticObject) GameObjectFactory.CreateFromBuffer(currbuffer, (ushort) (i + MobileObjectNum));
            if (i < FirstFreeStaticObjectIdx - MobileObjectNum + 2) // +2 because of objs 0 and 1
                currobj.Invalid = true;

            if ((currobj.IdxAtObjectArray < MobileObjectNum) & (currobj.Invalid))
            {
                throw new Exception(
                    "Attempted to add a static object to the region of mobile objects. Should not happen!");
            }

            StaticObjects[i] = currobj;
            AllGameObjects[currobj.IdxAtObjectArray] = currobj;
        }
    }

    // todo: consider also providing an entry number, for safekeeping
    private void Populate_MobileObjectsFromBuffer()
    {
        for (ushort i = 0; i < MobileObjectNum; i++)
        {
            byte[] currbuffer =
                MobileObjectInfoBuffer[
                    (i * MobileObject.FixedTotalLength)..((i + 1) * MobileObject.FixedTotalLength)];
            var currobj = (MobileObject) GameObjectFactory.CreateFromBuffer(currbuffer, i);
            if (i <= FirstFreeMobileObjectIdx)
                currobj.Invalid = true;
                
            if (currobj.IdxAtObjectArray >= MobileObjectNum)
            {
                throw new Exception(
                    "Attempted to add a static object to the region of mobile objects. Should not happen!");
            }

            MobileObjects[i] = currobj;
            AllGameObjects[currobj.IdxAtObjectArray] = currobj;
        }
    }

    private void Populate_FreeListMobileObjectArrFromBuffer()
    {
        for (int i = 0; i < FreeListMobileObjectsNum; i++)
        {
            byte[] currbuffer =
                FreeListMobileObjectBuffer[
                    (i * FreeListObjectEntry.FixedSize)..((i + 1) * FreeListObjectEntry.FixedSize)];
            var currobj = new FreeListObjectEntry(currbuffer, i);
            FreeListMobileObjects[i] = currobj;
        }
    }

    private void Populate_FreeListStaticObjectArrFromBuffer()
    {
        for (int i = 0; i < FreeListStaticObjectsNum; i++)
        {
            byte[] currbuffer =
                FreeListStaticObjectBuffer[
                    (i * FreeListObjectEntry.FixedSize)..((i + 1) * FreeListObjectEntry.FixedSize)];
            var currobj = new FreeListObjectEntry(currbuffer, i);
            FreeListStaticObjects[i] = currobj;
        }
    }

    public TileMapMasterObjectListBlock(byte[] buffer, int levelNumber)
    {
        if (buffer.Length != FixedBlockLength)
        {
            throw new ArgumentException(
                $"Length of buffer ({buffer.Length}) is incompatible with expected TextureMappingBlock length ({FixedBlockLength})");
        }

        _buffer = new byte[FixedBlockLength];
        buffer.CopyTo(_buffer, 0);
        LevelNumber = levelNumber;

        TileMapBuffer = buffer[TileMapOffset..(TileMapOffset + TileMapLength)];
        MobileObjectInfoBuffer = buffer[MobileObjectInfoOffset..(MobileObjectInfoOffset + MobileObjectInfoLength)];
        StaticObjectInfoBuffer = buffer[StaticObjectInfoOffset..(StaticObjectInfoOffset + StaticObjectInfoLength)];
        FreeListMobileObjectBuffer =
            buffer[FreeListMobileObjectsOffset..(FreeListMobileObjectsOffset + FreeListMobileObjectsLength)];
        FreeListStaticObjectBuffer =
            buffer[FreeListStaticObjectsOffset..(FreeListStaticObjectsOffset + FreeListStaticObjectsLength)];
        UnknownBuffer = buffer[UnknownOffset..(UnknownOffset + UnknownLength)];
        Unknown2Buffer = buffer[Unknown2Offset..(Unknown2Offset + Unknown2Length)];

        Populate_FreeListMobileObjectArrFromBuffer();
        Populate_FreeListStaticObjectArrFromBuffer();
        Populate_MobileObjectsFromBuffer();
        Populate_StaticObjectsFromBuffer();
        Populate_TileInfos(); // This requires a complete array of game objects, so it comes last
        Populate_Containers();
    }

    private void Populate_TileInfos()
    {
        for (uint i = 0; i < TileMapLength / TileMapEntrySize; i++)
        {
            var offset = i * TileMapEntrySize;
            // Todo: Seems a bit weird to convert and de-convert later. Think better.
            var entry = BitConverter.ToUInt32(TileMapBuffer, (int) offset);
            Tile currTile = new Tile(i, entry, offset, LevelNumber);

            Tiles[i] = currTile;
            currTile.ObjectChain.PopulateObjectList(AllGameObjects);
        }
    }

    private void Populate_Containers()
    {
        foreach (var gameObject in AllGameObjects)
        {
            if (gameObject.Invalid) continue;
            if (gameObject is Container cont)
            {
                cont.Contents.PopulateObjectList(AllGameObjects);
            }

            if (gameObject is MobileObject mobileObject)
            {
                mobileObject.Inventory.PopulateObjectList(AllGameObjects);
            }
        }
    }


    private byte[] _tileMapBuffer = new byte[TileMapLength];

    public byte[] TileMapBuffer
    {
        get { return _tileMapBuffer; }
        set
        {
            if (value.Length != TileMapLength)
            {
                throw new ArgumentException($"Invalid length of TileMapBuffer");
            }

            value.CopyTo(_tileMapBuffer, 0);
        }
    }

    private byte[] _mobileObjectInfoBuffer = new byte[MobileObjectInfoLength];

    public byte[] MobileObjectInfoBuffer
    {
        get { return _mobileObjectInfoBuffer; }
        set
        {
            if (value.Length != MobileObjectInfoLength)
            {
                throw new ArgumentException($"Invalid length of MobileObjectInfoBuffer");
            }

            value.CopyTo(_mobileObjectInfoBuffer, 0);
        }
    }

    private byte[] _staticObjectInfoBuffer = new byte[StaticObjectInfoLength];

    public byte[] StaticObjectInfoBuffer
    {
        get { return _staticObjectInfoBuffer; }
        set
        {
            if (value.Length != StaticObjectInfoLength)
            {
                throw new ArgumentException($"Invalid length of StaticObjectInfoBuffer");
            }

            value.CopyTo(_staticObjectInfoBuffer, 0);
        }
    }

    private byte[] _freeListMobileObjectBuffer = new byte[FreeListMobileObjectsLength];

    public byte[] FreeListMobileObjectBuffer
    {
        get { return _freeListMobileObjectBuffer; }
        set
        {
            if (value.Length != FreeListMobileObjectsLength)
            {
                throw new ArgumentException($"Invalid length of FreeListMobileObjectBuffer");
            }

            value.CopyTo(_freeListMobileObjectBuffer, 0);
        }
    }

    private byte[] _freeListStaticObjectBuffer = new byte[FreeListStaticObjectsLength];

    public byte[] FreeListStaticObjectBuffer
    {
        get { return _freeListStaticObjectBuffer; }
        set
        {
            if (value.Length != FreeListStaticObjectsLength)
            {
                throw new ArgumentException($"Invalid length of FreeListStaticObjectBuffer");
            }

            value.CopyTo(_freeListStaticObjectBuffer, 0);
        }
    }

    private byte[] _unknownBuffer = new byte[UnknownLength];

    protected byte[] UnknownBuffer
    {
        get { return _unknownBuffer; }
        set
        {
            if (value.Length != UnknownLength)
            {
                throw new ArgumentException($"Invalid length");
            }

            value.CopyTo(_unknownBuffer, 0);
        }
    }

    private byte[] _unknown2Buffer = new byte[Unknown2Length];

    protected byte[] Unknown2Buffer
    {
        get { return _unknown2Buffer; }
        set
        {
            if (value.Length != Unknown2Length)
            {
                throw new ArgumentException($"Invalid length");
            }

            value.CopyTo(_unknown2Buffer, 0);
        }
    }
}