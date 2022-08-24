using static Randomizer.Utils;

namespace Randomizer
{
    public abstract class Block: ISaveBinary
    {
        public byte[] blockbuffer;
        public int LevelNumber;
        public int TotalBlockLength;

        protected Block(): this(new byte[] { }, -1){ }

        public Block(byte[] blockbuffer, int levelNumber)
        {
            this.blockbuffer = blockbuffer;
            this.TotalBlockLength = blockbuffer.Length;
            this.LevelNumber = levelNumber;
        }

        public virtual string? SaveBuffer(string? basePath = null, string extraInfo = "")
        {
            if (basePath is null)
            {
                basePath = Settings.DefaultBinaryTestsPath;
            }
            return StdSaveBuffer(blockbuffer, basePath, extraInfo);
        }
    }
}