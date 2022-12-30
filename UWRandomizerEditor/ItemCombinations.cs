using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using UWRandomizerEditor.Interfaces;
using static UWRandomizerEditor.Utils;

namespace UWRandomizerEditor;

public class CombinationsFile : IBufferObject
{
    public List<ItemCombination> Combinations;
    private string _path;
    public byte[] Buffer { get; set; }

    public CombinationsFile(string path)
    {
        _path = path;
        ReadFileIntoBuffer();
        ProcessCombinations();
    }

    [MemberNotNull(nameof(Buffer))]
    private void ReadFileIntoBuffer()
    {
        if (!File.Exists(_path))
        {
            throw new FileNotFoundException("Could not read 'CMB.DAT' or equivalent!");
        }

        Buffer = File.ReadAllBytes(_path);
    }

    [MemberNotNull(nameof(Combinations))]
    private void ProcessCombinations()
    {
        Combinations = new List<ItemCombination>();
        // if (BitConverter.ToInt64(Buffer[^6..]) != 0) // Doesn't end in 0s
        if (Buffer[^6] != 0 | Buffer[^5] != 0 | Buffer[^4] != 0 | Buffer[^3] != 0 | Buffer[^2] != 0 |
            Buffer[^1] != 0) // Doesn't end in 0s
        {
            throw new ArithmeticException("Buffer does not end in six bytes of 0s!");
        }

        for (int i = 0; i < Buffer.Length - ItemCombination.Size; i += ItemCombination.Size)
        {
            byte[] buf = Buffer[i..(i + ItemCombination.Size)];
            Combinations.Add(new ItemCombination(Buffer[i..(i + ItemCombination.Size)]));
        }

        Combinations.Add(new FinalCombination());
    }

    public string SaveCombinations(string? path)
    {
        path ??= _path;
        if (File.Exists(path))
            Console.WriteLine("Overwriting file");
        File.WriteAllBytes(path, Buffer);

        return path;
    }

    [MemberNotNull(nameof(Buffer))]
    public bool ReconstructBuffer()
    {
        Buffer = new byte[Combinations.Count * ItemCombination.Size];
        int i = 0;
        foreach (var comb in Combinations)
        {
            comb.Buffer.CopyTo(Buffer, i * ItemCombination.Size);
            i++; // Oh how I wish for an `enumerate` in C#.
        }

        return true;
    }

    public void AddCombination(ItemCombination comb)
    {
        Combinations.Insert(Combinations.Count - 1, comb); // Inserts before null
        ReconstructBuffer();
    }

    public void RemoveCombination(int idx)
    {
        if (idx == Combinations.Count - 1)
        {
            Console.WriteLine("Can't remove final combination!");
            return;
        }

        if (idx >= Combinations.Count | idx < 0)
        {
            Console.WriteLine("Invalid bounds");
        }

        Combinations.RemoveAt(idx);
        ReconstructBuffer();
    }

    public CombinationsFile(List<ItemCombination> combinations, string path = "CMB.DAT")
    {
        this.Combinations = combinations;
        this._path = path;
        ReconstructBuffer();
    }

    /// <summary>
    /// Checks if the file ends with 3 zeros as combinations
    /// </summary>
    /// <returns></returns>
    public bool CheckEnding() // TODO: this is a bit redundant with the function above.
    {
        return Combinations[^1].FirstItem.itemID == 0 &
               Combinations[^1].SecondItem.itemID == 0 &
               Combinations[^1].Product.itemID == 0;
    }


    /// <summary>
    /// Checks if at least one item is destroyed upon combination
    /// </summary>
    /// <returns></returns>
    public bool CheckConsistency()
    {
        // return Combinations.All(cmb => cmb.FirstItem.IsDestroyed | cmb.SecondItem.IsDestroyed);
        foreach (var cmb in Combinations)
        {
            if (!(cmb.FirstItem.IsDestroyed | cmb.SecondItem.IsDestroyed) &
                (cmb.FirstItem.itemID != 0 & cmb.SecondItem.itemID != 0 & cmb.Product.itemID != 0))
            {
                return false;
            }
        }

        return true;
    }

    public void ExportAsJson(string filename)
    {
        string json = JsonSerializer.Serialize(Combinations,
            options: new JsonSerializerOptions()
            {
                WriteIndented = true,
                IncludeFields = true
            });
        File.WriteAllText(filename, json);
    }

    public static CombinationsFile ImportFromJson(string filename)
    {
        var temp = JsonSerializer.Deserialize<List<ItemCombination>>(File.ReadAllText(filename),
                       options: new JsonSerializerOptions()
                       {
                           IncludeFields = true,
                           PropertyNameCaseInsensitive = true
                       }) ??
                   throw new InvalidOperationException();

        var file = new CombinationsFile(temp, "CMB.DAT");

        if (!file.CheckConsistency())
        {
            Console.WriteLine(
                "One of the combinations has both items preserved. This won't work. Consider editing to remove that combination");
        }

        if (!file.CheckEnding())
        {
            Console.WriteLine("The file doesn't end in zeros. Trying to fix...");
            file.AddCombination(new FinalCombination());
            Console.WriteLine("Done");
        }

        file.Combinations[^1] =
            new FinalCombination(); // Replacing because the Deserializer made it into ItemCombination

        return file;
    }
}

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

public class FinalCombination : ItemCombination
{
    public FinalCombination() : base(new FinalEntry(), new FinalEntry(), new FinalEntry())
    {
    }
}

/// <summary>
/// An entry is a short where the first bit is whether it's destroyed or not and the remaining bits are the ItemID
/// </summary>
public class ItemDescriptor
{
    [JsonIgnore] public const int size = 2; // In bytes
    [JsonIgnore] public byte[] buffer;
    [JsonIgnore] private ushort entry;

    // TODO is Item ID 9 or 10 bits?
    public ushort itemID
    {
        get { return (ushort) GetBits(entry, 0x3FF, 0); }
        set { entry = (ushort) SetBits(entry, value, 0x3FF, 0); }
    }

    public bool IsDestroyed
    {
        get { return entry >> 15 == 1; }
        set { entry = (ushort) SetBits(entry, value ? 1 : 0, 0b1, 15); }
    }

    [JsonConstructor]
    public ItemDescriptor(ushort itemID, bool isDestroyed)
    {
        this.itemID = itemID;
        this.IsDestroyed = isDestroyed;
        buffer = BitConverter.GetBytes(entry);
    }

    public ItemDescriptor(byte[] buffer)
    {
        this.buffer = buffer;
        entry = BitConverter.ToUInt16(buffer);
    }

    public ItemDescriptor()
    {
        buffer = new byte[] {0, 0};
        entry = 0;
        itemID = 0;
        IsDestroyed = false;
    }
}

public class FinalEntry : ItemDescriptor
{
    public new byte[] buffer = {0, 0};
    private short entry = 0;
    [JsonInclude] public short itemID = 0;
    [JsonInclude] public bool IsDestroyed = false;

    [JsonConstructor]
    public FinalEntry(short itemID, bool isDestroyed) : this()
    {
    }

    public FinalEntry() : base()
    {
    }
}