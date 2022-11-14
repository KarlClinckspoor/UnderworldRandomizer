using UWRandomizerEditor.Interfaces;
using static UWRandomizerEditor.Utils;

namespace UWRandomizerEditor.LEVDotARK.Blocks
{
    public abstract class Block: ISaveBinary
    {
        public byte[] blockbuffer;
        public int LevelNumber;

        public int TotalBlockLength
        {
            get
            {
                return blockbuffer.Length;
            }
        }

        protected Block(): this(new byte[] { }, -1){ }

        public Block(byte[] blockbuffer, int levelNumber)
        {
            this.blockbuffer = blockbuffer;
            this.LevelNumber = levelNumber;
        }

        public virtual string SaveBuffer(string? basePath = null, string filename = "")
        {
            if (basePath is null)
            {
                basePath = Settings.DefaultBinaryTestsPath;
            }
            return StdSaveBuffer(blockbuffer, basePath, filename);
        }
    }
}