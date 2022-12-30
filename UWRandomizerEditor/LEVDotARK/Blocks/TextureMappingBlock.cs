namespace UWRandomizerEditor.LEVDotARK.Blocks
{
    public class TextureMappingBlock : Block
    {
        public static new int TotalBlockLength = 0x007a;

        public TextureMappingBlock(byte[] buffer, int levelnumber)
        {
            // Debug.Assert(buffer.Length == TotalBlockLength);
            Buffer = buffer;
            LevelNumber = levelnumber;
        }
    }
}