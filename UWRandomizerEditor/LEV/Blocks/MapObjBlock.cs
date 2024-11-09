using System.Data;
using System.Diagnostics;
using UWRandomizerEditor.LEV.GameObjects;
using UWRandomizerEditor.LEV.GameObjects.Specifics;

// ReSharper disable AccessToStaticMemberViaDerivedType

namespace UWRandomizerEditor.LEV.Blocks;

public partial class MapObjBlock : Block
{
    /// <summary>
    /// Contains all tiles in a continuous array
    /// </summary>
    public Tile[] Tiles = new Tile[LengthOfTileMap / EntrySizeOfTileMap];

    /// <summary>
    /// Reshapes <see cref="Tiles"/> into a <see cref="TileWidth"/> by <see cref="TileHeight"/> array on the fly, for convenience.
    /// </summary>
    public Tile[,] Tiles2D => Utils.ReshapeArray(Tiles, TileWidth, TileHeight);

    /// <summary>
    /// This conveniently joins the Mobile Object and Static Object arrays into one, from 0 to <see cref="NumOfMobileObjects"/>+<see cref="NumOfStaticObjects"/>
    /// </summary>
    private GameObject[] _allGameObjects = new GameObject[NumOfMobileObjects + NumOfStaticObjects];
    public GameObject[] AllGameObjects
    {
        get => _allGameObjects;
        set => _allGameObjects = value;
    }

    /// <summary>
    /// Contains all the <see cref="MobileObject"/>s in the level, including objects 0 and 1, which
    /// should never be accessed. Should only be used for convenience! Nver replace a value in this array!
    /// Modifications can be used though (preserves reference).
    /// </summary>
    public MobileObject[] MobileObjects
    {
        get
        {
            var temp = _allGameObjects[0..NumOfMobileObjects];
            return temp.Select(x => (MobileObject) x).ToArray();
        }
    }

    /// <summary>
    /// Contains all the <see cref="StaticObject"/>s in the level. Should only be used for convenience! Never
    /// replace a value in this array! Modifications can be used though (preserves reference)
    /// </summary>
    public StaticObject[] StaticObjects
    {
        get
        {
            var temp = _allGameObjects[NumOfMobileObjects..(NumOfMobileObjects+NumOfStaticObjects)];
            return temp.Select(x => (StaticObject)x).ToArray();
        }
    }

    private ushort[] _indicesOfFreeMobileObjects = new ushort[NumOfFreeMobileObjects];
    // private IdxOfFreeObj[] _indicesOfFreeMobileObjects = new IdxOfFreeObj[NumOfFreeMobileObjects];

    public ushort[] IndicesOfFreeMobileObjects
    {
        get => _indicesOfFreeMobileObjects;
        private set
        {
            if (value.Length != NumOfFreeMobileObjects)
                throw new ArgumentException($"Length of IndicesIntoFreeSlots should be {NumOfFreeMobileObjects}");
            _indicesOfFreeMobileObjects = value;
        }
    }

    private ushort[] _indicesOfFreeStaticObjects = new ushort[NumberOfFreeStaticObjectSlots];

    /// <summary>
    /// Contains a list of objects that point to the slot in StaticObjects that is "free", i.e. unused.
    /// </summary>
    /// <exception cref="ArgumentException">If the array length is different from <see cref="NumberOfFreeStaticObjectSlots"/></exception>
    public ushort[] IndicesOfFreeStaticObjects
    {
        get => _indicesOfFreeStaticObjects;
        private set
        {
            if (value.Length != NumberOfFreeStaticObjectSlots)
                throw new ArgumentException($"Length of FreeStaticObjectSlots should be {NumberOfFreeStaticObjectSlots}");
            _indicesOfFreeStaticObjects = value;
        }
    }

