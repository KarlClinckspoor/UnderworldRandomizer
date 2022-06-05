using System.Diagnostics;
using static Randomizer.Utils;

namespace Randomizer
{
    
    public class TileInfo: ISaveBinary
    {
        public const int Size = 4;
        public static readonly IDictionary<int, string> TileTypeDescriptors = new Dictionary<int, string>()
        {
            { 0, "#SOLID" },
            { 1, "#OPEN" },
            { 2, "#DIAG_SE" },
            { 3, "#DIAG_SW" },
            { 4, "#DIAG_NE" },
            { 5, "#DIAG_NW" },
            { 6, "#SLP_N" },
            { 7, "#SLP_S" },
            { 8, "#SLP_E" },
            { 9, "#SLP_W" }
        };
        public static readonly IDictionary<int, char> TileCharReplacements = new Dictionary<int, char>()
        {
            { 0, '#' },
            { 1, '_' },
            { 2, '/' },
            { 3, '\\' },
            { 4, '\\' },
            { 5, '/' },
            { 6, '^' },
            { 7, 'v' },
            { 8, '>' },
            { 9, '<' }
        };

        // Defined in the constructor
        public int Entry;
        public byte[] TileBuffer;
        // todo: Offset and EntryNum are linearly dependent. I should remove their "independence" here.
        public int EntryNum;
        public int Offset;
        public int LevelNum;

        public int TileType
        {
            //get { return Entry & 0b1111; }
            //set { Entry |= ((value & 0b1111) << 0); UpdateBuffer(); } // This only works if Entry is all zeros. Have to clear it first.
            get { return GetBits(Entry, 0b1111, 0); }
            set { Entry = SetBits(Entry, value, 0b1111, 0); UpdateBuffer(); }
        }

        public int TileHeight
        {
            // get { return (Entry >> 4) & 0b1111; }
            // set { Entry |= ((value & 0b1111) << 4); UpdateBuffer(); }
            get { return GetBits(Entry, 0b1111, 4); }
            set { Entry = SetBits(Entry, value, 0b1111, 4); UpdateBuffer(); }
        }

        public int Light
        {
            // get { return (Entry >> 8) & 0b1; }
            // set { Entry |= ((value & 0b1) << 8); UpdateBuffer(); }
            get { return GetBits(Entry, 0b1, 8); }
            set { Entry = SetBits(Entry, value, 0b1, 8); UpdateBuffer(); }
        }
        
        // todo: recheck this.
        public int Bit9
        {
            // get { return ((Entry >> 9) & 0b1); }
            // set { Entry |= ((value & 0b1) << 9); UpdateBuffer(); }
            get { return GetBits(Entry, 0b1, 9); }
            set { Entry = SetBits(Entry, value, 0b1, 9); UpdateBuffer(); }
        }

        public int FloorTextureIdx
        {
            // get { return (Entry >> 10) & 0b1111; }
            // set { Entry |= ((value & 0b1111) << 10); UpdateBuffer(); }
            get { return GetBits(Entry, 0b1111, 10); }
            set { Entry = SetBits(Entry, value, 0b1111, 10); UpdateBuffer(); }
        }
        public int NoMagic
        {
            // get { return (Entry >> 14) & 0b1; }
            // set { Entry |= ((value & 0b1) << 14); UpdateBuffer(); }
            get { return GetBits(Entry, 0b1, 14); }
            set { Entry = SetBits(Entry, value, 0b1, 14); UpdateBuffer(); }
        }
        public int DoorBit
        {
            // get { return (Entry >> 15) & 0b1; }
            // set { Entry |= ((value & 0b1) << 15); UpdateBuffer(); }
            get { return GetBits(Entry, 0b1, 15); }
            set { Entry = SetBits(Entry, value, 0b1, 15); UpdateBuffer(); }
        }
        public int WallTextureIdx
        {
            // get { return (Entry >> 16) & 0b111111; }
            // set { Entry |= ((value & 0b111111) >> 16); UpdateBuffer(); }
            get { return GetBits(Entry, 0b111111, 16); }
            set { Entry = SetBits(Entry, value, 0b111111, 16); UpdateBuffer(); }
        }
        public int FirstObjIdx
        {
            // get { return (Entry >> 22) & 0b1111111111; }
            // set { Entry |= ((value & 0b1111111111) << 22); UpdateBuffer(); }
            get { return GetBits(Entry, 0b1111111111, 22);}
            set { Entry = SetBits(Entry, value, 0b1111111111, 22); UpdateBuffer(); }
        }

        // TODO: Maybe it would be convenient to keep a list of "interesting" objects.
        // For instance, plaques and doors aren't that interesting to shuffle
        // So if I keep a list of interesting objects, it should make shuffling them easier.
        // I already have a class for objects with textures, which are the ones I want to keep in place.
        // So it's a matter of populating the lists appropriately.
        public MobileObject[] MobileObjects = new MobileObject[0];
        public GameObject[] StaticObjects = new GameObject[0];

