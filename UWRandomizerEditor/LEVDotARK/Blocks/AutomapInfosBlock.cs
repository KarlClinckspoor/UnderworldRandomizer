namespace UWRandomizerEditor.LEVDotARK.Blocks;
public class AutomapInfosBlock: Block
{
    public static new int TotalBlockLength = 0x1000;

    public AutomapInfosBlock(byte[] buffer, int levelnumber)
        {
            // Debug.Assert(buffer.Length == TotalBlockLength);
            blockbuffer = buffer;
            LevelNumber = levelnumber;
        }

    public override string SaveBuffer(string? basePath = null, string filename = "")
    {
        return base.SaveBuffer(basePath, filename.Length == 0 ? $@"_AUTOMAP_{LevelNumber}" : filename);
    }

}