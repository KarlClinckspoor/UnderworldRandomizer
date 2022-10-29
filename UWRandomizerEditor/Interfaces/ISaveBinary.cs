namespace UWRandomizerEditor.Interfaces;

public interface ISaveBinary
{
    /// <summary>
    /// Saves the object's own buffer to a specific path.
    /// </summary>
    /// <param name="basePath">Base path to the file (e.g. folder structure) </param>
    /// <param name="filename">Extra info to add (e.g. name of file) </param>
    /// <returns>Path to the saved object. If couldn't be saved, returns null</returns>
    public string SaveBuffer(string? basePath = null, string filename = "");
}