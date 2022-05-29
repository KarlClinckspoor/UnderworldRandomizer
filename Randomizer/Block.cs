namespace Randomizer
{
    public abstract class Block
    {
        public byte[] blockbuffer;
        public int LevelNumber;
        public static int TotalBlockLength;

        protected Block(): this(new byte[] { }, -1){ }

        public Block(byte[] blockbuffer, int levelNumber)
        {
            this.blockbuffer = blockbuffer;
            this.LevelNumber = levelNumber;
        }
    }
}