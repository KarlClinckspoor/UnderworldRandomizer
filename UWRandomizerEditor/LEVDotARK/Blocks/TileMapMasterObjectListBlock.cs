using System.Data;
using UWRandomizerEditor.LEVdotARK.GameObjects;

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
    /// <summary>
    /// Contains all the <see cref="MobileObject"/>s in the level, including objects 0 and 1, which
    /// should never be accessed.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown in case the new array length is different from <see cref="MobileObjectNum"/></exception>
    public MobileObject[] MobileObjects
    {
        get => _mobileObjects;
        private set
        {
            if (value.Length != MobileObjectNum)
                throw new ArgumentException($"Length of MobileObjects must be {MobileObjectNum}");
            _mobileObjects = value;
        }
    }

    private StaticObject[] _staticObjects = new StaticObject[StaticObjectNum];
    /// <summary>
    /// Contains all the <see cref="StaticObject"/>s in the level.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown in case the new array length is different from <see cref="StaticObjectNum"/></exception>
    public StaticObject[] StaticObjects
    {
        get => _staticObjects;
        private set
        {
            if (value.Length != StaticObjectNum)
                throw new ArgumentException($"Length of StaticObjects must be {StaticObjectNum}");
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
        get
        {
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
            throw new BlockOperationException(
                $"Somehow the length of TileMapMasterObjectListBlock has the invalid length of {_buffer.Length}");
        }

        if (StaticObjects.Length != StaticObjectNum)
        {
            throw new BlockOperationException(
                $"Somehow there's more static objects ({StaticObjects.Length}) than the max ({StaticObjectNum})");
        }

        if (MobileObjects.Length != MobileObjectNum)
        {
            throw new BlockOperationException(
                $"Somehow there's more mobile objects ({MobileObjects.Length}) than the max ({MobileObjectNum})");
        }

        if (Tiles.Length != (TileWidth * TileHeight))
        {
            throw new BlockOperationException(
                $"Somehow there's more tiles ({Tiles.Length}) than the max ({TileWidth * TileHeight}");
        }

        if (UnknownBuffer.Length != UnknownLength)
        {
            throw new BlockOperationException(
                $"Somehow UnknownBuffer length ({UnknownBuffer.Length}) is different from {UnknownLength}");
        }

        if (Unknown2Buffer.Length != Unknown2Length)
        {
            throw new BlockOperationException(
                $"Somehow Unknown2Buffer length ({Unknown2Buffer.Length}) is different from {Unknown2Length}");
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
        foreach (var obj in MobileObjects)
        {
            obj.ReconstructBuffer();
            obj.Buffer.CopyTo(MobileObjectInfoBuffer, obj.IdxAtObjectArray * MobileObject.FixedTotalLength);
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
    /// <exception cref="BlockOperationException">if the length is different from <see cref="TileHeight"/>*<see cref="TileWidth"/></exception>
    private void ReconstructTileMapBuffer()
    {
        if (Tiles.Length != TileHeight * TileWidth)
        {
            throw new BlockOperationException(
                $"Current number of tiles ({Tiles.Length}) is different from required length {TileHeight * TileWidth}");
        }

        foreach (var tile in Tiles)
        {
            tile.ReconstructBuffer();
            tile.Buffer.CopyTo(_tileMapBuffer, tile.Offset);
        }
    }

    /// <summary>
    /// Goes through <see cref="StaticObjectInfoBuffer"/>, separates it into chunks for each object, then
    /// sends the buffers to the GameObjectFactory to be converted into an object. Finally, stores the object
    /// in the <see cref="StaticObjects"/> array.
    /// </summary>
    /// <exception cref="BlockOperationException">Raised when an error occured in the data and somehow a StaticObject
    /// got placed in the MobileObjects array</exception>
    private void PopulateStaticObjectsFromBuffer()
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
    private void PopulateMobileObjectsFromBuffer()
    {
        for (ushort i = 0; i < MobileObjectNum; i++)
        {
            byte[] buffer =
                MobileObjectInfoBuffer[
                    (i * MobileObject.FixedTotalLength)..((i + 1) * MobileObject.FixedTotalLength)];
            var obj = (MobileObject) GameObjectFactory.CreateFromBuffer(buffer, i);
            if (i <= FirstFreeMobileObjectIdx)
                obj.Invalid = true;

            if (obj.IdxAtObjectArray >= MobileObjectNum)
            {
                throw new BlockOperationException(
                    "Attempted to add a static object to the region of mobile objects. Should not happen!");
            }

            MobileObjects[i] = obj;
        }
    }

    /// <summary>
    /// Goes through the buffer and creates the FreeSlotIndexes at the appropriate locations.
    /// </summary>
    private void PopulateFreeListMobileObjectArrFromBuffer()
    {
        for (int i = 0; i < FreeMobileObjectSlotsNumber; i++)
        {
            var buffer =
                FreeListMobileObjectBuffer[(i * FreeSlotIndexes.FixedSize)..((i + 1) * FreeSlotIndexes.FixedSize)];
            var obj = new FreeSlotIndexes(buffer, i);
            FreeMobileObjectSlots[i] = obj;
        }
    }

    /// <summary>
    /// Goes through the buffer and creates the FreeSlotIndexes at the appropriate locations.
    /// </summary>
    private void PopulateFreeListStaticObjectArrFromBuffer()
    {
        for (int i = 0; i < FreeStaticObjectSlotsNumber; i++)
        {
            var buffer =
                FreeListStaticObjectBuffer[(i * FreeSlotIndexes.FixedSize)..((i + 1) * FreeSlotIndexes.FixedSize)];
            var obj = new FreeSlotIndexes(buffer, i);
            FreeStaticObjectSlots[i] = obj;
        }
    }

    /// <summary>
    /// Instantiates a new TileMapMasterObjectListBlock based upon the buffer and the level number.
    /// </summary>
    /// <param name="buffer">Byte buffer containing the data. Must be <see cref="FixedBlockLength"/> bytes long.</param>
    /// <param name="levelNumber">Level number, for convenience. Isn't required for anything.</param>
    /// <exception cref="ArgumentException">Thrown in case the length of buffer is different from <see cref="FixedBlockLength"/>.</exception>
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

        PopulateFreeListMobileObjectArrFromBuffer();
        PopulateFreeListStaticObjectArrFromBuffer();
        PopulateMobileObjectsFromBuffer();
        PopulateStaticObjectsFromBuffer();
        PopulateTiles();
        AddObjectsToTiles();
        AddObjectsToContainers();
    }

    /// <summary>
    /// Creates tiles for the buffer
    /// </summary>
    private void PopulateTiles()
    {
        for (uint i = 0; i < TileMapLength / TileMapEntrySize; i++)
        {
            var offset = i * TileMapEntrySize;
            var entry = BitConverter.ToUInt32(TileMapBuffer, (int) offset);
            var currTile = new Tile(i, entry, offset, LevelNumber);
            Tiles[i] = currTile;
        }
    }

    /// <summary>
    /// Iterates through all tiles and adds the objects to the tile's internal ObjectChain linked list.
    /// Should not have any side effects.
    /// </summary>
    private void AddObjectsToTiles()
    {
        foreach (var tile in Tiles)
        {
            tile.ObjectChain.PopulateObjectList(AllGameObjects);
        }
    }

    /// <summary>
    /// <para>
    /// Goes through all objects that implement IContainer and adds GameObjects to its internal linked list.
    /// This function has the following logic.
    /// </para>
    /// <list type="number">
    ///     <item> <description> Goes through all containers that were referenced in tiles</description> </item>
    ///     <item>
    ///         <description>
    ///             Populates the items inside those objects. In case there's containers inside, adds them to a temporary list
    ///             to be processed later.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             Processes all containers that were reserved, and adds any other containers to this list. The list
    ///             grows and shrinks as it continues processing stuff.
    ///         </description>
    ///     </item>
    /// </list>
    /// <para> In essence, instead of iterating recursively, adds all items to this "to-do" list to be processed later.</para>
    /// <para> Suppose the following two situations. Here, the nomenclature is object(idx) </para>
    /// <list type="bullet">
    ///     <item> <description> container(X) in tile -> container(X-10) in container(X)  </description> </item>
    ///     <item> <description> container(X) in tile -> container(X+10) in container(X)  </description> </item>
    /// </list>
    /// <para>
    ///     In situation 1, since we're already past the container (X-10), it won't be populated. In situation 2,
    ///     it'll eventually be populated since it hasn't been reached. By adding containers to this temporary list,
    ///     and processing them later, we guarantee that we can reach all containers that were potentially in a previous
    ///     location. However, if we don't remove the containers in the second situation, they'll be populated again, and
    ///     why there's a check for location, then removal.
    /// </para>
    /// <para>
    ///     Example for situation 1:
    ///     In lvl 3 (block 2), obj 226 (Lizardman at X17 Y7) has a pouch (603). The pouch points to 599->598->932.
    ///     When going through this, the pouch is added to objects since it's "inside" the lizardman, but that sets
    ///     the refcount of the pouch to 1. This makes it accessible to this loop, and so the contents can be added.
    ///     If the pouch isn't removed from containersToProcessLater, it'll be processed later again.
    /// </para>
    /// <para>
    ///     Potential examples for situation 2: traps that store mobile objects. Traps are always StaticObjects and
    ///     MobileObjects are always at the start of the array. Not considering this would lead to unpopulated traps.
    /// </para>
    /// </summary>
    private void AddObjectsToContainers()
    {
        // Gathering IContainers that were in tiles, populating them, then getting any contents they had that also were containers
        var containersToProcessLater = new List<IContainer>();
        foreach (var gameObject in AllGameObjects)
        {
            if (gameObject.Invalid) continue;
            // Only considering containers that were placed in Tiles, which are "relevant". If this isn't checked, objects in unused
            // slots might be referenced and lead to bugs.
            if (gameObject.ReferenceCount < 1) continue;
            if (gameObject is IContainer container)
            {
                container.Contents.PopulateObjectList(AllGameObjects);
                containersToProcessLater.AddRange(container.Contents.OfType<IContainer>());
                // Removes itself from further processing if itself was added previously
                if (containersToProcessLater.Contains(container)) containersToProcessLater.Remove(container);
            }
        }

        // Guaranteeing every remaining container will eventually be accounted for
        while (containersToProcessLater.Count > 0)
        {
            var container = containersToProcessLater[0];
            containersToProcessLater.RemoveAt(0);
            container.Contents.PopulateObjectList(AllGameObjects);
            containersToProcessLater.AddRange(container.Contents.OfType<IContainer>());
        }
    }


    private byte[] _tileMapBuffer = new byte[TileMapLength];
    /// <summary>
    /// Controls the buffer to the tile map.
    /// </summary>
    /// <exception cref="ArgumentException">In case the length is inappropriate</exception>
    private byte[] TileMapBuffer
    {
        get => _tileMapBuffer;
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
    /// <summary>
    /// Controls the buffer that contains all the mobile objects.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown in case the set value has a length different from <see cref="MobileObjectInfoLength"/></exception>
    private byte[] MobileObjectInfoBuffer
    {
        get => _mobileObjectInfoBuffer;
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

    /// <summary>
    /// Controls the buffer that contains all the static objects.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown in case the set value has a length different from <see cref="StaticObjectInfoLength"/></exception>
    private byte[] StaticObjectInfoBuffer
    {
        get => _staticObjectInfoBuffer;
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

    /// <summary>
    /// Controls the buffer that contains all the slot pointers to mobile objects.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown in case the set value has a length different from <see cref="FreeListMobileObjectsLength"/></exception>
    private byte[] FreeListMobileObjectBuffer
    {
        get => _freeListMobileObjectBuffer;
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
    /// <summary>
    /// Controls the buffer that contains all the slot pointers to static objects.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown in case the set value has a length different from <see cref="FreeListStaticObjectsLength"/></exception>
    private byte[] FreeListStaticObjectBuffer
    {
        get => _freeListStaticObjectBuffer;
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
    /// <summary>
    /// Controls one of the, for the moment, unknown buffer.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown in case the set value has a length different from <see cref="UnknownLength"/></exception>
    private byte[] UnknownBuffer
    {
        get => _unknownBuffer;
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
    /// <summary>
    /// Controls another of the unknown buffers.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown in case the set value has a length different from <see cref="Unknown2Buffer"/></exception>
    private byte[] Unknown2Buffer
    {
        get => _unknown2Buffer;
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