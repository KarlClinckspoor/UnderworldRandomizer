using System.Text.Json.Serialization;

namespace UWRandomizerEditor.CMBdotDAT;

/// <summary>
/// Class that represents the final entry in the file, which is just all zeroes.
/// </summary>
public class FinalDescriptor : ItemDescriptor
{
    [JsonConstructor]
    public FinalDescriptor() { }
}