    /// <summary>
    /// Joins together <see cref="IndicesOfFreeMobileObjects"/> and <see cref="IndicesOfFreeStaticObjects"/> into one array.
    /// </summary>
    public ushort[] IndicesOfFreeObjects
    {
        get
        {
            var temp = new List<ushort>();
            temp.AddRange(IndicesOfFreeMobileObjects);
            temp.AddRange(IndicesOfFreeStaticObjects);
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
    public ushort IdxLookupOfFreeMobileObject
    {
        get => BitConverter.ToUInt16(_buffer, OffsetOfFirstFreeSlotInMobileSlots);
        set => BitConverter.GetBytes(value).CopyTo(_buffer, OffsetOfFirstFreeSlotInMobileSlots);
    }

    /// <summary>
    /// This value represents the index of the first slot that points to a free static entry in the <see cref="AllGameObjects"/> array 
    /// </summary>
    public ushort IdxLookupOfFreeStaticObject
    {
        get => BitConverter.ToUInt16(_buffer, OffsetOfFirstFreeSlotInStaticSlots);
        set => BitConverter.GetBytes(value).CopyTo(_buffer, OffsetOfFirstFreeSlotInStaticSlots);
    }

    /// <summary>
    /// The index in the <see cref="AllGameObjects"/> array (and <see cref="MobileObjects"/> array) that points to an unused <see cref="MobileObject"/> and can be freely modified.
    /// </summary>
    public ushort IdxOfFreeMobileObject => IndicesOfFreeObjects[IdxLookupOfFreeMobileObject];

    /// <summary>
    /// The index in the <see cref="AllGameObjects"/> array that points to an unused <see cref="StaticObject"/> and can be freely modified.
    /// </summary>
    public ushort IdxOfFreeStaticObject => IndicesOfFreeStaticObjects[IdxLookupOfFreeStaticObject];

    /// <summary>
    /// The first <see cref="MobileObject"/> that can be freely modified.
    /// </summary>
    public MobileObject CurrentFreeMobileObject => MobileObjects[IdxOfFreeMobileObject];

    /// <summary>
    /// The first <see cref="StaticObject"/> that can be freely modified.
    /// </summary>
    public StaticObject CurrentFreeStaticObject => (StaticObject) AllGameObjects[IdxOfFreeStaticObject];

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

        if (AllGameObjects.Count(x => x is StaticObject) != NumOfStaticObjects)
        {
            throw new BlockOperationException(
                $"Somehow there's more static objects ({StaticObjects.Length}) than the max ({NumOfStaticObjects})");
        }

        if (AllGameObjects.Count(x => x is MobileObject) != NumOfMobileObjects)
        {
            throw new BlockOperationException(
                $"Somehow there's more mobile objects ({MobileObjects.Length}) than the max ({NumOfMobileObjects})");
        }

        if (Tiles.Length != (TileWidth * TileHeight))
        {
            throw new BlockOperationException(
                $"Somehow there's more tiles ({Tiles.Length}) than the max ({TileWidth * TileHeight}");
        }

        if (ListOfActiveMobs.Length != LengthOfListOfActiveMobileObjects)
        {
            throw new BlockOperationException(
                $"Somehow ListOfActiveMobs length ({ListOfActiveMobs.Length}) is different from {LengthOfListOfActiveMobileObjects}");
        }

        ReconstructSubBuffers();

        TileMapBuffer.CopyTo(_buffer, OffsetOfTileMap);
        MobileObjectInfoBuffer.CopyTo(_buffer, OffsetOfMobileObjectInfo);
        StaticObjectInfoBuffer.CopyTo(_buffer, OffsetOfStaticObjectInfo);
        FreeListMobileObjectBuffer.CopyTo(_buffer, OffsetOfFreeListMobileObjects);
        FreeListStaticObjectBuffer.CopyTo(_buffer, OffsetOfFreeListStaticObjects);
        ListOfActiveMobs.CopyTo(_buffer, OffsetOfListOfActiveMobileObjects);
        BitConverter.GetBytes(IdxLookupOfActiveMobs).CopyTo(_buffer, OffsetOfIdxLookupOfActiveMobs);
        BitConverter.GetBytes(IdxLookupOfFreeMobileObject).CopyTo(_buffer, OffsetOfFirstFreeSlotInMobileSlots);
        BitConverter.GetBytes(IdxLookupOfFreeStaticObject).CopyTo(_buffer, OffsetOfFirstFreeSlotInStaticSlots);
        BitConverter.GetBytes(EndOfBlockConfirmationValue).CopyTo(_buffer, OffsetOfEndOfBlockConfirmation);
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
            obj.Buffer.CopyTo(MobileObjectInfoBuffer, obj.IdxAtObjectArray * MobileObject.FixedMobileBufferLength);
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
                (obj.IdxAtObjectArray - NumOfMobileObjects) * StaticObject.FixedBufferLength);
        }
    }

    /// <summary>
    /// Reconstructs the <see cref="FreeListStaticObjectBuffer"/>.
    /// </summary>
    private void ReconstructFreeListStaticObjectBuffer()
    {
        var tempAcc = new List<byte>();
        foreach (var obj in IndicesOfFreeStaticObjects)
        {
            tempAcc.AddRange(BitConverter.GetBytes(obj));
            // obj.Buffer.CopyTo(FreeListStaticObjectBuffer, obj.EntryNum * IdxOfFreeObj.FixedSize);
        }

        FreeListStaticObjectBuffer = tempAcc.ToArray();
    }

    /// <summary>
    /// Reconstructs the <see cref="FreeListMobileObjectBuffer"/>.
    /// </summary>
    private void ReconstructFreeListMobileObjectBuffer()
    {
        var tempAcc = new List<byte>();
        foreach (var obj in IndicesOfFreeMobileObjects)
        {
            tempAcc.AddRange(BitConverter.GetBytes(obj));
            // obj.Buffer.CopyTo(FreeListStaticObjectBuffer, obj.EntryNum * IdxOfFreeObj.FixedSize);
        }

        FreeListMobileObjectBuffer = tempAcc.ToArray();
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
        for (short i = 0; i < NumOfStaticObjects; i++)
        {
            var currBuffer =
                StaticObjectInfoBuffer[
                    (i * StaticObject.FixedBufferLength)..((i + 1) * StaticObject.FixedBufferLength)];
            var currObj =
                (StaticObject) GameObjectFactory.CreateFromBuffer(currBuffer, (ushort) (i + NumOfMobileObjects));

            if ((currObj.IdxAtObjectArray < NumOfMobileObjects))
            {
                throw new BlockOperationException(
                    "Attempted to add a static object to the region of mobile objects. Should not happen!");
            }

            AllGameObjects[i+NumOfMobileObjects] = currObj;
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
        for (ushort i = 0; i < NumOfMobileObjects; i++)
        {
            byte[] buffer =
                MobileObjectInfoBuffer[
                    (i * MobileObject.FixedMobileBufferLength)..((i + 1) * MobileObject.FixedMobileBufferLength)];
            var obj = (MobileObject) GameObjectFactory.CreateFromBuffer(buffer, i);
            if (obj.IdxAtObjectArray >= NumOfMobileObjects)
            {
                throw new BlockOperationException(
                    "Attempted to add a static object to the region of mobile objects. Should not happen!");
            }

            AllGameObjects[i] = obj;
        }
    }
    
    /// <summary>
    /// Goes through the buffer and creates the FreeSlotIndexes at the appropriate locations.
    /// </summary>
    private void PopulateFreeListMobileObjectArrFromBuffer()
    {
        for (int i = 0; i < NumOfFreeMobileObjects; i++)
        {
            var val =BitConverter.ToUInt16(
                FreeListMobileObjectBuffer[(i * IdxOfFreeObj.FixedSize)..((i + 1) * IdxOfFreeObj.FixedSize)]);
            IndicesOfFreeMobileObjects[i] = val;
        }
    }
    

    /// <summary>
    /// Goes through the buffer and creates the FreeSlotIndexes at the appropriate locations.
    /// </summary>
    private void PopulateFreeListStaticObjectArrFromBuffer()
    {
        for (int i = 0; i < NumberOfFreeStaticObjectSlots; i++)
        {
            var val = BitConverter.ToUInt16(
                FreeListStaticObjectBuffer[(i * IdxOfFreeObj.FixedSize)..((i + 1) * IdxOfFreeObj.FixedSize)]);
            // var obj = new IdxOfFreeObj(buffer, i);
            IndicesOfFreeStaticObjects[i] = val;
        }
    }

    /// <summary>
    /// Instantiates a new TileMapMasterObjectListBlock based upon the buffer and the level number.
    /// </summary>
    /// <param name="buffer">Byte buffer containing the data. Must be <see cref="FixedBlockLength"/> bytes long.</param>
    /// <param name="levelNumber">Level number, for convenience. Isn't required for anything.</param>
    /// <exception cref="ArgumentException">Thrown in case the length of buffer is different from <see cref="FixedBlockLength"/>.</exception>
    public MapObjBlock(byte[] buffer, int levelNumber)
    {
        if (buffer.Length != FixedBlockLength)
        {
            throw new ArgumentException(
                $"Length of buffer ({buffer.Length}) is incompatible with expected TextureMappingBlock length ({FixedBlockLength})");
        }

        _buffer = new byte[FixedBlockLength];
        buffer.CopyTo(_buffer, 0);
        LevelNumber = levelNumber;

        TileMapBuffer = buffer[OffsetOfTileMap..(OffsetOfTileMap + LengthOfTileMap)];
        MobileObjectInfoBuffer = buffer[OffsetOfMobileObjectInfo..(OffsetOfMobileObjectInfo + LengthOfMobileObjectInfo)];
        StaticObjectInfoBuffer = buffer[OffsetOfStaticObjectInfo..(OffsetOfStaticObjectInfo + LengthOfStaticObjectInfo)];
        FreeListMobileObjectBuffer =
            buffer[OffsetOfFreeListMobileObjects..(OffsetOfFreeListMobileObjects + LengthOfFreeListMobileObjects)];
        FreeListStaticObjectBuffer =
            buffer[OffsetOfFreeListStaticObjects..(OffsetOfFreeListStaticObjects + LengthOfFreeListStaticObjects)];
        ListOfActiveMobs = buffer[OffsetOfListOfActiveMobileObjects..(OffsetOfListOfActiveMobileObjects + LengthOfListOfActiveMobileObjects)];
        IdxLookupOfActiveMobs = BitConverter.ToUInt16(buffer, OffsetOfIdxLookupOfActiveMobs);

        PopulateFreeListMobileObjectArrFromBuffer();
        PopulateFreeListStaticObjectArrFromBuffer();
        PopulateMobileObjectsFromBuffer();
        PopulateStaticObjectsFromBuffer();
        PopulateTiles();
        AddObjectsToTiles();
        AddObjectsToContainers();
        AttributeActivityStatusToMobileObjects();
    }

    /// <summary>
    /// Creates tiles for the buffer
    /// </summary>
    private void PopulateTiles()
    {
        for (uint i = 0; i < LengthOfTileMap / EntrySizeOfTileMap; i++)
        {
            var offset = i * EntrySizeOfTileMap;
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


    private byte[] _tileMapBuffer = new byte[LengthOfTileMap];

    /// <summary>
    /// Controls the buffer to the tile map.
    /// </summary>
    /// <exception cref="ArgumentException">In case the length is inappropriate</exception>
    private byte[] TileMapBuffer
    {
        get => _tileMapBuffer;
        set
        {
            if (value.Length != LengthOfTileMap)
            {
                throw new ArgumentException($"Invalid length of TileMapBuffer");
            }

            value.CopyTo(_tileMapBuffer, 0);
        }
    }

    private byte[] _mobileObjectInfoBuffer = new byte[LengthOfMobileObjectInfo];

    /// <summary>
    /// Controls the buffer that contains all the mobile objects.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown in case the set value has a length different from <see cref="LengthOfMobileObjectInfo"/></exception>
    private byte[] MobileObjectInfoBuffer
    {
        get => _mobileObjectInfoBuffer;
        set
        {
            if (value.Length != LengthOfMobileObjectInfo)
            {
                throw new ArgumentException($"Invalid length of MobileObjectInfoBuffer");
            }

            value.CopyTo(_mobileObjectInfoBuffer, 0);
        }
    }

    private byte[] _staticObjectInfoBuffer = new byte[LengthOfStaticObjectInfo];

    /// <summary>
    /// Controls the buffer that contains all the static objects.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown in case the set value has a length different from <see cref="LengthOfStaticObjectInfo"/></exception>
    private byte[] StaticObjectInfoBuffer
    {
        get => _staticObjectInfoBuffer;
        set
        {
            if (value.Length != LengthOfStaticObjectInfo)
            {
                throw new ArgumentException($"Invalid length of StaticObjectInfoBuffer");
            }

            value.CopyTo(_staticObjectInfoBuffer, 0);
        }
    }

    private byte[] _freeListMobileObjectBuffer = new byte[LengthOfFreeListMobileObjects];

    /// <summary>
    /// Controls the buffer that contains all the slot pointers to mobile objects.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown in case the set value has a length different from <see cref="LengthOfFreeListMobileObjects"/></exception>
    private byte[] FreeListMobileObjectBuffer
    {
        get => _freeListMobileObjectBuffer;
        set
        {
            if (value.Length != LengthOfFreeListMobileObjects)
            {
                throw new ArgumentException($"Invalid length of FreeListMobileObjectBuffer");
            }

            value.CopyTo(_freeListMobileObjectBuffer, 0);
        }
    }

    private byte[] _freeListStaticObjectBuffer = new byte[LengthOfFreeListStaticObjects];

    /// <summary>
    /// Controls the buffer that contains all the slot pointers to static objects.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown in case the set value has a length different from <see cref="LengthOfFreeListStaticObjects"/></exception>
    private byte[] FreeListStaticObjectBuffer
    {
        get => _freeListStaticObjectBuffer;
        set
        {
            if (value.Length != LengthOfFreeListStaticObjects)
            {
                throw new ArgumentException($"Invalid length of FreeListStaticObjectBuffer");
            }

            value.CopyTo(_freeListStaticObjectBuffer, 0);
        }
    }

    private byte[] _listOfActiveMobs = new byte[LengthOfListOfActiveMobileObjects];

    /// <summary>
    /// Controls the list of active mob indices
    /// </summary>
    /// <exception cref="ArgumentException">Thrown in case the set value has a length different from <see cref="LengthOfListOfActiveMobileObjects"/></exception>
    public byte[] ListOfActiveMobs
    {
        get => _listOfActiveMobs;
        set
        {
            if (value.Length != LengthOfListOfActiveMobileObjects)
            {
                throw new ArgumentException($"Invalid length");
            }

            value.CopyTo(_listOfActiveMobs, 0);
        }
    }

    /// <summary>
    /// This is the index into the list of active mobs. Doubles as a count of active mobs.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown in case the set value has a length different from <see cref="IdxLookupOfActiveMobs"/></exception>
    public ushort IdxLookupOfActiveMobs
    {
        get => BitConverter.ToUInt16(_buffer, OffsetOfIdxLookupOfActiveMobs);
        set => BitConverter.GetBytes(value).CopyTo(_buffer, OffsetOfIdxLookupOfActiveMobs);
    }
    
    /// <summary>
    /// Be careful when adding containers! The contents will be removed and the items linked might go missing!
    /// </summary>
    /// <param name="position"></param>
    /// <param name="obj"></param>
    /// <exception cref="BlockOperationException"></exception>
    public GameObject AddNewGameObjectToTile(Point p, GameObject obj, bool stayInactive = false)
    {
        if (AllGameObjects[obj.IdxAtObjectArray].Equals(obj))
        {
            throw new BlockOperationException(
                "Can't add an object that is already present in the level! Try cloning it.");
        }
        ushort idxLookup = obj is MobileObject ? IdxLookupOfFreeMobileObject : IdxLookupOfFreeStaticObject;
        if (idxLookup <= 0)
        {
            throw new BlockOperationException("Can't add another object, no free slots remaining!");
        }

        ushort idxOfNewObject = obj is MobileObject
            ? IndicesOfFreeMobileObjects[idxLookup]
            : IndicesOfFreeStaticObjects[idxLookup];

        var tile = Tiles2D[p.Row, p.Column];
        obj.IdxAtObjectArray = idxOfNewObject;
        tile.ObjectChain.Add(obj);
        AllGameObjects[idxOfNewObject] = obj;
        
        if (obj is MobileObject mobj)
        {
            IdxLookupOfFreeMobileObject--;
            mobj.XHome = p.X;
            mobj.YHome = p.Y;
            
            // Activating mobile object
            // Guarding against that activating some unknown objects on the upper range of mobile object id. This is from UWE.
            // TODO: test if these really are problematic.
            if (!((new [] { 125, 126, 127 }.Contains(mobj.ItemID)) | (mobj.IdxAtObjectArray == 0) | (mobj.IdxAtObjectArray == 1)))
            {
                if (!stayInactive)
                {
                    mobj.IsActive = true;
                    mobj.IdxAtActiveMobj = IdxLookupOfActiveMobs;
                    ListOfActiveMobs[IdxLookupOfActiveMobs] = (byte) (mobj.IdxAtObjectArray & 0xFF);
                    IdxLookupOfActiveMobs++;
                }
            }
        }
        else
        {
            IdxLookupOfFreeStaticObject--;
        }
        tile.MoveObjectsToSameZLevel();
        if (obj is IContainer cont) // In case the created object supposedly has "contents". IContainer because
        {                           // the object can be mobile or static
            cont.Contents.Clear();
        }

        return obj;
    }

    public GameObject AddNewGameObjectToExistingContainer(GameObject container, GameObject obj)
    {
        // Let's assert the game object exists
        Debug.Assert(container.Equals(AllGameObjects[container.IdxAtObjectArray]));
        // Let's assert the object implements IContainer
        if (!(container is IContainer))
        {
            throw new BlockOperationException("Can't add an item to a non-container in this operation");
        }
        
        // Can be a monster with inventory or a bag, etc.
        ushort idxLookup = obj is MobileObject ? IdxLookupOfFreeMobileObject : IdxLookupOfFreeStaticObject;
        if (idxLookup <=0)
        {
            throw new BlockOperationException("Can't add another object, no free slots remaining!");
        }
        ushort idxOfNewObject = obj is MobileObject ? IndicesOfFreeMobileObjects[idxLookup] : IndicesOfFreeStaticObjects[idxLookup];
        AllGameObjects[idxOfNewObject] = obj;
        obj.IdxAtObjectArray = idxOfNewObject;
        if (obj is MobileObject)
        {
            // TODO: Should I activate the mobj, for funsies?
            IdxLookupOfFreeMobileObject--; 
            // UWE has some checks about objects not being able to be added to the container, like npcs.
            // TODO: Do I really need those checks?
        }
        else 
        {
            IdxLookupOfFreeStaticObject--;
        }
        ((IContainer) container).Contents.Add(obj);
        return obj;
    }
    
    public List<GameObject> RemoveGameObjectFromTile(Point p, GameObject obj)
    {
        var tile = Tiles2D[p.Row, p.Column];
        // List<GameObject>  
        // Iterate through every possible object, down to base, and add them to a list of objects to remove
        // from array, but only remove the requested object from the tile. Return a list of objects that were
        // removed.
        void RecursiveGetContentsOfContainer(IContainer container_, List<GameObject> toRemove)
        {
            foreach (var content in container_.Contents)
            {
                toRemove.Add(content);
                if (content is IContainer cont)
                {
                    RecursiveGetContentsOfContainer(cont, toRemove);
                }
            }
        }

        var objectsToRemoveNotOnTheTile = new List<GameObject>();
        if (obj is IContainer container)
        {
            RecursiveGetContentsOfContainer(container, objectsToRemoveNotOnTheTile);
        }
        
        // Take care of object in the tile
        tile.ObjectChain.Remove(obj);

        // Then clean up the indices.
        var allRemovedObjects = new List<GameObject> { obj };
        allRemovedObjects.AddRange(objectsToRemoveNotOnTheTile);
        
        // TODO: Lookup the order. First increment then set, or set then increment?
        foreach (var objToRemove in allRemovedObjects)
        {
            // TODO: is it better if I replace the object with one with a buffer equal to zeroes?
            if (objToRemove is MobileObject mobj)
            {
                // Replace with zeroes
                MobileObjects[mobj.IdxAtObjectArray] = MobileObject.ZeroedOutMobileObject(mobj.IdxAtObjectArray);
                // Activity
                mobj.IsActive = false;
                mobj.IdxAtActiveMobj = 0;
                IdxLookupOfActiveMobs--;
                // Indices
                IdxLookupOfFreeMobileObject++;
                IndicesOfFreeMobileObjects[IdxLookupOfFreeMobileObject] = mobj.IdxAtObjectArray;
            }
            else
            {
                // Replace with zeroes
                AllGameObjects[objToRemove.IdxAtObjectArray] = StaticObject.ZeroedOutStaticObject(objToRemove.IdxAtObjectArray);
                // Clean up indices
                IdxLookupOfFreeStaticObject++;
                IndicesOfFreeStaticObjects[IdxLookupOfFreeStaticObject] = objToRemove.IdxAtObjectArray;
            }
        }
        foreach (var specialObj in AllGameObjects)
        {
            switch (specialObj)
            {
                case Trigger trig:
                    trig.SpecialIdx = 0;
                    break;
                case Trap trap:
                    trap.SpecialIdx = 0;
                    break;
            }
        }
        return allRemovedObjects;
    }

    private void AttributeActivityStatusToMobileObjects()
    {
        foreach (var mobj in MobileObjects)
        {
            // Goes through the number of active mobs and sets their activity to true if they're in 
            for (int i = 0; i < IdxLookupOfActiveMobs; i++)
            {
                if (mobj.IdxAtObjectArray == ListOfActiveMobs[i])
                {
                    mobj.IsActive = true;
                    break;
                }
            }
        }
    }

}