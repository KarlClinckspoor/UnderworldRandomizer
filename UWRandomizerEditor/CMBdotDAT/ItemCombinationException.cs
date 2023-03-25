namespace UWRandomizerEditor.CMBdotDAT;

[Serializable]
public class ItemCombinationException: Exception
{
    public ItemCombinationException() { }
    public ItemCombinationException(string message) : base(message) { }
    public ItemCombinationException(string message, Exception innerException) : base(message, innerException) { }
}
