using UWRandomizerEditor.Interfaces;
using UWRandomizerEditor.LEVdotARK.Blocks;

// TODO: What about empty entries? And the null entry (id = 0)?
// TODO: Add checks that prevent modification if id=0.
namespace UWRandomizerEditor.LEVdotARK.GameObjects;

public abstract class GameObject : IEquatable<GameObject>, IBufferObject
{
    // Constants related to GameObjects
    private const int SizeOfFields = 2;
    private const int NumberOfFields = 4;
    public const int FixedBufferLength = NumberOfFields * SizeOfFields;

    /// <summary>
    /// References the <see cref="TileMapMasterObjectListBlock.AllGameObjects"/> array
    /// </summary>
    public ushort IdxAtObjectArray;

    /// <summary>
    /// Number of times a GameObject or its derived classes were referenced in IContainers
    /// </summary>
    public uint ReferenceCount = 0;

    /// <summary>
    /// true if the object is inside an <see cref="IContainer"/>, false if on a <see cref="Tile"/> or if <see cref="ReferenceCount"/> is zero.
    /// </summary>
    public bool InContainer { get; set; }

    protected byte[] BasicInfoBuffer = new byte[FixedBufferLength];
    protected byte[] ExtraInfoBuffer = Array.Empty<byte>(); // Can be 19 bytes long if a MobileObject

    /// <summary>
    /// The Buffer represents the game object as it's stored in lev.ark
    /// </summary>
    public byte[] Buffer
    {
        get
        {
            var temp = new List<byte>();
            temp.AddRange(BasicInfoBuffer);
            temp.AddRange(ExtraInfoBuffer);
            return temp.ToArray();
        }
        set
        {
            value[..FixedBufferLength].CopyTo(BasicInfoBuffer, 0);
            value[FixedBufferLength..].CopyTo(ExtraInfoBuffer, 0);
        }
    }

    protected ushort ObjIdFlags
    {
        get => BitConverter.ToUInt16(BasicInfoBuffer, 0);
        set => BitConverter.GetBytes(value).CopyTo(BasicInfoBuffer, 0);
    }

    protected ushort Position
    {
        get => BitConverter.ToUInt16(BasicInfoBuffer, 2);
        set => BitConverter.GetBytes(value).CopyTo(BasicInfoBuffer, 2);
    }

    protected ushort QualityChain
    {
        get => BitConverter.ToUInt16(BasicInfoBuffer, 4);
        set => BitConverter.GetBytes(value).CopyTo(BasicInfoBuffer, 4);
    }

    protected ushort LinkSpecial
    {
        get => BitConverter.ToUInt16(BasicInfoBuffer, 6);
        set => BitConverter.GetBytes(value).CopyTo(BasicInfoBuffer, 6);
    }

    /// <summary>
    /// Naively checks if <see cref="IdxAtObjectArray"/> of 0 or 1 (which are reserved) or if it's a
    /// <see cref="MobileObject"/> located in the StaticObject array, or a <see cref="StaticObject"/>
    /// in the MobileObject array.
    /// </summary>
    public bool NaiveIsInvalid()
    {
        // Otherwise, will use these conditions will always yield an invalid object.
        if (IdxAtObjectArray == 0 | IdxAtObjectArray == 1)
            return true;
        if (this is MobileObject & IdxAtObjectArray >= TileMapMasterObjectListBlock.MobileObjectNum)
            return true;
        if (this is StaticObject & IdxAtObjectArray < TileMapMasterObjectListBlock.MobileObjectNum)
            return true;
        return false;
    }

    /// <summary>
    /// This property is meant to symbolize if a GameObject shouldn't be considered available for anything, or
    /// if it shouldn't be moved. 
    /// </summary>
    public bool Invalid { get; set; }

    protected GameObject()
    { }

    protected GameObject(byte[] buffer, ushort idxAtObjArray)
    {
        IdxAtObjectArray = idxAtObjArray;
        Buffer = buffer;
    }

    protected GameObject(ushort objIdFlags, ushort position, ushort qualityChain,
        ushort linkSpecial, ushort idxAtObjectArray)
    {
        ObjIdFlags = objIdFlags;
        Position = position;
        QualityChain = qualityChain;
        LinkSpecial = linkSpecial;
        IdxAtObjectArray = idxAtObjectArray;
    }

    public virtual bool ReconstructBuffer()
    {
        return true;
    }

    // First short
    public int ItemID
    {
        get => Utils.GetBits(ObjIdFlags, 0b111111111, 0);
        set => ObjIdFlags = (ushort) Utils.SetBits(ObjIdFlags, value, 0b111111111, 0);
    }

    public int Flags
    {
        // todo: error in uw-formats.txt? Check hank's. -- He uses a mask with 3 bits, so I changed here.
        get => Utils.GetBits(ObjIdFlags, 0b111, 9);
        set => ObjIdFlags = (ushort) Utils.SetBits(ObjIdFlags, value, 0b111, 9);
    }

    public int EnchantFlag
    {
        get => Utils.GetBits(Flags, 0b1, 0);
        set => Flags = (short) Utils.SetBits(Flags, value, 0b1, 0);
    }

