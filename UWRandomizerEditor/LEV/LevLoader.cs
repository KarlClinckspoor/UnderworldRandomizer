using System.Diagnostics;
using UWRandomizerEditor.Interfaces;
using UWRandomizerEditor.LEVdotARK.Blocks;
using System.Linq;
using System.Diagnostics;

namespace UWRandomizerEditor.LEVdotARK;

/// <summary>
/// This class loads an lev.ark file and subdivides it into 1) a header and 2) a sequence of blocks.
///
/// The block types are:
///
/// * Tile Map and Master Object List block
/// * Object animation Overlay Info block
/// * Texture Mapping block
/// * Automap Info block
/// * Map Notes block
/// * Empty blocks
///
/// </summary>
public class LevLoader : IBufferObject
{
    public const int NumOfLevels = 9; // In UW1

    private byte[] _buffer;
    public byte[] Buffer
    {
        get
        {
            // Reconstruct the header by the sizes of the blocks, which *can* be in whatever offsets, from recent studies.
            var newHeaderBuffer = new List<byte>();
            var offsetList = new List<int>();
            int previousOffset = header.Buffer.Length;

            ushort numEntries = (ushort) blocks.Length;
            byte numEntriesLB = (byte)(numEntries & 0xFF);
            byte numEntriesUB = (byte)((numEntries >> 8) & 0xFF);
            newHeaderBuffer.Add(numEntriesLB);
            newHeaderBuffer.Add(numEntriesUB);

            // Loop through blocks that have some content
            List<int> validBlockLengths = new();
            foreach (var block in blocks)
            {
                if (block.Buffer.Length == 0) continue;
                validBlockLengths.Add(block.Buffer.Length);
            }
            // Cumulative sum of the offsets
            List<int> validBlockOffsets = new();
            int acc = header.Buffer.Length;
            foreach (var length in validBlockLengths)
            {
                validBlockOffsets.Add(acc);
                acc += length;
            }
            // Loop through to set the offsets when the block has something
            int currValidBlockItem = 0;
            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i].Buffer.Length == 0)
                {
                    newHeaderBuffer.Add(0);
                    newHeaderBuffer.Add(0);
                    newHeaderBuffer.Add(0);
                    newHeaderBuffer.Add(0);
                    continue;
                }
                var offset = validBlockOffsets[currValidBlockItem];
                currValidBlockItem++;
                newHeaderBuffer.Add((byte) ((offset >> (8 * 0)) & 0xFF));
                newHeaderBuffer.Add((byte) ((offset >> (8 * 1)) & 0xFF));
                newHeaderBuffer.Add((byte) ((offset >> (8 * 2)) & 0xFF));
                newHeaderBuffer.Add((byte) ((offset >> (8 * 3)) & 0xFF));
            }
            // This still isn't passing the Difficult lev.ark test. I'll have to think this through better.

            var headerBuffer = newHeaderBuffer.ToArray();
            header.Buffer = headerBuffer;

            // Reconstruct the buffer itself by copying the buffers of the blocks.
            var tempBufferList = new List<byte>();
            tempBufferList.AddRange(header.Buffer);
            foreach (var (offset, block) in System.Linq.Enumerable.Zip<int, Block>(offsetList, blocks))
            {
                //Debug.Assert(tempBufferList.Count == offset);
                tempBufferList.AddRange(block.Buffer);
            }

            var concatbuffers = tempBufferList.ToArray();
            concatbuffers.CopyTo(_buffer, 0);
            return _buffer;
        }
        set
        {
            _buffer = value;
            LoadBlocks();
        }
    }

    public string Path;

    public Header header;
    public Block[] blocks;
    public TileMapMasterObjectListBlock[] TileMapObjectsBlocks;
    public ObjectAnimationOverlayInfoBlock[] ObjAnimBlocks;
    public TextureMappingBlock[] TextMapBlocks;
    public AutomapInfosBlock[] AutomapBlocks;
    public MapNotesBlock[] MapNotesBlocks;

    public enum Sections
    {
        LevelTilemapObjlist = 0,
        ObjectAnimOverlayInfo = 1,
        TextureMappings = 2,
        AutomapInfos = 3,
        MapNotes = 4,
        Unused = 5,
    }

    IDictionary<Sections, int> MinimumBlockLengths = new Dictionary<Sections, int>()
    {
        {Sections.LevelTilemapObjlist, TileMapMasterObjectListBlock.FixedBlockLength},
        {Sections.ObjectAnimOverlayInfo, ObjectAnimationOverlayInfoBlock.FixedBlockLength},
        {Sections.TextureMappings, TextureMappingBlock.FixedBlockLength},
        {Sections.AutomapInfos, 0},
        {Sections.MapNotes, 0},
        {Sections.Unused, 0}
    };

    /// <summary>
    /// Instantiates a new ArkLoader object using a provided path.
    /// </summary>
    /// <param name="path"></param>
    public LevLoader(string path)
    {
        Path = path;
        if (!File.Exists(path))
            throw new FileNotFoundException();
        _buffer = File.ReadAllBytes(path); 
        
        var headerSize = Header.blockNumSize + Header.blockOffsetSize * Header.NumEntriesFromBuffer(_buffer);
        header = new Header(_buffer[0..headerSize], _buffer.Length);

        blocks = new Block[header.NumEntries];
        TileMapObjectsBlocks = new TileMapMasterObjectListBlock[NumOfLevels];
        ObjAnimBlocks = new ObjectAnimationOverlayInfoBlock[NumOfLevels];
        TextMapBlocks = new TextureMappingBlock[NumOfLevels];
        AutomapBlocks = new AutomapInfosBlock[NumOfLevels];
        MapNotesBlocks = new MapNotesBlock[NumOfLevels];

        LoadBlocks();
    }

    /// <summary>
    /// Creates a ArkLoader instance using an already opened buffer.
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="path"></param>
    public LevLoader(byte[] buffer, string path)
    {
        Path = path;
        _buffer = buffer;
        
        var headerSize = Header.blockNumSize + Header.blockOffsetSize * Header.NumEntriesFromBuffer(_buffer);
        header = new Header(_buffer[0..headerSize], _buffer.Length);
        blocks = new Block[header.NumEntries];
        TileMapObjectsBlocks = new TileMapMasterObjectListBlock[NumOfLevels];
        ObjAnimBlocks = new ObjectAnimationOverlayInfoBlock[NumOfLevels];
        TextMapBlocks = new TextureMappingBlock[NumOfLevels];
        AutomapBlocks = new AutomapInfosBlock[NumOfLevels];
        MapNotesBlocks = new MapNotesBlock[NumOfLevels];

        LoadBlocks();
    }

    private void LoadBlocks()
    {
        var currBlockTypeCount = 0;
        var currBlockType = 0;

        // Loop through all entries specified in the header
        for (short blockNum = 0; blockNum < header.NumEntries; blockNum++)
        {
            if (currBlockType >= 6)
            {
                currBlockType = 5; // Hack
            }

            var block = GetBlock(blockNum, (Sections) currBlockType, currBlockTypeCount);
            // Let's store the blocks this general array
            blocks[blockNum] = block;

            switch (block)
            {
                case TileMapMasterObjectListBlock tilemap:
                    TileMapObjectsBlocks[currBlockTypeCount] = tilemap;
                    break;
                case ObjectAnimationOverlayInfoBlock obj:
                    ObjAnimBlocks[currBlockTypeCount] = obj;
                    break;
                case TextureMappingBlock text:
                    TextMapBlocks[currBlockTypeCount] = text;
                    break;
                case MapNotesBlock map:
                    MapNotesBlocks[currBlockTypeCount] = map;
                    break;
                case AutomapInfosBlock automap:
                    AutomapBlocks[currBlockTypeCount] = automap;
                    break;
            }

            currBlockTypeCount++;
            // Resets currBlockTypeCount when going from one block type to another
            if (currBlockTypeCount >= NumOfLevels)
            {
                currBlockTypeCount = 0;
                currBlockType++;
            }
        }
    }

    public bool ReconstructBuffer()
    {
        return true;
    }


    public byte[] GetBlockBuffer(short blockNum, int BlockLength)
    {
        if (blockNum > header.NumEntries)
        {
            throw new InvalidDataException($"Invalid block number ({blockNum})Block number is greater than total" +
                                           $" number of blocks {header.NumEntries}");
        }

        int blockOffset = header.BlockOffsets[blockNum];
        int otherBlockOffset = header.GetOffsetForBlock(blockNum);

        if (blockOffset == 0)
        {
            return Array.Empty<byte>(); // TODO: figure out why this is better than new byte[] {}
        }

        var blockBuffer = _buffer[blockOffset..(blockOffset + BlockLength)];
        return blockBuffer;
    }

    /// <summary>
    /// Function that returns a Block object of a class depending on the position of the block in LEV.ARK. 
    /// </summary>
    /// <param name="BlockNum"></param>
    /// <param name="BlockType"></param>
    /// <param name="levelnumber"></param>
    /// <returns></returns>
    public Block GetBlock(short BlockNum, Sections BlockType, int levelnumber = -1)
    {
        // When the offset is zero, it's an empty block
        if (header.BlockOffsets[BlockNum] == 0)
        {
            // TODO: When I'm less tired, check if this will go back in the last block.
            //var BlockLength1 = header.BlockOffsets[BlockNum + 1] - header.BlockOffsets[BlockNum];
            BlockLength = header.GetBlockSize(BlockNum);
            //if (BlockLength1 != BlockLength2)
            //{
            //    throw new Exception();
            //}

        }
        else
        {
            BlockLength = BlockLengths[BlockType];
        }

        var buffer = GetBlockBuffer(BlockNum, blockLength);
        switch (BlockType)
        {
            case Sections.LevelTilemapObjlist:
                return new TileMapMasterObjectListBlock(buffer, levelnumber);
            case Sections.ObjectAnimOverlayInfo:
                return new ObjectAnimationOverlayInfoBlock(buffer, levelnumber);
            case Sections.TextureMappings:
                return new TextureMappingBlock(buffer, levelnumber);
            case Sections.AutomapInfos:
                return new AutomapInfosBlock(buffer, levelnumber);
            case Sections.MapNotes:
                return new MapNotesBlock(buffer, levelnumber);
            case Sections.Unused:
                // Debug.Assert(buffer.Length == 0); // Got the buffer, but should be empty.
                return new EmptyBlock();
            default:
                return new EmptyBlock();
        }
    }
}