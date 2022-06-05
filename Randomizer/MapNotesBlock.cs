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
        public string? SaveBuffer(string basePath = "D:\\Dropbox\\UnderworldStudy\\studies\\LEV.ARK",
            string extraInfo = "")
        {
            if (extraInfo.Length == 0)
            {
                extraInfo = $@"_AUTOMAP_{LevelNumber}";
            }

            return StdSaveBuffer(blockbuffer, basePath, extraInfo);
        }
    }
}