    public int DoorDir
    {
        get => Utils.GetBits(Flags, 0b1, 1);
        set => Flags = (short) Utils.SetBits(Flags, value, 0b1, 1);
    }

    public int Invis
    {
        get => Utils.GetBits(Flags, 0b1, 2);
        set => Flags = (short) Utils.SetBits(Flags, value, 0b1, 2);
    }

    public int IsQuant
    {
        get => Utils.GetBits(Flags, 0b1, 3);
        set => Flags = (short) Utils.SetBits(Flags, value, 0b1, 3);
    }

    // Second short
    public byte Zpos
    {
        get => (byte) Utils.GetBits(Position, 0b1111111, 0);
        set => Position = (ushort) Utils.SetBits(Position, value, 0b1111111, 0);
    }

    public byte Heading
    {
        get => (byte) Utils.GetBits(Position, 0b111, 7);
        set => Position = (ushort) Utils.SetBits(Position, value, 0b111, 7);
    }

    public byte Ypos
    {
        get => (byte) Utils.GetBits(Position, 0b111, 10);
        set => Position = (ushort) Utils.SetBits(Position, value, 0b111, 10);
    }

    public byte Xpos
    {
        get => (byte) Utils.GetBits(Position, 0b111, 13);
        set => Position = (ushort) Utils.SetBits(Position, value, 0b111, 13);
    }

    // Second short
    public byte Quality
    {
        get => (byte) Utils.GetBits(QualityChain, 0b111111, 0);
        set => QualityChain = (ushort) Utils.SetBits(QualityChain, value, 0b111111, 0);
    }

    public ushort next
    {
        get => (ushort) Utils.GetBits(QualityChain, 0b1111111111, 6);
        set => QualityChain = (ushort) Utils.SetBits(QualityChain, value, 0b1111111111, 6);
    }


    public bool IsEndOfList => next == 0;

    // Fourth short
    public byte OwnerOrSpecial
    {
        get => (byte) Utils.GetBits(LinkSpecial, 0b111111, 0);
        set => LinkSpecial = (ushort) Utils.SetBits(LinkSpecial, value, 0b111111, 0);
    }

    public ushort QuantityOrSpecialLinkOrSpecialProperty
    {
        get => (ushort) Utils.GetBits(LinkSpecial, 0b11_1111_1111, 6);
        set => LinkSpecial = (ushort) Utils.SetBits(LinkSpecial, value, 0b11_1111_1111, 6);
    }

    // todo: which one? This or OwnerOrSpecial == 0?
    public bool HasOwner => ItemOwnerStrIdx == 0;

    public int ItemOwnerStrIdx
    {
        get => OwnerOrSpecial - 1 + 370;
        set => OwnerOrSpecial = (byte) (value + 1 - 370);
    }

    // TODO: Revise all these "Is..." functions. Would they be needed by the GameObjectFactory?
    // private static bool IsTexturedObject(byte[] buffer)
    // {
    //     var firstByte = buffer[0];
    //     var start = 0x160; // TODO: I haven't checked these indices thoroughly!
    //     var end = 0x17f; // TODO: I haven't checked these indices thoroughly!
    //     if ((firstByte > start) | (firstByte < end))
    //         return true;
    //     return false;
    // }
    //
    // private static bool IsQuantityObject(byte[] buffer)
    // {
    //     if (IsTexturedObject(buffer))
    //     {
    //         return false;
    //     }
    //
    //     var first2Bytes = BitConverter.ToInt16(buffer, 0);
    //     var quantityBit = ((first2Bytes >> 15) & 1) == 1;
    //     return quantityBit;
    // }
    //
    // public static bool IsSpecialPropertyObject(byte[] buffer)
    // {
    //     if (IsTexturedObject(buffer))
    //     {
    //         return false;
    //     }
    //
    //     short lastShort = BitConverter.ToInt16(buffer, 6);
    //     int quantity = (lastShort >> 6) & 0b1111111111;
    //     if ((quantity > 512) & IsQuantityObject(buffer))
    //     {
    //         return true;
    //     }
    //     else if ((quantity < 512) & IsQuantityObject(buffer))
    //     {
    //         return false;
    //     }
    //     else
    //     {
    //         throw new Exception("Has to be a quantity object.");
    //     }
    // }
    //
    // public static bool IsSpecialLinkObject(byte[] buffer)
    // {
    //     if (IsTexturedObject(buffer))
    //     {
    //         return false;
    //     }
    //
    //     if (!IsQuantityObject(buffer))
    //     {
    //         return true;
    //     }
    //     else
    //     {
    //         return false;
    //     }
    // }

    /// <summary>
    /// Checks if two GameObject instances are the same by comparing their buffers
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(GameObject? other)
    {
        if (other == null)
        {
            return false;
        }

        if (Buffer.Length != other.Buffer.Length)
        {
            return false;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        for (int i = 0; i < Buffer.Length; i++)
        {
            if (Buffer[i] != other.Buffer[i])
            {
                return false;
            }
        }

        return true;
    }
}