﻿using static Randomizer.Utils;

namespace Randomizer
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

    public override string? SaveBuffer(string? basePath = null, string extraInfo = "")
    {
        return base.SaveBuffer(basePath, extraInfo.Length == 0 ? $@"_TEXMAP_{LevelNumber}" : extraInfo);
    }
}
}
