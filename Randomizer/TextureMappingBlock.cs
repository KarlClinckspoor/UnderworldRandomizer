using static Randomizer.Utils;
using System.Diagnostics;

namespace Randomizer
{
    public class TextureMappingBlock: Block, ISaveBinary
    {
        public static new int TotalBlockLength = 0x007a;

        public TextureMappingBlock(byte[] buffer, int levelnumber)
        {
            // Debug.Assert(buffer.Length == TotalBlockLength);
            blockbuffer = buffer;
            LevelNumber = levelnumber;
        }

    public string? SaveBuffer(string basePath = "D:\\Dropbox\\UnderworldStudy\\studies\\LEV.ARK", string extraInfo = "")
    {
        if (extraInfo.Length == 0)
        {
            extraInfo = $@"_TEXMAP_{LevelNumber}";
        }

        return StdSaveBuffer(blockbuffer, basePath, extraInfo);
    }
    }
}
