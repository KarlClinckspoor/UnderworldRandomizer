using System.Diagnostics.CodeAnalysis;
using static Randomizer.Utils;
namespace Randomizer;

public class CombinationsFile// : ISaveBinary
{
    public List<ItemCombination> Combinations;
    private string _path;
    public byte[] Buffer;

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
        ProcessCombinations();
    }

    [MemberNotNull(nameof(Combinations))]
    private void ProcessCombinations()
    {
        Combinations = new List<ItemCombination>();
        if (BitConverter.ToInt64(Buffer[^6..]) != 0) // Doesn't end in 0s
        {
            throw new ArithmeticException("Buffer does not end in six bytes of 0s!");
        }
        
        for (int i = 0; i < Buffer.Length - ItemCombination.Size; i += ItemCombination.Size)
        {
            byte[] buf = Buffer[i..(i + ItemCombination.Size)];
            Combinations.Add(new ItemCombination(Buffer[i..(i+ItemCombination.Size)]));
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

    private void UpdateBuffer()
    {
        Buffer = new byte[Combinations.Count * ItemCombination.Size];
        int i = 0;
        foreach (var comb in Combinations)
        {
            comb.Buffer.CopyTo(Buffer, i * ItemCombination.Size);
            i++; // Oh how I wish for an `enumerate` in C#.
        }
    }

    public void AddCombination(ItemCombination comb)
    {
        Combinations.Insert(Combinations.Count - 2, comb); // Inserts before null
        UpdateBuffer();
    }
}

public class ItemCombination: ISaveBinary
{
    public const int NumOfItemsInCombination = 3;
    public const int Size = NumOfItemsInCombination * ItemDescriptor.size;
    
    public byte[] Buffer = new byte[NumOfItemsInCombination * ItemDescriptor.size];
    public ItemDescriptor FirstItem;
    public ItemDescriptor SecondItem;
    public ItemDescriptor Product;

    public ItemCombination(byte[] buffer) // 3 shorts = 6 bytes
    {
        Buffer = buffer;
        FirstItem = new ItemDescriptor(buffer[(ItemDescriptor.size*0)..(ItemDescriptor.size * 1)]);
        SecondItem = new ItemDescriptor(buffer[(ItemDescriptor.size * 1)..(ItemDescriptor.size * 2)]);
        Product = new ItemDescriptor(buffer[(ItemDescriptor.size * 2)..(ItemDescriptor.size * 3)]);
    }

    public ItemCombination(ItemDescriptor firstItem, ItemDescriptor secondItem, ItemDescriptor product)
    {
        FirstItem = firstItem;
        SecondItem = secondItem;
        Product = product;
        
        firstItem.buffer.CopyTo(Buffer, ItemDescriptor.size * 0);
        secondItem.buffer.CopyTo(Buffer, ItemDescriptor.size * 1);
        product.buffer.CopyTo(Buffer, ItemDescriptor.size * 2);
    }

    public string? SaveBuffer(string? basePath = null, string extraInfo = "")
    {
        if (basePath is null)
        {
            basePath = Settings.DefaultBinaryTestsPath;
        }
        return StdSaveBuffer(Buffer, basePath, extraInfo);
    }
}

public class FinalCombination: ItemCombination
{
    public FinalCombination(): base(new FinalEntry(), new FinalEntry(), new FinalEntry()) {}
}

/// <summary>
/// An entry is a short where the first bit is whether it's destroyed or not and the remaining bits are the ItemID
/// </summary>
public class ItemDescriptor
{
    public const int size = 2; // In bytes
    public byte[] buffer;
    private short entry;

    private short itemID;     // TODO: Make this a property. When set, modify buffer
    private bool IsDestroyed; // TODO: Make this a property. When set, modify buffer

    public ItemDescriptor(short itemID, bool isDestroyed)
    {
        this.itemID = itemID;
        this.IsDestroyed = isDestroyed;

        entry |= itemID;

        if (isDestroyed)
        {
            entry |= 0x80;
        }
        
        UpdateBuffer();
    }

    public ItemDescriptor(byte[] buffer)
    {
        this.buffer = buffer;
        UpdateEntry();
    }

    public ItemDescriptor()
    {
        buffer = new byte[] {0, 0};
        entry = 0;
        itemID = 0;
        IsDestroyed = false;
    }

    [MemberNotNull(nameof(buffer))] // Tells compiler UpdateBuffer assures buffer isn't null.
    public void UpdateBuffer()
    {
        buffer = BitConverter.GetBytes(entry);
    }

    public void UpdateEntry()
    {
        entry = BitConverter.ToInt16(buffer);

        // IsDestroyed = (buffer[0] & 0x8) == 1;
        IsDestroyed = GetBits(entry, 0x80, 0) == 1; // TODO: check this, I always forget how my functions work.

        // Removing first bit by shifting left and right. Note that shifts convert into ints!
        itemID = (short) ( ((short) (entry << 1)) >> 1);

        // Is this easier?
        itemID = (short) GetBits(entry, 0x7FFF, 0);
    }
    
}

public class FinalEntry : ItemDescriptor
{
    public new byte[] buffer = {0, 0};
    private short entry = 0;
    private short itemID = 0;
    private bool IsDestroyed = false;

    public FinalEntry(short itemID, bool isDestroyed): this()
    { }

    public FinalEntry(): base()
    { }

}