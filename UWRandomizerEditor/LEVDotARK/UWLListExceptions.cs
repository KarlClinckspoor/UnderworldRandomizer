namespace UWRandomizerEditor.LEVdotARK;

[Serializable]
public class UWLinkedListException: Exception
{
    public UWLinkedListException() { }
    public UWLinkedListException(string message) : base(message) { }
    public UWLinkedListException(string message, Exception innerException) : base(message, innerException) { }
}