namespace UWRandomizerEditor.LEV.Blocks;

public class BlockOperationException: Exception
{
    public BlockOperationException() { }

    public BlockOperationException(string? message) : base(message) { }

    public BlockOperationException(string? message, Exception? innerException) : base(message, innerException) { }
}