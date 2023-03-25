using System.Text.Json.Serialization;
using UWRandomizerEditor.Interfaces;

namespace UWRandomizerEditor.CMBdotDAT;

public class ItemCombination : IBufferObject
{
    [JsonIgnore] public const int NumOfItemsInCombination = 3;
    [JsonIgnore] public const int Size = NumOfItemsInCombination * ItemDescriptor.size;
    [JsonIgnore] public byte[] Buffer { get; set; } = new byte[NumOfItemsInCombination * ItemDescriptor.size];

    public bool ReconstructBuffer()
    {
        FirstItem.buffer.CopyTo(Buffer, ItemDescriptor.size * 0);
        SecondItem.buffer.CopyTo(Buffer, ItemDescriptor.size * 1);
        Product.buffer.CopyTo(Buffer, ItemDescriptor.size * 2);
        return true;
    }

    public ItemDescriptor FirstItem;
    public ItemDescriptor SecondItem;
    public ItemDescriptor Product;

    public ItemCombination(byte[] buffer) // 3 shorts = 6 bytes
    {
        Buffer = buffer;
        FirstItem = new ItemDescriptor(buffer[(ItemDescriptor.size * 0)..(ItemDescriptor.size * 1)]);
        SecondItem = new ItemDescriptor(buffer[(ItemDescriptor.size * 1)..(ItemDescriptor.size * 2)]);
        Product = new ItemDescriptor(buffer[(ItemDescriptor.size * 2)..(ItemDescriptor.size * 3)]);
    }

    [JsonConstructor]
    public ItemCombination(ItemDescriptor firstItem, ItemDescriptor secondItem, ItemDescriptor product)
    {
        FirstItem = firstItem;
        SecondItem = secondItem;
        Product = product;
        ReconstructBuffer();
    }
}
