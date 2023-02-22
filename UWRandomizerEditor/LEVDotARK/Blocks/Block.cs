using UWRandomizerEditor.Interfaces;

namespace UWRandomizerEditor.LEVdotARK.Blocks
{
    public abstract class Block : IBufferObject
    {
        public byte[] Buffer { get; set; } = Array.Empty<byte>();

        public abstract bool ReconstructBuffer();

        public int LevelNumber = -1;

        /// If 0, means the block has no fixed length
        public const int FixedBlockLength = 0;

        protected Block()
        {
        }

        protected Block(byte[] buffer, int levelNumber)
        {
            Buffer = new byte[buffer.Length];
            buffer.CopyTo(Buffer, 0);
            LevelNumber = levelNumber;
        }
    }
}