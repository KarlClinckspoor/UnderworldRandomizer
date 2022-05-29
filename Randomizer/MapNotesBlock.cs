using static Randomizer.Utils;

namespace Randomizer
{
    public class MapNotesBlock: Block, ISaveBinary
    {
        public static new int TotalBlockLength = 1; // TODO: Is this fixed or variable length?
        public MapNotesBlock(byte[] buffer, int levelnumber)
        {
            //Debug.Assert(buffer.Length, TotalBlockLength ) 
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
}
