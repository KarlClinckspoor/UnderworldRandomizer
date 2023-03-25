using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using UWRandomizerEditor.Interfaces;

namespace UWRandomizerEditor.CMBdotDAT;

/// <summary>
/// This class simulates the sequence of item combinations present in CMB.DAT.
/// For UW1, the limit of combinations is CombinationsFile.UW1CombinationLimit/>
/// </summary>
public class CombinationsFile : IBufferObject
{
    public const uint UW1CombinationLimit = 10; // TODO: Recheck if it's 10 or 11.

    private List<ItemCombination> _combinations;

    public List<ItemCombination> Combinations
    {
        get => _combinations;

        [MemberNotNull(nameof(_combinations))]
        [MemberNotNull(nameof(_buffer))]
        private set
        {
            _combinations = value;
            ReconstructBuffer();
        }
    }

    /// <summary>
    /// Contains the path where the file was read from. If it's created artificially with a list of combinations, this becomes null.
    /// </summary>
    private readonly string? _path;

    /// <summary>
    /// Private buffer which is accessed only after passing through checks.
    /// </summary>
    private byte[] _buffer;

    /// <summary>
    /// <inheritdoc cref="IBufferObject.Buffer"/>
    /// The getter reconstructs (updates) a private buffer and when setting, the private buffer is replaced and the
    /// item combinations are redone.
    /// </summary>
    public byte[] Buffer
    {
        get
        {
            ReconstructBuffer();
            return _buffer;
        }
        set
        {
            _buffer = value;
            ProcessCombinations();
        }
    }


    /// <summary>
    /// Loads the buffer of CMB.DAT
    /// </summary>
    /// <exception cref="FileNotFoundException">Thrown if file can't be found.</exception>
    [MemberNotNull(nameof(_buffer))]
    private void ReadFileIntoBuffer()
    {
        if (!File.Exists(_path))
            throw new FileNotFoundException("Could not read 'CMB.DAT' or equivalent!");

        _buffer = File.ReadAllBytes(_path);
    }

    /// <summary>
    /// Processes the buffer into several ItemCombinations
    /// </summary>
    /// <exception cref="ItemCombinationException"></exception>
    [MemberNotNull(nameof(_combinations))]
    private void ProcessCombinations()
    {
        _combinations = new List<ItemCombination>();
        var correctBufferEndUW1 = new byte[] {0, 0, 0, 0, 0, 0};
        if (!_buffer[^6..].SequenceEqual(correctBufferEndUW1))
        {
            throw new ItemCombinationException("Buffer does not end in six bytes of 0s!");
        }

        for (int i = 0; i < _buffer.Length - ItemCombination.Size; i += ItemCombination.Size)
        {
            _combinations.Add(new ItemCombination(_buffer[i..(i + ItemCombination.Size)]));
            Debug.WriteLineIf(i >= UW1CombinationLimit,
                $"Exceeding UW1's item limit of {UW1CombinationLimit}! Anything after this won't be considered");
        }

        _combinations.Add(new FinalCombination());
    }

    /// <summary>
    /// Reconstructs the buffer by joining the individual buffers of each combination.
    /// </summary>
    /// <returns></returns>
    public bool ReconstructBuffer()
    {
        var buffer = new byte[Combinations.Count * ItemCombination.Size];
        int i = 0;
        foreach (var comb in Combinations)
        {
            comb.Buffer.CopyTo(buffer, i * ItemCombination.Size);
            i++;
        }

        _buffer = buffer;
        return true;
    }

    /// <summary>
    /// Adds a new combination to the file.
    /// </summary>
    /// <param name="comb"></param>
    public void AddCombination(ItemCombination comb)
    {
        Combinations.Insert(Combinations.Count - 1, comb); // Inserts before null
    }

    /// <summary>
    /// Removes a combination at a certain index.
    /// </summary>
    /// <param name="idx"></param>
    public void RemoveCombinationAt(int idx)
    {
        if (idx == Combinations.Count - 1)
        {
            throw new ItemCombinationException("Can't remove final combination!");
        }

        if (idx >= Combinations.Count | idx < 0)
        {
            throw new IndexOutOfRangeException($"Idx {idx} is out of range");
        }

        Combinations.RemoveAt(idx);
    }

    /// <summary>
    /// Creates a new instance given a path.
    /// </summary>
    /// <param name="path"></param>
    public CombinationsFile(string path)
    {
        _path = path;
        ReadFileIntoBuffer();
        ProcessCombinations();
    }

    /// <summary>
    /// Creates a new instance given a list of combinations and 
    /// </summary>
    /// <param name="combinations"></param>
    /// <param name="path"></param>
    public CombinationsFile(List<ItemCombination> combinations)
    {
        Combinations = combinations;
        _path = null;
    }

    /// <summary>
    /// Checks if the file ends with 3 zeros as combinations
    /// </summary>
    /// <returns></returns>
    private bool CheckEnding()
    {
        return Combinations[^1] is FinalCombination;
    }


    /// <summary>
    /// Checks if at least one item is destroyed upon combination
    /// </summary>
    /// <returns></returns>
    public bool CheckConsistency()
    {
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

    /// <summary>
    /// Exports the object as a JSON file
    /// </summary>
    /// <param name="filename"></param>
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

    /// <summary>
    /// From a JSON file, creates a new instance of a CombinationsFile, which can then be replace the original file if desired.
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static CombinationsFile ImportFromJson(string filename)
    {
        var temp = JsonSerializer.Deserialize<List<ItemCombination>>(File.ReadAllText(filename),
                       options: new JsonSerializerOptions()
                       {
                           IncludeFields = true,
                           PropertyNameCaseInsensitive = true
                       }) ??
                   throw new InvalidOperationException();

        var file = new CombinationsFile(temp);

        if (!file.CheckConsistency())
        {
            throw new ItemCombinationException(
                "One of the combinations has both items preserved. This won't work. Consider editing to remove that combination");
        }

        if (!file.CheckEnding())
        {
            throw new ItemCombinationException(
                "The file doesn't end with a sequence of zeroes (FinalCombination). Please fix.");
        }

        file.Combinations[^1] =
            (FinalCombination) file.Combinations[^1]; // Replacing because the Deserializer made it into ItemCombination

        return file;
    }
}