using System.Diagnostics.CodeAnalysis;
using UWRandomizerEditor.Interfaces;
using UWRandomizerEditor.LEVDotARK.Blocks;
using UWRandomizerEditor.LEVDotARK.GameObjects;
using static UWRandomizerEditor.Utils;

namespace UWRandomizerEditor.LEVDotARK
{
    public class TileInfo : IBufferObject, IEquatable<TileInfo>
    {
        public const int FixedSize = 4;

        public enum TileTypes
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
        };

        public static readonly IDictionary<int, string> TileTypeDescriptors = new Dictionary<int, string>()
        {
            {0, "#SOLID"},
            {1, "#OPEN"},
            {2, "#DIAG_SE"},
            {3, "#DIAG_SW"},
            {4, "#DIAG_NE"},
            {5, "#DIAG_NW"},
            {6, "#SLP_N"},
            {7, "#SLP_S"},
            {8, "#SLP_E"},
            {9, "#SLP_W"}
        };

        public static readonly IDictionary<int, char> TileCharReplacements = new Dictionary<int, char>()
        {
            {0, '#'},
            {1, '_'},
            {2, '/'},
            {3, '\\'},
            {4, '\\'},
            {5, '/'},
            {6, '^'},
            {7, 'v'},
            {8, '>'},
            {9, '<'}
        };

        // Defined in the constructor
        public int Entry { get; set; }

        public byte[] Buffer { get; set; }

        public int EntryNum { get; set; }

        // TODO: Test this
        public int Offset
        {
            get { return EntryNum * FixedSize; }
        }

        public int LevelNum { get; set; }

        public int TileType
        {
            get { return GetBits(Entry, 0b1111, 0); }
            set { Entry = SetBits(Entry, value, 0b1111, 0); }
        }

        public int TileHeight
        {
            get { return GetBits(Entry, 0b1111, 4); }
            set
            {
                Entry = SetBits(Entry, value, 0b1111, 4);
                ReconstructBuffer();
            }
        }

        public int Light
        {
            get { return GetBits(Entry, 0b1, 8); }
            set
            {
                Entry = SetBits(Entry, value, 0b1, 8);
                ReconstructBuffer();
            }
        }

        // todo: recheck this.
        public int Bit9
        {
            get { return GetBits(Entry, 0b1, 9); }
            set
            {
                Entry = SetBits(Entry, value, 0b1, 9);
                ReconstructBuffer();
            }
        }

        public int FloorTextureIdx
        {
            get { return GetBits(Entry, 0b1111, 10); }
            set
            {
                Entry = SetBits(Entry, value, 0b1111, 10);
                ReconstructBuffer();
            }
        }

        public int NoMagic
        {
            get { return GetBits(Entry, 0b1, 14); }
            set
            {
                Entry = SetBits(Entry, value, 0b1, 14);
                ReconstructBuffer();
            }
        }

        public int DoorBit
        {
            get { return GetBits(Entry, 0b1, 15); }
            set
            {
                Entry = SetBits(Entry, value, 0b1, 15);
                ReconstructBuffer();
            }
        }

        public int WallTextureIdx
        {
            get { return GetBits(Entry, 0b111111, 16); }
            set
            {
                Entry = SetBits(Entry, value, 0b111111, 16);
                ReconstructBuffer();
            }
        }

        public int FirstObjIdx
        {
            get { return ObjectChain.startingIdx; }
            set
            {
                Entry = SetBits(Entry, value, 0b1111111111, 22);
                ObjectChain.startingIdx = value;
                ReconstructBuffer();
            }
        }

        public UWLinkedList ObjectChain { get; }


        public int[] XYPos
        {
            get
            {
                int row = EntryNum % TileMapMasterObjectListBlock.TileHeight;
                int col = EntryNum / TileMapMasterObjectListBlock.TileWidth;
                return new int[] {row, col};
            }
        }

        [MemberNotNull(nameof(Buffer))]
        public bool ReconstructBuffer() // Modified entry, updates buffer
        {
            // Sometimes the entry is stale regarding the starting index of the object chain, since that's what tracks this property.
            Entry = SetBits(Entry, ObjectChain.startingIdx, 0b1111111111, 22);
            Buffer = BitConverter.GetBytes(Entry);
            return true;
        }

        private void UpdateEntry() // Modified buffer, updates entry
        {
            Entry = BitConverter.ToInt32(Buffer);
        }


        public TileInfo(int entrynum, int entry, int offset, int levelNumber)
        {
            EntryNum = entrynum;
            Entry = entry;
            LevelNum = levelNumber;
            ObjectChain = new UWLinkedList();
            ReconstructBuffer();
        }

        public TileInfo(int entrynum, byte[] buffer, int offset, int levelNumber)
        {
            EntryNum = entrynum;
            LevelNum = levelNumber;
            Buffer = buffer;
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

        // TODO: Keeping this because of the filename string interp. Might use this somewhere else
        //         filename = $@"_TILE_{LevelNum}_{XYPos}_{TileTypeDescriptors[TileType]}";

        public bool Equals(TileInfo? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            // I think I shouldn't have to consider the object chain, only the first index and the level num
            // I've added the other flags for completeness, because comparing only the "Entry" should be enough to cover them
            if (
                this.Entry == other.Entry &
                this.Buffer.SequenceEqual(other.Buffer) &
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
            return HashCode.Combine(Entry, Buffer, EntryNum, LevelNum, ObjectChain);
        }
    }
}