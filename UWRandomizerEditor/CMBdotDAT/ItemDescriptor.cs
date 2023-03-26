using System.Text.Json.Serialization;
using UWRandomizerEditor.Interfaces;

namespace UWRandomizerEditor.CMBdotDAT;

/// <summary>
/// An BufferAsUShort is a short where the first bit is whether it's destroyed or not and the remaining bits are the ItemID
/// </summary>
public class ItemDescriptor: IBufferObject, IEquatable<ItemDescriptor>
{
    /// <summary>
    /// Specifies the size in bytes of this type of entry, which is 2 bytes (ushort)
    /// </summary>
    [JsonIgnore] public const int Size = 2;
    
    /// <summary>
    /// Contains the internal buffer representation
    /// </summary>
    private byte[] _buffer = new byte[Size];
    
    /// <summary>
    /// <inheritdoc cref="IBufferObject.Buffer"/>
    /// </summary>
    /// <exception cref="ItemCombinationException">Thrown if buffer length is incorrect</exception>
    [JsonIgnore] 
    public byte[] Buffer {
        get
        {
            ReconstructBuffer();
            return _buffer;
        }
        set
        {
            if (value.Length != Size)
            {
                throw new ItemCombinationException(
                    $"Can't create an ItemDescriptor with buffer size {value.Length}. Expected {Size}");
            }

            _buffer = value;
        } 
    }

    /// <summary>
    /// <inheritdoc cref="IBufferObject.ReconstructBuffer"/>
    /// </summary>
    /// In this case, won't perform anything because the getters and setters of the two properties already keep them updated.
    /// <returns></returns>
    public bool ReconstructBuffer() => true; // The getters and setters for ItemID and IsDestroyed should keep the buffer updated.

    /// <summary>
    /// Converts the byte array into a number for convenience.
    /// </summary>
    [JsonIgnore]
    private ushort BufferAsUShort
    {
        get => BitConverter.ToUInt16(_buffer);
        set => _buffer = BitConverter.GetBytes(value);
    }

    /// <summary>
    /// ItemID, same as GameObject ItemID. Unique for each item, so for different types of debris, you have different ItemIDs!
    /// </summary>
    public ushort ItemID
    {
        get => (ushort) Utils.GetBits(BufferAsUShort, 0x1FF, 0);
        set => BufferAsUShort = (ushort) Utils.SetBits(BufferAsUShort, value, 0x1FF, 0);
    }

    /// <summary>
    /// Specifies if the object should be destroyed upon combination. Either one, the other, or both items in a combination
    /// should be destroyed. If neither are, process won't finish.
    /// </summary>
    public bool IsDestroyed
    {
        get => BufferAsUShort >> 15 == 1;
        set => BufferAsUShort = (ushort) Utils.SetBits(BufferAsUShort, value ? 1 : 0, 0b1, 15);
    }

    /// <summary>
    /// Creates a new ItemDescriptor instance based upon itemID and isDestroyed flag
    /// </summary>
    /// <param name="itemID"></param>
    /// <param name="isDestroyed"></param>
    [JsonConstructor]
    public ItemDescriptor(ushort itemID, bool isDestroyed)
    {
        ItemID = itemID;
        IsDestroyed = isDestroyed;
    }

    /// <summary>
    /// Creates a new ItemDescriptor instance based on a buffer
    /// </summary>
    /// <param name="buffer"></param>
    public ItemDescriptor(byte[] buffer)
    {
        Buffer = buffer;
    }

    /// <summary>
    /// Creates a default ItemDescriptor.
    /// </summary>
    public ItemDescriptor() { }


    public bool Equals(ItemDescriptor? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return _buffer.SequenceEqual(other._buffer);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ItemDescriptor) obj);
    }
}