        public void PopulateObjectList(MobileObject[] gameObjects)
        {
            Console.WriteLine("'PopulateObjectList' is UNTESTED!");

            if (FirstObjIdx == 0) // no objects in tile.
                return;
            
            List<MobileObject> lst = new List<MobileObject>();
            
            // TODO: HIGHLY INEFFICIENT! TRY TO GET A DICT OR SOME TYPE OF MANAGER!
            int safetycounter = 0;
            int maxcounter = 500;
            int currentIdx = FirstObjIdx;
            while (currentIdx != 0) 
            {
                safetycounter++;
                if (safetycounter >= maxcounter)
                {
                    Console.WriteLine("WARNING! Encountered potentially infinite loop when populating ObjectList!");
                    break;
                }

                MobileObject currobj = gameObjects[currentIdx];
                currentIdx = currobj.next;
                lst.Add(currobj);

            }

            MobileObjects = lst.ToArray();
        }
        
        public void PopulateObjectList(GameObject[] gameObjects)
        {
            Console.WriteLine("'PopulateObjectList' is UNTESTED!");

            if (FirstObjIdx == 0) // no objects in tile.
                return;
            
            List<GameObject> lst = new List<GameObject>();
            
            // TODO: HIGHLY INEFFICIENT! TRY TO GET A DICT OR SOME TYPE OF MANAGER!
            int safetycounter = 0;
            int maxcounter = 500;
            int currentIdx = FirstObjIdx;
            while (currentIdx != 0) 
            {
                safetycounter++;
                if (safetycounter >= maxcounter)
                {
                    Console.WriteLine("WARNING! Encountered potentially infinite loop when populating ObjectList!");
                    break;
                }

                GameObject currobj = gameObjects[currentIdx];
                currentIdx = currobj.next;
                lst.Add(currobj);

            }

            StaticObjects = lst.ToArray();
        }

        // public void PopulateObjectList(GameObject[] gameObjects)
        // {
        //     Console.WriteLine("' PopulateObjectList' is UNTESTED!");
        //
        //     if (FirstObjIdx == 0) // no objects in tile.
        //         return;
        //     
        //     List<GameObject> lst = new List<GameObject>();
        //     
        //     // TODO: HIGHLY INEFFICIENT! TRY TO GET A DICT OR SOME TYPE OF MANAGER!
        //     bool finished = false;
        //     int safetycounter = 0;
        //     int maxcounter = 500;
        //     int idToSearch = FirstObjIdx;
        //     while (true) 
        //     {
        //         for (int i = 0; i < gameObjects.Length; i++)
        //         {
        //             GameObject currobj = gameObjects[i];
        //             safetycounter++;
        //             
        //             // TODO: Is Idx == ItemID?
        //             // Not the object I'm looking for
        //             if (currobj.ItemID != idToSearch) 
        //                 continue;
        //             
        //             // ItemID == 0, end of list
        //             if (currobj.IsEndOfList) 
        //                 finished = true;
        //
        //             idToSearch = currobj.next;
        //             lst.Add(currobj);
        //             
        //             // To prevent infinite loops in case of a cyclic ID reference.
        //             if (safetycounter >= maxcounter)
        //             {
        //                 Console.WriteLine("WARNING! Encountered infinite loop when populating StaticObjectList!");
        //                 finished = true;
        //                 break;
        //             }
        //         } 
        //         
        //         if (finished)
        //             break;
        //     }
        //
        //     StaticObjects = lst.ToArray();
        // }

        public int[]? XYPos
        {
            get
            {
                int row = EntryNum % TileMapMasterObjectListBlock.TileHeight;
                int col = EntryNum / TileMapMasterObjectListBlock.TileWidth;
                return new int[] {row, col};
            }
        }

        public bool CheckValidityOfObjects()
        {
            bool ends = false;
            foreach (MobileObject mobileObject in MobileObjects)
            {
                if (mobileObject.IsEndOfList)
                    ends = true;
            }

            if (!ends)
                return false;

            if (MobileObjects[0].ItemID != FirstObjIdx)
                return false;
            
            return true;
        }

        public void UpdateBuffer()
        {
            TileBuffer = BitConverter.GetBytes(Entry);
            Debug.Assert(TileBuffer.Length == Size);
        }

        public void UpdateEntry()
        {
            Debug.Assert(TileBuffer.Length == Size);
            Entry = BitConverter.ToInt32(TileBuffer);
        }


        public TileInfo(int entrynum, int entry, int offset, int levelNumber)
        {
            EntryNum = entrynum;
            Entry = entry;
            LevelNum = levelNumber;
            UpdateBuffer();
            Offset = offset;
        }

        public TileInfo(int entrynum, byte[] buffer, int offset, int levelNumber)
        {
            EntryNum = entrynum;
            LevelNum = levelNumber;
            TileBuffer = buffer;
            UpdateEntry();
            Offset = offset;
        }

        // TODO: Check if we need that modification in the height value mentioned in the uw-formats.txt
        public void MoveObjectsToSameZLevel()
        {
            foreach (GameObject obj in MobileObjects)
            {
                obj.Zpos = (byte) TileHeight;
            }

            foreach (GameObject obj in StaticObjects)
            {
                obj.Zpos = (byte) TileHeight;
            }
        }

        public string? SaveBuffer(string basePath = "D:\\Dropbox\\UnderworldStudy\\studies\\LEV.ARK",
            string extraInfo = "")
        {
            if (extraInfo.Length == 0)
            {
                extraInfo = $@"_TILE_{LevelNum}_{XYPos}_{TileTypeDescriptors[TileType]}";
            }

            return StdSaveBuffer(TileBuffer, basePath, extraInfo);

        }
        
    }
}