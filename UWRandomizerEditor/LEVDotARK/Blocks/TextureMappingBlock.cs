namespace UWRandomizerEditor.LEVDotARK.Blocks
{
    public class TextureMappingBlock: Block
    {
        public static new int TotalBlockLength = 0x007a;

        public TextureMappingBlock(byte[] buffer, int levelnumber)
        {
            // Debug.Assert(buffer.Length == TotalBlockLength);
            blockbuffer = buffer;
            LevelNumber = levelnumber;
        }

    public override string SaveBuffer(string? basePath = null, string filename = "")
    {
        return base.SaveBuffer(basePath, filename.Length == 0 ? $@"_TEXMAP_{LevelNumber}" : filename);
    }
}
}
