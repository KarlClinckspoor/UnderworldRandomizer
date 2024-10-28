using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using UWRandomizerEditor.Interfaces;
using UWRandomizerEditor.LEV;
using UWRandomizerEditor.LEV.Blocks;
using UWRandomizerEditor.LEV.GameObjects;

namespace UWRandomizerEditor.LEV;

public class Tile : IBufferObject, IEquatable<Tile>
{
    public const int FixedSize = 4;

    public enum TileTypes: uint
    {
        Solid = 0,
        Open,
        DiagSe,
        DiagSw,
        DiagNe,
        DiagNw,
        SlpN,
        SlpS,
        SlpE,
        SlpW,
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

    // This is the 4 bytes used to describe a Tile Info. This is useful for bitwise operations that span
    // the boundary of 1 byte, which would be a bit cumbersome to do with a byte[] buffer. The byte[] Buffer
    // is to be considered "primary", meaning it should always be kept up to date.
    private uint BufferAsUInt32
    {
        get
        {
            return BitConverter.ToUInt32(Buffer);
        }
        [MemberNotNull("Buffer")]
        set
        {
            Buffer = BitConverter.GetBytes(value);
        }
    }

    public byte[] Buffer { get; set; }

    public uint EntryNum { get; set; }

    public int Offset => (int) EntryNum * FixedSize;

    public int LevelNum { get; set; }

    public uint TileType
    {
        get => Utils.GetBits(BufferAsUInt32, 0b1111, 0);
        set => BufferAsUInt32 = Utils.SetBits(BufferAsUInt32, value, 0b1111, 0);
    }

    public uint TileHeight
    {
        get => Utils.GetBits(BufferAsUInt32, 0b1111, 4);
        set => BufferAsUInt32 = Utils.SetBits(BufferAsUInt32, value, 0b1111, 4);
    }

    public uint Light
    {
        get { return Utils.GetBits(BufferAsUInt32, 0b1, 8); }
        set
        {
            BufferAsUInt32 = Utils.SetBits(BufferAsUInt32, value, 0b1, 8);
        }
    }

    // todo: recheck this.
    public uint Bit9
    {
        get { return Utils.GetBits(BufferAsUInt32, 0b1, 9); }
        set
        {
            BufferAsUInt32 = Utils.SetBits(BufferAsUInt32, value, 0b1, 9);
        }
    }

    public uint FloorTextureIdx
    {
        get { return Utils.GetBits(BufferAsUInt32, 0b1111, 10); }
        set
        {
            BufferAsUInt32 = Utils.SetBits(BufferAsUInt32, value, 0b1111, 10);
        }
    }

    public uint NoMagic
    {
        get { return Utils.GetBits(BufferAsUInt32, 0b1, 14); }
        set
        {
            BufferAsUInt32 = Utils.SetBits(BufferAsUInt32, value, 0b1, 14);
        }
    }

    public uint DoorBit
    {
        get { return Utils.GetBits(BufferAsUInt32, 0b1, 15); }
        set
        {
            BufferAsUInt32 = Utils.SetBits(BufferAsUInt32, value, 0b1, 15);
        }
    }

    public uint WallTextureIdx
    {
        get { return Utils.GetBits(BufferAsUInt32, 0b111111, 16); }
        set
        {
            BufferAsUInt32 = Utils.SetBits(BufferAsUInt32, value, 0b111111, 16);
        }
    }

    public uint FirstObjIdx
    {
        get
        {
            return ObjectChain.StartingIdx;
        }
        set
        {
            BufferAsUInt32 = Utils.SetBits(BufferAsUInt32, value, 0b1111111111, 22);
            ObjectChain.StartingIdx = value;
            if (GetStartingIndex(Buffer) != value)
                throw new Exception("Why isn't the starting index being added correctly?");
        }
    }

    private static uint GetStartingIndex(uint bufferAsUInt32)
    {
        return Utils.GetBits(bufferAsUInt32, 0b1111111111, 22);
    }

    private static uint GetStartingIndex(byte[] buffer)
    {
        uint temp = BitConverter.ToUInt32(buffer);
        return GetStartingIndex(temp);
    }

    public UWLinkedList ObjectChain { get; }


    // TODO: Review this
    public Point XYPos
    {
        get
        {
            var row = (int) EntryNum % TileMapMasterObjectListBlock.TileHeight;
            var col = (int) EntryNum / TileMapMasterObjectListBlock.TileWidth;
            return new Point(row, col);
        }
    }

    public int XPos => XYPos.x;
    public int YPos => XYPos.y;

    // TODO: I could add more checks here
    public bool ReconstructBuffer()
    {
        if (GetStartingIndex(Buffer) != ObjectChain.StartingIdx)
        {
            Debug.Print("Mismatch between Tile firstObjectIndex in buffer and UWLinkedList.");
            FirstObjIdx = ObjectChain.StartingIdx;
        }
        return true;
    }

    public Tile(uint entrynum, uint bufferAsUInt32, uint offset, int levelNumber)
    {
        EntryNum = entrynum;
        BufferAsUInt32 = bufferAsUInt32;
        LevelNum = levelNumber;
        ObjectChain = new UWLinkedList() {StartingIdx = GetStartingIndex(bufferAsUInt32), RepresentingContainer = false};
        if (offset != Offset)
        {
            throw new Exception("Invalid calculation of offset from EntryNum!");
        }
    }

    public Tile(uint entrynum, byte[] buffer, uint offset, int levelNumber)
    {
        if (buffer.Length != FixedSize)
        {
            throw new ArgumentException($"Invalid size of Tile Info Buffer: {buffer.Length}");
        }

        EntryNum = entrynum;
        LevelNum = levelNumber;
        Buffer = buffer;
        ObjectChain = new UWLinkedList() {StartingIdx = GetStartingIndex(buffer), RepresentingContainer = false};
        if (offset != Offset)
        {
            throw new Exception("Invalid calculation of offset from EntryNum!");
        }
    }

    // TODO: Check if we need that modification in the height value mentioned in the uw-formats.txt
    public void MoveObjectsToSameZLevel()
    {
        foreach (GameObject obj in ObjectChain)
        {
            obj.Zpos = (byte) (TileHeight * 8);
        }
    }

    // TODO: Make the positions randomized among a set of possible values
    public void MoveObjectsToCorrectCorner(Random r)
    {
        foreach (var obj in ObjectChain)
        {
            switch (TileType)
            {
                case (int) TileTypes.Open:
                case (int) TileTypes.SlpN:
                case (int) TileTypes.SlpE:
                case (int) TileTypes.SlpS:
                case (int) TileTypes.SlpW:
                case (int) TileTypes.Solid:
                    break;
                case (int) TileTypes.DiagSe:
                {
                    obj.Xpos = 6;
                    obj.Ypos = 1;
                    break;
                }
                case (int) TileTypes.DiagSw:
                {
                    obj.Xpos = 1;
                    obj.Ypos = 1;
                    break;
                }
                case (int) TileTypes.DiagNe:
                {
                    obj.Xpos = 6;
                    obj.Ypos = 6;
                    break;
                }
                case (int) TileTypes.DiagNw:
                {
                    obj.Xpos = 1;
                    obj.Ypos = 6;
                    break;
                }
            }
        }
    }

    public bool Equals(Tile? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        // I think I shouldn't have to consider the object chain, only the first index and the level num
        // I've added the other flags for completeness, because comparing only the "BufferAsUInt32" should be enough to cover them
        if (
            this.BufferAsUInt32 == other.BufferAsUInt32 &
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
        return Equals((Tile) obj);
    }
}