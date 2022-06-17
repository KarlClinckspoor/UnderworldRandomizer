using System.Diagnostics;
using static Randomizer.Utils;

namespace Randomizer
{
    
    public class TileInfo: ISaveBinary
    {
        public const int Size = 4;
        private enum TileTypes
        {
            solid = 0,
            open,
            diag_se,
            diag_sw,
            diag_ne,
            diag_nw,
            slp_n,
            slp_s,
            slp_e,
            slp_w,
        } ;

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
        public GameObject[] AllObjects = new GameObject[0];
        public MobileObject[] MobileObjects = new MobileObject[0];
        public GameObject[] StaticObjects = new GameObject[0];

        public bool HasTexturedObject
        {
            get { 
                foreach(var obj in StaticObjects)
                {
                    if (obj.GetType() == typeof(TexturedGameObject))
                    {
                        return true;
                    }

                }
                return false;
            }
        }


        /// <summary>
        /// Fills in the list of objects in this Tile given a list of all GameObjects present in that level.
        /// </summary>
        /// <param name="AllBlockObjects">All game objects in the level, both static and mobile</param>
        public void PopulateObjectList(GameObject[] AllBlockObjects)
        {
            if (FirstObjIdx == 0)
            {
                return;
            }
            List<GameObject> allObjects = new List<GameObject>();
            List<MobileObject> mobileObjects = new List<MobileObject>();
            List<GameObject> staticObjects = new List<GameObject>();

            int safetycounter = 0;
            int maxcounter = 1024;
            int currentIdx = FirstObjIdx;
            while (currentIdx != 0) 
            {
                safetycounter++;
                if (safetycounter >= maxcounter)
                {
                    Console.WriteLine("WARNING! Encountered potentially infinite loop when populating ObjectList!");
                    break;
                }

                GameObject obj = AllBlockObjects[currentIdx];
                allObjects.Add(obj);

                if (obj.GetType() == typeof(MobileObject))
                {
                    mobileObjects.Add((MobileObject) obj);                   
                }
                else
                {
                    staticObjects.Add(obj);
                }
                currentIdx = obj.next;
            }
            MobileObjects = mobileObjects.ToArray();
            StaticObjects = staticObjects.ToArray();
            AllObjects = allObjects.ToArray();

        }

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
            foreach (GameObject obj in AllObjects)
            {
                obj.Zpos = (byte) TileHeight;
            }
        }

        // TODO: Make the positions randomized among a set of possible values
        public void MoveObjectsToCorrectCorner()
        {
            Random r = new Random();
            foreach (GameObject obj in AllObjects)
            {
                switch (TileType)
                {
                    case (int) TileTypes.open:
                    case (int) TileTypes.slp_n:
                    case (int) TileTypes.slp_e:
                    case (int) TileTypes.slp_s:
                    case (int) TileTypes.slp_w:
                    case (int) TileTypes.solid:
                        break;
                    case (int) TileTypes.diag_se:
                        {
                            //int newXpos = r.Next(5) + 1;
                            //int newYpos = r.Next(newXpos);
                            //obj.Xpos = (byte)newXpos;
                            //obj.Ypos = (byte)newYpos;
                            //break;
                            obj.Xpos = (byte)6;
                            obj.Ypos = (byte)1;
                            break;
                        }
                    case (int) TileTypes.diag_sw:
                        {
                            obj.Xpos = (byte)1;
                            obj.Ypos = (byte)1;
                            break;
                        }
                    case (int) TileTypes.diag_ne:
                        {
                            obj.Xpos = (byte)6;
                            obj.Ypos = (byte)6;
                            break;
                        }
                    case (int) TileTypes.diag_nw:
                        {
                            obj.Xpos = (byte)1;
                            obj.Ypos = (byte)6;
                            break;
                        }
                }
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