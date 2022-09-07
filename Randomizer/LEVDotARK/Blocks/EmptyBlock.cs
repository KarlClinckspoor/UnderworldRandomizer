namespace Randomizer.LEVDotARK.Blocks
{
    public class EmptyBlock: Block
    {
        public static new int TotalBlockLength = 0;
        public new byte[] blockbuffer = Array.Empty<byte>(); // VS suggestion
        public EmptyBlock() { }
    }
}
