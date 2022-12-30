using UWRandomizerEditor.Interfaces;
using static UWRandomizerEditor.Utils;

namespace UWRandomizerEditor.LEVDotARK.Blocks
{
    public abstract class Block : IBufferObject
    {
        public byte[] Buffer { get; set; }

        public bool ReconstructBuffer()
        {
            throw new NotImplementedException();
        }

        public int LevelNumber;

        public int TotalBlockLength
        {
            get { return Buffer.Length; }
        }

        protected Block() : this(new byte[] { }, -1)
        {
        }

        public Block(byte[] buffer, int levelNumber)
        {
            this.Buffer = buffer;
            this.LevelNumber = levelNumber;
        }
    }
}