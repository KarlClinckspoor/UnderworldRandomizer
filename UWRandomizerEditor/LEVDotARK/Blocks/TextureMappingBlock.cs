using System.Diagnostics;

namespace UWRandomizerEditor.LEVDotARK.Blocks
{
    public sealed class TextureMappingBlock : Block
    {
        public new const int FixedBlockLength = 0x007a;

        public override bool ReconstructBuffer()
        {
            // Since there's no operations that can change this block, this doesn't do anything.
            return true;
        }

        public TextureMappingBlock(byte[] buffer, int levelnumber)
        {
            if (buffer.Length != FixedBlockLength)
            {
                throw new ArgumentException(
                    $"Length of buffer ({buffer.Length}) is incompatible with expected TextureMappingBlock length ({FixedBlockLength})");
            }

            Buffer = new byte[FixedBlockLength];
            buffer.CopyTo(Buffer, 0);
            LevelNumber = levelnumber;
        }
    }
}