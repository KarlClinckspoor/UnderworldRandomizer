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

    private MobileObject[] _mobileObjects = new MobileObject[MobileObjectNum];
    public MobileObject[] MobileObjects
    {
        get => _mobileObjects;
        private set
        {
            if (value.Length != MobileObjectNum) throw new ArgumentException($"Length of MobileObjects must be {MobileObjectNum}");
            _mobileObjects = value;
        }
    }

    private StaticObject[] _staticObjects = new StaticObject[StaticObjectNum];
    public StaticObject[] StaticObjects
    {
        get => _staticObjects;
        private set
        {
            if (value.Length != StaticObjectNum) throw new ArgumentException($"Length of StaticObjects must be {StaticObjectNum}");
            _staticObjects = value;
        }
    }

    private FreeSlotIndexes[] _freeMobileObjectSlots = new FreeSlotIndexes[FreeMobileObjectSlotsNumber];
    /// <summary>
    /// Contains a list of objects that point to the slot in MobileObjects that is "free", i.e. unused.
    /// </summary>
    /// <exception cref="ArgumentException">If the array length is different from <see cref="FreeMobileObjectSlotsNumber"/></exception>
    public FreeSlotIndexes[] FreeMobileObjectSlots
    {
        get => _freeMobileObjectSlots;
        private set
        {
            if (value.Length != FreeMobileObjectSlotsNumber)
                throw new ArgumentException($"Length of FreeMobileObjectSlots should be {FreeMobileObjectSlotsNumber}");
            _freeMobileObjectSlots = value;
        }
    }
    
    private FreeSlotIndexes[] _freeStaticObjectSlots = new FreeSlotIndexes[FreeStaticObjectSlotsNumber];
    /// <summary>
    /// Contains a list of objects that point to the slot in StaticObjects that is "free", i.e. unused.
    /// </summary>
    /// <exception cref="ArgumentException">If the array length is different from <see cref="FreeStaticObjectSlotsNumber"/></exception>
    public FreeSlotIndexes[] FreeStaticObjectSlots
    {
        get => _freeStaticObjectSlots;
        private set
        {
            if (value.Length != FreeStaticObjectSlotsNumber)
                throw new ArgumentException($"Length of FreeStaticObjectSlots should be {FreeStaticObjectSlotsNumber}");
            _freeStaticObjectSlots = value;
        }
        
    }
    /// <summary>
    /// Joins together <see cref="FreeMobileObjectSlots"/> and <see cref="FreeStaticObjectSlots"/> into one array.
    /// </summary>
    public FreeSlotIndexes[] AllFreeObjectSlots
    {
        get
        {
            var temp = new List<FreeSlotIndexes>();
            temp.AddRange(FreeMobileObjectSlots);
            temp.AddRange(FreeStaticObjectSlots);
            return temp.ToArray();
        }
    }

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

    /// <summary>
    /// This value represents the index of the first slot that points to a free mobile entry in the <see cref="AllGameObjects"/> array 
    /// </summary>
    public ushort FirstFreeMobileSlot
    {
        get => BitConverter.ToUInt16(_buffer, FirstFreeSlotInMobileSlotsOffset);
        set => BitConverter.GetBytes(value).CopyTo(_buffer, FirstFreeSlotInMobileSlotsOffset);
    }

    /// <summary>
    /// This value represents the index of the first slot that points to a free static entry in the <see cref="AllGameObjects"/> array 
    /// </summary>
    public ushort FirstFreeStaticSlot
    {
        get => BitConverter.ToUInt16(_buffer, FirstFreeSlotInStaticSlotsOffset);
        set => BitConverter.GetBytes(value).CopyTo(_buffer, FirstFreeSlotInStaticSlotsOffset);
    }

    /// <summary>
    /// The index in the <see cref="AllGameObjects"/> array (and <see cref="MobileObjects"/> array) that points to an unused <see cref="MobileObject"/> and can be freely modified.
    /// </summary>
    public ushort FirstFreeMobileObjectIdx => FreeMobileObjectSlots[FirstFreeMobileSlot].IdxAtFullArray;

    /// <summary>
    /// The index in the <see cref="AllGameObjects"/> array that points to an unused <see cref="StaticObject"/> and can be freely modified.
    /// </summary>
    public ushort FirstFreeStaticObjectIdx => FreeStaticObjectSlots[FirstFreeStaticSlot].IdxAtFullArray;

    /// <summary>
    /// The first <see cref="MobileObject"/> that can be freely modified.
    /// </summary>
    public MobileObject FirstFreeMobileObject => MobileObjects[FirstFreeMobileObjectIdx];
    /// <summary>
    /// The first <see cref="StaticObject"/> that can be freely modified.
    /// </summary>
    public StaticObject FirstFreeStaticObject => (StaticObject) AllGameObjects[FirstFreeStaticObjectIdx];

    // todo: Recheck and make sure the number of entries is correct.
    /// <summary>
    /// Goes through all the sub elements of this block (<see cref="Tile"/>s, <see cref="GameObject"/>s, etc) and
    /// gets all of their buffers (updated) then joins everything to update this block's buffer.
    /// </summary>
    /// <returns>True if the reconstruction was successful</returns>
    /// <exception cref="ConstraintException">Thrown if the buffer somehow has a different length than <see cref="FixedBlockLength"/></exception>
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
        BitConverter.GetBytes(FirstFreeMobileSlot).CopyTo(_buffer, FirstFreeSlotInMobileSlotsOffset);
        BitConverter.GetBytes(FirstFreeStaticSlot).CopyTo(_buffer, FirstFreeSlotInStaticSlotsOffset);
        BitConverter.GetBytes(EndOfBlockConfirmationValue).CopyTo(_buffer, EndOfBlockConfirmationOffset);
        if (_buffer.Length != FixedBlockLength)
        {
            throw new ConstraintException(
                $"Somehow the length of TileMapMasterObjectListBlock has the invalid length of {_buffer.Length}");
        }
        return true;
    }

    /// <summary>
    /// Reconstructs the <see cref="TileMapBuffer"/>, <see cref="MobileObjectInfoBuffer"/>, <see cref="StaticObjectInfoBuffer"/>,
    /// <see cref="FreeListMobileObjectBuffer"/>, <see cref="FreeListStaticObjectBuffer"/>.
    /// </summary>
    private void ReconstructSubBuffers()
    {
        ReconstructTileMapBuffer();
        ReconstructMobileObjectInfoBuffer();
        ReconstructStaticObjectInfoBuffer();
        ReconstructFreeListMobileObjectBuffer();
        ReconstructFreeListStaticObjectBuffer();
    }

    /// <summary>
    /// Reconstructs the <see cref="MobileObjectInfoBuffer"/>.
    /// </summary>
    private void ReconstructMobileObjectInfoBuffer()
    {
        // Let's use the object's own index to decide where it goes...
        foreach (var mobj in MobileObjects)
        {
            mobj.ReconstructBuffer();
            mobj.Buffer.CopyTo(MobileObjectInfoBuffer, mobj.IdxAtObjectArray * MobileObject.FixedTotalLength);
        }
    }

    /// <summary>
    /// Reconstructs the <see cref="StaticObjectInfoBuffer"/>.
    /// </summary>
    private void ReconstructStaticObjectInfoBuffer()
    {
        foreach (var obj in StaticObjects)
        {
            obj.ReconstructBuffer();
            obj.Buffer.CopyTo(StaticObjectInfoBuffer,
                (obj.IdxAtObjectArray - MobileObjectNum) * StaticObject.FixedTotalLength);
        }
    }

    /// <summary>
    /// Reconstructs the <see cref="FreeListStaticObjectBuffer"/>.
    /// </summary>
    private void ReconstructFreeListStaticObjectBuffer()
    {
        foreach (var obj in FreeStaticObjectSlots)
        {
            obj.ReconstructBuffer();
            obj.Buffer.CopyTo(FreeListStaticObjectBuffer, obj.EntryNum * FreeSlotIndexes.FixedSize);
        }
    }

    /// <summary>
    /// Reconstructs the <see cref="FreeListMobileObjectBuffer"/>.
    /// </summary>
    private void ReconstructFreeListMobileObjectBuffer()
    {
        foreach (var obj in FreeMobileObjectSlots)
        {
            obj.ReconstructBuffer();
            obj.Buffer.CopyTo(FreeListMobileObjectBuffer, obj.EntryNum * FreeSlotIndexes.FixedSize);
        }
    }

    /// <summary>
    /// Reconstructs the <see cref="TileMapBuffer"/>.
    /// </summary>
    private void ReconstructTileMapBuffer()
    {
        // Todo: Check that Tiles is the required length AND there aren't repeat indices.
        foreach (var currInfo in Tiles)
        {
            currInfo.ReconstructBuffer();
            currInfo.Buffer.CopyTo(TileMapBuffer, currInfo.Offset);
        }
    }

    /// <summary>
    /// Goes through <see cref="StaticObjectInfoBuffer"/>, separates it into chunks for each object, then
    /// sends the buffers to the GameObjectFactory to be converted into an object. Finally, stores the object
    /// in the <see cref="StaticObjects"/> array.
    /// </summary>
    /// <exception cref="BlockOperationException">Raised when an error occured in the data and somehow a StaticObject
    /// got placed in the MobileObjects array</exception>
    private void Populate_StaticObjectsFromBuffer()
    {
        for (short i = 0; i < StaticObjectNum; i++)
        {
            var currBuffer =
                StaticObjectInfoBuffer[
                    (i * StaticObject.FixedTotalLength)..((i + 1) * StaticObject.FixedTotalLength)];
            var currObj =
                (StaticObject) GameObjectFactory.CreateFromBuffer(currBuffer, (ushort) (i + MobileObjectNum));
            if (i < FirstFreeStaticObjectIdx - MobileObjectNum + 2) // +2 because of objs 0 and 1
                currObj.Invalid = true;

            if ((currObj.IdxAtObjectArray < MobileObjectNum) & (currObj.Invalid))
            {
                throw new BlockOperationException(
                    "Attempted to add a static object to the region of mobile objects. Should not happen!");
            }

            StaticObjects[i] = currObj;
        }
    }

    /// <summary>
    /// Goes through <see cref="MobileObjectInfoBuffer"/>, separates it into chunks for each object, then
    /// sends the buffers to the GameObjectFactory to be converted into an object. Finally, stores the object
    /// in the <see cref="MobileObjects"/> array.
    /// </summary>
    /// <exception cref="BlockOperationException">Raised when an error occured in the data and somehow a MobileObject
    /// got placed in the StaticObjects array</exception>
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
                throw new BlockOperationException(
                    "Attempted to add a static object to the region of mobile objects. Should not happen!");
            }

            MobileObjects[i] = currobj;
        }
    }

    private void Populate_FreeListMobileObjectArrFromBuffer()
    {
        for (int i = 0; i < FreeMobileObjectSlotsNumber; i++)
        {
            byte[] currbuffer =
                FreeListMobileObjectBuffer[
                    (i * FreeSlotIndexes.FixedSize)..((i + 1) * FreeSlotIndexes.FixedSize)];
            var currobj = new FreeSlotIndexes(currbuffer, i);
            FreeMobileObjectSlots[i] = currobj;
        }
    }

    private void Populate_FreeListStaticObjectArrFromBuffer()
    {
        for (int i = 0; i < FreeStaticObjectSlotsNumber; i++)
        {
            byte[] currbuffer =
                FreeListStaticObjectBuffer[
                    (i * FreeSlotIndexes.FixedSize)..((i + 1) * FreeSlotIndexes.FixedSize)];
            var currobj = new FreeSlotIndexes(currbuffer, i);
            FreeStaticObjectSlots[i] = currobj;
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
            if (gameObject.ReferenceCount < 1) continue; // Only considering containers that were placed in Tiles.
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