using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using Randomizer.Interfaces;
using Randomizer.LEVDotARK.Blocks;
using Randomizer.LEVDotARK.GameObjects;
using static Randomizer.Utils;

namespace Randomizer.LEVDotARK
{
    
    public class TileInfo: ISaveBinary, IEquatable<TileInfo>
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
        private int _entry;
        public int Entry
        {
            get { UpdateEntry(); return _entry; }
            set { _entry = value; UpdateBuffer(); }
        }

        private byte[] _tileBuffer;
        public byte[] TileBuffer
        {
            get
            {
                UpdateBuffer(); // Let's assure the buffer is always updated
                return _tileBuffer;
            }
            private set
            {
                _tileBuffer = value;
            }
        }
        public int EntryNum;

        // TODO: Test this
        public int Offset
        {
            get
            {
                return EntryNum * Size;
            }  
        } 
        public int LevelNum;

        public int TileType
        {
            get { return GetBits(Entry, 0b1111, 0); }
            set { Entry = SetBits(Entry, value, 0b1111, 0);}
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
            get
            {
                return ObjectChain.startingIdx;
            }
            set
            {
                Entry = SetBits(Entry, value, 0b1111111111, 22);
                ObjectChain.startingIdx = value;
                UpdateBuffer();
            }
        }

        public UWLinkedList ObjectChain; // Store 
        

        public int[]? XYPos
        {
            get
            {
                int row = EntryNum % TileMapMasterObjectListBlock.TileHeight;
                int col = EntryNum / TileMapMasterObjectListBlock.TileWidth;
                return new int[] {row, col};
            }
        }

        [MemberNotNull(nameof(_tileBuffer))]
        private void UpdateBuffer() // Modified entry, updates buffer
        {
            _entry = SetBits(_entry, ObjectChain.startingIdx, 0b1111111111, 22);
            _tileBuffer = BitConverter.GetBytes(_entry);
        }

        private void UpdateEntry() // Modified buffer, updates entry
        {
            _entry = BitConverter.ToInt32(_tileBuffer);
        }


        public TileInfo(int entrynum, int entry, int offset, int levelNumber)
        {
            EntryNum = entrynum;
            _entry = entry;
            LevelNum = levelNumber;
            ObjectChain = new UWLinkedList();
            UpdateBuffer();
        }

        public TileInfo(int entrynum, byte[] buffer, int offset, int levelNumber)
        {
            EntryNum = entrynum;
            LevelNum = levelNumber;
            _tileBuffer = buffer;
            ObjectChain = new UWLinkedList();
            UpdateEntry();
        }

        // TODO: Check if we need that modification in the height value mentioned in the uw-formats.txt
        public void MoveObjectsToSameZLevel()
        {
            foreach (GameObject obj in ObjectChain)
            {
                obj.Zpos = (byte) TileHeight;
            }
        }

        // TODO: Make the positions randomized among a set of possible values
        public void MoveObjectsToCorrectCorner()
        {
            Random r = new Random(); // TODO: Make a singleton random instance
            foreach (var obj in ObjectChain)
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
                            obj.Xpos = 6;
                            obj.Ypos = 1;
                            break;
                        }
                    case (int) TileTypes.diag_sw:
                        {
                            obj.Xpos = 1;
                            obj.Ypos = 1;
                            break;
                        }
                    case (int) TileTypes.diag_ne:
                        {
                            obj.Xpos = 6;
                            obj.Ypos = 6;
                            break;
                        }
                    case (int) TileTypes.diag_nw:
                        {
                            obj.Xpos = 1;
                            obj.Ypos = 6;
                            break;
                        }
                }
            }
        }

        public string SaveBuffer(string? basePath, string? filename)
        {
            basePath ??= Settings.DefaultBinaryTestsPath;
            filename ??= string.Empty;
            if (filename.Length == 0)
            {
                filename = $@"_TILE_{LevelNum}_{XYPos}_{TileTypeDescriptors[TileType]}";
            }

            return StdSaveBuffer(TileBuffer, basePath, filename);

        }

        public bool Equals(TileInfo? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            // I think I shouldn't have to consider the object chain, only the first index and the level num
            // I've added the other flags for completeness, because comparing only the "Entry" should be enough to cover them
            if (
                this.Entry == other.Entry &
                this.TileBuffer.SequenceEqual(other.TileBuffer) &
                this.EntryNum == other.EntryNum &
                this.LevelNum == other.LevelNum &
                this.TileType == other.TileType &
                this.TileHeight == other.TileHeight &
                this.Light == other.Light &
                this.FloorTextureIdx == other.FloorTextureIdx &
                this.NoMagic == other.NoMagic &
                this.DoorBit == other.DoorBit &
                this.WallTextureIdx == other.WallTextureIdx
                )
            {
                return true;
            }

            return false;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TileInfo) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_entry, _tileBuffer, EntryNum, LevelNum, ObjectChain);
        }
    }
}