using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using UWRandomizerEditor.Interfaces;

namespace UWRandomizerEditor.CMB;

/// <summary>
/// Represents a 2-item combination that produces another item.
/// </summary>
public class ItemCombination : IBufferObject
{
    /// <summary>
    /// First Item, Second Item, Product, totaling 3 items
    /// </summary>
    [JsonIgnore] public const int NumOfItemsInCombination = 3;
    /// <summary>
    /// Buffer size of this IBufferObject. For 3 items, each 1 short long, totals 6 bytes.
    /// </summary>
    [JsonIgnore] public const int FixedBufferSize = NumOfItemsInCombination * ItemDescriptor.Size;
    
    public ItemDescriptor FirstItem;
    public ItemDescriptor SecondItem;
    public ItemDescriptor Product;

    /// <summary>
    /// Stores the actual buffer.
    /// </summary>
    private byte[] _buffer = new byte[FixedBufferSize];
    
    /// <summary>
    /// Interfaces with the Buffer. When getting, updates the actual buffer with the buffers from the items. When setting,
    /// checks the buffer length and creates new descriptors for each pair of shorts.
    /// </summary>
    /// <exception cref="ItemCombinationException">In case the buffer provided is the wrong length.</exception>
    [JsonIgnore] public byte[] Buffer
    {
        get {
            ReconstructBuffer();
            return _buffer;
        }
        set
        {
            if (value.Length != FixedBufferSize)
            {
                throw new ItemCombinationException(
                    $"New buffer size of {value.Length} is incompatible with ItemCombination length of {FixedBufferSize}");
            }
            _buffer = value;
            CreateDescriptors();
        } 
    }

    /// <summary>
    /// <inheritdoc cref="IBufferObject.Buffer"/>
    /// </summary>
    /// <returns></returns>
    public bool ReconstructBuffer()
    {
        FirstItem.Buffer.CopyTo(_buffer, ItemDescriptor.Size * 0);
        SecondItem.Buffer.CopyTo(_buffer, ItemDescriptor.Size * 1);
        Product.Buffer.CopyTo(_buffer, ItemDescriptor.Size * 2);
        return true;
    }

    /// <summary>
    /// Instantiates new ItemDescriptors for the current buffer.
    /// </summary>
    [MemberNotNull(nameof(FirstItem))]
    [MemberNotNull(nameof(SecondItem))]
    [MemberNotNull(nameof(Product))]
    private void CreateDescriptors()
    {
        FirstItem  = new ItemDescriptor(_buffer[(ItemDescriptor.Size * 0)..(ItemDescriptor.Size * 1)]);
        SecondItem = new ItemDescriptor(_buffer[(ItemDescriptor.Size * 1)..(ItemDescriptor.Size * 2)]);
        Product    = new ItemDescriptor(_buffer[(ItemDescriptor.Size * 2)..(ItemDescriptor.Size * 3)]);
    }

    /// <summary>
    /// Checks if at least one of the items is being destroyed in the combination, and if the product isn't.
    /// </summary>
    /// <returns></returns>
    public virtual bool IsValidItemCombination()
    {
        // TODO: If the product is marked to be destroyed, will it affect anything?
        if ((FirstItem.IsDestroyed | SecondItem.IsDestroyed) & (!Product.IsDestroyed))
            return true;
        return false;
    }


    /// <summary>
    /// Creates a new ItemCombination object given a buffer.
    /// </summary>
    /// <param name="buffer"></param>
    public ItemCombination(byte[] buffer)
    {
        Buffer = buffer;
        CreateDescriptors();
    }

    /// <summary>
    /// Creates a new ItemCombination object given the first, second and product ItemDescriptors. Constructs a buffer
    /// to match the arguments. This is also used as the JsonConstructor for this object, given that storing buffers as
    /// json is not ideal. 
    /// </summary>
    /// <param name="firstItem"></param>
    /// <param name="secondItem"></param>
    /// <param name="product"></param>
    [JsonConstructor]
    public ItemCombination(ItemDescriptor firstItem, ItemDescriptor secondItem, ItemDescriptor product)
    {
        FirstItem = firstItem;
        SecondItem = secondItem;
        Product = product;
        ReconstructBuffer();
    }
    
    // TODO: check out how to check for Equality without having to specify the hash.
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ItemCombination) obj);
    }

    public bool Equals(ItemCombination? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return FirstItem.Equals(other.FirstItem) && SecondItem.Equals(other.SecondItem) && Product.Equals(other.Product);
    }
}
