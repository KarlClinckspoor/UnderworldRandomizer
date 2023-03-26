namespace UWRandomizerEditor.CMBdotDAT;

/// <summary>
/// Exception that occurs when trying to create or modify ItemCombinations with invalid data.
/// </summary>
[Serializable]
public class ItemCombinationException: Exception
{
    public ItemCombinationException() { }
    public ItemCombinationException(string message) : base(message) { }
    public ItemCombinationException(string message, Exception innerException) : base(message, innerException) { }
}
