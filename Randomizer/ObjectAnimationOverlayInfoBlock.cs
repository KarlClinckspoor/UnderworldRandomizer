using static Randomizer.Utils;
using System.Diagnostics;

namespace Randomizer
{
    public class ObjectAnimationOverlayInfoBlock: Block, ISaveBinary
    {
        public static new int TotalBlockLength = 0x0180;

   /*
   This block contains entries with length of 6 bytes with infos about
   objects with animation overlay images from "animo.gr".
   It always is 0x0180 bytes long which leads to 64 entries.

   0000   Int16   link1
   0002   Int16   unk2
   0004   Int8    tile x coordinate
   0005   Int8    tile y coordinate

   link1's most significant 10 bits contain a link into the master object
   list, to the object that should get an animation overlay.
   */

        public ObjectAnimationOverlayInfoBlock(byte[] buffer, int levelnumber)
        {
            // Debug.Assert(buffer.Length == TotalBlockLength);
            blockbuffer = buffer;
            LevelNumber = levelnumber;
        }

        public bool SaveBuffer(string basepath = Settings.DefaultArkPath, string extrainfo = "")
        {
            if (extrainfo.Length == 0)
            {
                extrainfo = $@"_ObjAnimOverlayInfo_{LevelNumber}";
            }

            return StdSaveBuffer(blockbuffer, basepath, extrainfo);
        }

    }
}
