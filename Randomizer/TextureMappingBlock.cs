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

    public bool SaveBuffer(string basepath = Settings.DefaultArkPath, string extrainfo = "")
    {
        if (extrainfo.Length == 0)
        {
            extrainfo = $@"_TEXMAP_{LevelNumber}";
        }

        return StdSaveBuffer(blockbuffer, basepath, extrainfo);
    }
    }
}
