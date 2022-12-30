namespace UWRandomizerEditor.LEVDotARK.Blocks;

public class AutomapInfosBlock : Block
{
    public static new int TotalBlockLength = 0x1000;

    public AutomapInfosBlock(byte[] buffer, int levelnumber)
    {
        // Debug.Assert(buffer.Length == TotalBlockLength);
        Buffer = buffer;
        LevelNumber = levelnumber;
    }
}