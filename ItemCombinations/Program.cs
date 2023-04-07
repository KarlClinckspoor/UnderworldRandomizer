using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using UWRandomizerEditor.CMBdotDAT;

namespace ItemCombinations;

public static class Program
{
    public static void Main()
    {
        TUICombinations tuiCombinations = new TUICombinations();
        tuiCombinations.MainLoop();
    }

}

internal class TUICombinations
{
    private CombinationsFile _cmb;
    private string _path;
    private List<JsonEntry> IDsAndNames;
    
    [MemberNotNull(nameof(_cmb)), MemberNotNull(nameof(_path)), MemberNotNull(nameof(IDsAndNames))]
    private void Load()
    {
        Console.WriteLine("Loading item names");
        var opt = new JsonSerializerOptions() {PropertyNameCaseInsensitive = true, IncludeFields = true};
        IDsAndNames = JsonSerializer.Deserialize<List<JsonEntry>>(File.ReadAllText("objects.json"), opt) ?? throw new InvalidOperationException();
        
        Console.WriteLine("Please input CMB.DAT file path [nothing for CWD]");
        _path = Console.ReadLine() ?? throw new InvalidOperationException();
        if (_path.Length == 0)
        {
            _path = "CMB.DAT";
        }

        if (!File.Exists(_path))
            throw new FileNotFoundException("Could not find CMB.DAT. Please provide a valid path");
        _cmb = new CombinationsFile(_path);
    }

    private void PrintExistingCombinations()
    {
        int i = 0;
        foreach (var combination in _cmb.Combinations)
        {
            if (combination is FinalCombination)
            {
                Console.WriteLine("End");
                return;
            }
            
            int id1 = combination.FirstItem.ItemID;
            string destroy1 = combination.FirstItem.IsDestroyed ? "*" : "";
            int id2 = combination.SecondItem.ItemID;
            string destroy2 = combination.SecondItem.IsDestroyed ? "*" : "";
            int prod = combination.Product.ItemID;
            string destroy3 = combination.Product.IsDestroyed ? "*" : "";
            
            Console.WriteLine(
                $"{i}: ({id1}:0x{id1:X}){IDsAndNames[id1].Name}{destroy1}+" +
                $"({id2}:0x{id2:X}){IDsAndNames[id2].Name}{destroy2}=" +
                $"({prod}:0x{prod:X}){IDsAndNames[prod].Name}{destroy3}"
            );
            i++;
        }
    }

    private void PrintAllItems()
    {
        foreach (var IDItem in IDsAndNames)
        {
            Console.WriteLine($"{IDItem.ID}(0x{IDItem.ID:X}):{IDItem.Name}");
        }
    }

    private void AddCombination()
    {
        Console.WriteLine("Input ID of item 1");
        var id1 = Console.ReadLine();
        Console.WriteLine("Will it be destroyed? [Y/n]");
        var destroy1Q = Console.ReadLine();
        var destroy1 = false;
        
        if ((id1 is null) | (destroy1Q is null) | !ushort.TryParse(id1, out var id1Val))
        {
            Console.WriteLine("Invalid input. Quitting.");
            return;
        }
        if (destroy1Q != "n")
        {
            destroy1 = true;
        }
        
        Console.WriteLine("Input ID of item 2");
        var id2 = Console.ReadLine();
        Console.WriteLine("Will it be destroyed? [Y/n]");
        string? destroy2Q = Console.ReadLine();
        bool destroy2 = false;
        
        if ((id2 is null) | (destroy2Q is null) | !ushort.TryParse(id2, out var id2Val))
        {
            Console.WriteLine("Invalid input. Quitting.");
            return;
        }
        if (destroy2Q != "n")
        {
            destroy2 = true;
        }
        
        Console.WriteLine("Input ID of product");
        string? prod = Console.ReadLine();
        if (prod is null | !UInt16.TryParse(prod, out var prodVal))
        {
            Console.WriteLine("Invalid input. Quitting.");
            return;
        }
        

        var entry = new ItemCombination(
            new ItemDescriptor(id1Val, destroy1),
            new ItemDescriptor(id2Val, destroy2),
            new ItemDescriptor(prodVal, false)
        );
        
        _cmb.AddCombination(entry);
    }

    public void MainLoop()
    {
        while (true)
        {
            Console.WriteLine("What do you want to do?\n" +
                              "(P)rint existing combinations\n" +
                              "(A)dd combination\n" +
                              "(R)emove combination\n" +
                              "(D)isplay all item IDs\n" +
                              "(S)ave changes\n" +
                              "(E)xport as JSON\n" +
                              "(I)mport from JSON\n" +
                              "(Q)uit");
            string choice = Console.ReadLine() ?? throw new InvalidOperationException();
            var validChoices = new List<string> {"P", "A", "D", "Q", "S", "R", "E", "I"};
            choice = choice.ToUpper();
            if (!validChoices.Contains(choice))
            {
                Console.WriteLine("Invalid choice.");
            }

            if (choice == "Q")
            {
                break;
            }
            
            switch (choice)
            {
                case "P":
                    PrintExistingCombinations();
                    break;
                case "A":
                    AddCombination();
                    break;
                case "D":
                    PrintAllItems();
                    break;
                case "S":
                    SaveCombinations();
                    break;
                case "R":
                    RemoveCombination();
                    break;
                case "E":
                    ExportAsJson();
                    break;
                case "I":
                    ImportFromJson();
                    break;
            }
        }
    }

    private void ExportAsJson()
    {
        Console.WriteLine("Exporting. What will be the filename? (nothing for CMB.DAT.json)");
        string choice = Console.ReadLine() ?? throw new InvalidOperationException();
        if (choice.Length == 0)
            choice = "CMB.DAT.json";
        this._cmb.ExportAsJson(choice);
        Console.WriteLine($"Exported as \"{choice}\"");
    }

    private void ImportFromJson()
    {
        Console.WriteLine("What is the filename to load (default is \"CMB.DAT.json\")");
        string filename = Console.ReadLine() ?? throw new InvalidOperationException();
        if (filename.Length == 0)
        {
            filename = "CMB.DAT.json";
        }
        _cmb = CombinationsFile.ImportFromJson(filename);
        Console.WriteLine("Loaded");
    }

    private void RemoveCombination()
    {
        PrintExistingCombinations();
        Console.WriteLine("Which combination do you want to remove?");
        string choice = Console.ReadLine() ?? throw new InvalidOperationException();
        if (int.TryParse(choice, out int val))
        {
            _cmb.RemoveCombinationAt(val);
            Console.WriteLine("Removed!");
        }
        
    }

    private void SaveCombinations()
    {
        File.WriteAllBytes(_path, _cmb.Buffer);
        Console.WriteLine("Saved!");
    }

    public TUICombinations()
    {
        Load();
    }
    
}

class JsonEntry
{
    public int ID { get; set; }
    public string ItemID { get; set; }
    public string Name { get; set; }

    [JsonConstructor]
    public JsonEntry(string ItemID, string Name)
    {
        if (int.TryParse(ItemID, out int temp))
        {
            this.ID = temp;
        }
        this.ItemID = ItemID;
        this.Name = Name;
    }

}
       