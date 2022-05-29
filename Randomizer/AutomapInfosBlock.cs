using static Randomizer.Utils;
using System.Diagnostics;

namespace Randomizer;
public class AutomapInfosBlock: Block, ISaveBinary
{
    public static new int TotalBlockLength = 0x1000;

    public AutomapInfosBlock(byte[] buffer, int levelnumber)
        {
            Debug.Assert(buffer.Length == TotalBlockLength);
            blockbuffer = buffer;
            LevelNumber = levelnumber;
        }

    public bool SaveBuffer(string basepath = Settings.DefaultArkPath, string extrainfo = "")
    {
        if (extrainfo.Length == 0)
        {
            extrainfo = $@"_AUTOMAP_{LevelNumber}";
        }

        return StdSaveBuffer(blockbuffer, basepath, extrainfo);
    }

}