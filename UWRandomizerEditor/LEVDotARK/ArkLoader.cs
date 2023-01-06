using UWRandomizerEditor.Interfaces;
using UWRandomizerEditor.LEVDotARK.Blocks;

namespace UWRandomizerEditor.LEVDotARK
{
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
    public class ArkLoader : IBufferObject
    {
        public const int NumOfLevels = 9; // In UW1

        public byte[] Buffer { get; set; }
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

        IDictionary<Sections, int> BlockLengths = new Dictionary<Sections, int>()
        {
            {Sections.LevelTilemapObjlist, TileMapMasterObjectListBlock.FixedBlockLength},
            {Sections.ObjectAnimOverlayInfo, ObjectAnimationOverlayInfoBlock.FixedBlockLength},
            {Sections.TextureMappings, TextureMappingBlock.FixedBlockLength},
            {Sections.AutomapInfos, 0},
            {Sections.MapNotes, 0},
            {Sections.Unused, 0}
        };

        private ArkLoader()
        {
        }

        /// <summary>
        /// Instantiates a new ArkLoader object using a provided path.
        /// </summary>
        /// <param name="path"></param>
        public ArkLoader(string path)
        {
            Path = path;
            Buffer = LoadArkfile(path);
            int headerSize = Header.blockNumSize + Header.blockOffsetSize * Header.NumEntriesFromBuffer(Buffer);
            header = new Header(Buffer[0..headerSize]);

            blocks = new Block[header.NumEntries];
            TileMapObjectsBlocks = new TileMapMasterObjectListBlock[NumOfLevels];
            ObjAnimBlocks = new ObjectAnimationOverlayInfoBlock[NumOfLevels];
            TextMapBlocks = new TextureMappingBlock[NumOfLevels];
            AutomapBlocks = new AutomapInfosBlock[NumOfLevels];
            MapNotesBlocks = new MapNotesBlock[NumOfLevels];

            LoadBlocks();
        }

        public void LoadBlocks()
        {
            int currblocktypecount = 0;
            int currblocktype = 0;

            // Loop through all entries specified in the header
            for (short blocknum = 0; blocknum < header.NumEntries; blocknum++)
            {
                if (currblocktype >= 6)
                {
                    currblocktype = 5; // Hack
                }

                Block block = GetBlock(blocknum, (Sections) currblocktype, currblocktypecount);
                // Let's store the blocks this general array
                blocks[blocknum] = block;

                // And also in its specific array, depending on its type
                if (block is TileMapMasterObjectListBlock tilemap)
                {
                    TileMapObjectsBlocks[currblocktypecount] = tilemap;
                }
                else if (block is ObjectAnimationOverlayInfoBlock obj)
                {
                    ObjAnimBlocks[currblocktypecount] = obj;
                }
                else if (block is TextureMappingBlock text)
                {
                    TextMapBlocks[currblocktypecount] = text;
                }
                else if (block is MapNotesBlock map)
                {
                    MapNotesBlocks[currblocktypecount] = map;
                }
                else if (block is AutomapInfosBlock automap)
                {
                    AutomapBlocks[currblocktypecount] = automap;
                }

                currblocktypecount++;
                // Resets currblocktypecount when going from one block type to another
                if (currblocktypecount >= NumOfLevels) // TODO: looks like I could do a % here. Think more
                {
                    currblocktypecount = 0;
                    currblocktype++;
                }
            }
        }

        public bool ReconstructBuffer()
        {
            List<byte> templist = new List<byte>();
            header.ReconstructBuffer();
            templist.AddRange(header.Buffer);
            foreach (var block in blocks)
            {
                block.ReconstructBuffer();
                templist.AddRange(block.Buffer);
            }

            byte[] concatbuffers = templist.ToArray();
            concatbuffers.CopyTo(Buffer, 0);
            return true;
        }


        public byte[] GetBlockBuffer(short blockNum, int BlockLength)
        {
            if (blockNum > header.NumEntries)
            {
                throw new InvalidDataException($"Invalid block number ({blockNum})Block number is greater than total" +
                                               $" number of blocks {header.NumEntries}");
            }

            //int blockOffset = BitConverter.ToInt32(Buffer[(2 + 4 * blockNum)..(6 + 4 * blockNum)]);
            int blockOffset = header.BlockOffsets[blockNum];

            if (blockOffset == 0)
            {
                return Array.Empty<byte>(); // TODO: figure out why this is better than new byte[] {}
            }

            byte[] blockBuffer = Buffer[blockOffset..(blockOffset + BlockLength)];
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
            int BlockLength;
            if ((BlockType == Sections.MapNotes) | (BlockType == Sections.AutomapInfos))
            {
                // TODO: When I'm less tired, check if this will go back in the last block.
                BlockLength = header.BlockOffsets[BlockNum + 1] - header.BlockOffsets[BlockNum];
            }
            else
            {
                BlockLength = BlockLengths[BlockType];
            }

            byte[] buffer = GetBlockBuffer(BlockNum, BlockLength);
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

        static byte[] LoadArkfile(string? path = null)
        {
            if (path is null)
            {
                throw new FileNotFoundException();
            }

            return File.ReadAllBytes(path);
        }
    }
}