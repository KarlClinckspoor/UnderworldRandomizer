using System.Text.Json.Serialization;

namespace UWRandomizerEditor.CMBdotDAT;

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
