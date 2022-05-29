namespace Randomizer;

public interface ISaveBinary
{
    // TODO: Make an interface that also returns the path of the saved file.
    public bool SaveBuffer(string basepath = Settings.DefaultArkPath, string extrainfo = "");
}