using System.Diagnostics;
using System.Security.Cryptography;
using Randomizer.Interfaces;
using Randomizer.LEVDotARK.Blocks;
using static Randomizer.Utils;

namespace Randomizer.LEVDotARK
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
    /// A publicly accessible buffer is available and can be saved anytime. Each block can also be updated and,
    /// in turn, update the ark buffer. This also contains a Sha256 hash of the original lev.ark.
    ///
    /// </summary>
    public class ArkLoader: ISaveBinary
    {
        public const string PristineLevArkSha256Hash =
            "87e9a6e5d249df273e1964f48ad910afee6f7e073165c00237dfb9a22ae3a121";

        public const int NumOfLevels = 9; // In UW1

        public byte[] arkbuffer;
        public string arkpath;
        public byte[] CurrentLevArkSha256Hash = new byte[] {};

        public Header header;
        public Block[] blocks;
        public TileMapMasterObjectListBlock[] TileMapObjectsBlocks;
        public ObjectAnimationOverlayInfoBlock[] ObjAnimBlocks;
        public TextureMappingBlock[] TextMapBlocks;
        public AutomapInfosBlock[] AutomapBlocks;
        public MapNotesBlock[] MapNotesBlocks;

        public enum Sections
        {
            level_tilemap_objlist = 0,
            object_anim_overlay_info = 1,
            texture_mappings = 2,
            automap_infos = 3,
            map_notes = 4,
            unused = 5,
        }

        IDictionary<Sections, int> BlockLengths = new Dictionary<Sections, int>()
        {
            {Sections.level_tilemap_objlist, TileMapMasterObjectListBlock.TotalBlockLength},
            {Sections.object_anim_overlay_info, ObjectAnimationOverlayInfoBlock.TotalBlockLength},
            {Sections.texture_mappings, TextureMappingBlock.TotalBlockLength },
            {Sections.automap_infos, 0},
            {Sections.map_notes, 0},
            {Sections.unused, 0 }
        };

        private ArkLoader() : this(Settings.DefaultArkPath)
        {
        }

        /// <summary>
        /// Instantiates a new ArkLoader object using a provided path.
        /// </summary>
        /// <param name="arkpath"></param>
        public ArkLoader(string arkpath)
        {
            this.arkpath = arkpath;
            arkbuffer = LoadArkfile(arkpath);
            int headerSize = Header.blockNumSize + Header.blockOffsetSize * Header.NumEntriesFromBuffer(arkbuffer);
            header = new Header(arkbuffer[0..headerSize]);

            blocks = new Block[header.NumEntries];
            TileMapObjectsBlocks = new TileMapMasterObjectListBlock[NumOfLevels];
            ObjAnimBlocks = new ObjectAnimationOverlayInfoBlock[NumOfLevels];
            TextMapBlocks = new TextureMappingBlock[NumOfLevels];
            AutomapBlocks = new AutomapInfosBlock[NumOfLevels];
            MapNotesBlocks = new MapNotesBlock[NumOfLevels];
            
            CheckEqualityToPristineLevDotArk();
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

        public byte[] ReconstructBufferFromBlocks()
        {
            byte[] tempbuffer = new byte[arkbuffer.Length];
            List<byte> templist = new List<byte>();
            templist.AddRange(header.buffer);
            foreach(var block in blocks)
            {
                templist.AddRange(block.blockbuffer);
            }
            byte[] concatbuffers = templist.ToArray();
            concatbuffers.CopyTo(tempbuffer, 0);

            return tempbuffer;
        }

        public void CheckEqualityToPristineLevDotArk()
        {
            SHA256 mySHA256 = SHA256.Create();
            this.CurrentLevArkSha256Hash = mySHA256.ComputeHash(this.arkbuffer);
            if (!CompareCurrentArkWithHash())
            {
                Debug.WriteLine(
                    "Warning: Current ark file is different from original. Are you editing a save file? If not, report this.");
            }
            else
            {
                Debug.WriteLine("Current lev.ark is the same as the original.");
            }
        }

        public bool CompareCurrentArkWithHash()
        {
            if (PristineLevArkSha256Hash.Length / 2 != CurrentLevArkSha256Hash.Length)
            {
                return false;
            }

            for (int currbyte = 0; currbyte < PristineLevArkSha256Hash.Length / 2; currbyte++)
            {
                int currbyteStrPos = currbyte * 2;
                //byte originalByte = byte.Parse(PristineLevArkSha256Hash[currbyteStrPos..(currbyteStrPos + 2)], );
                byte originalByte = Convert.ToByte(PristineLevArkSha256Hash[currbyteStrPos..(currbyteStrPos + 2)], 16);
                byte currentbyte = CurrentLevArkSha256Hash[currbyte];
                if (originalByte != currentbyte)
                {
                    return false;
                }
            }

            return true;
        }

        public byte[] GetBlockBuffer(short blockNum, int BlockLength)
        {
            if (blockNum > header.NumEntries)
            {
                throw new InvalidDataException($"Invalid block number ({blockNum})Block number is greater than total" +
                                               $" number of blocks {header.NumEntries}");
            }

            //int blockOffset = BitConverter.ToInt32(arkbuffer[(2 + 4 * blockNum)..(6 + 4 * blockNum)]);
            int blockOffset = header.BlockOffsets[blockNum];
            
            if (blockOffset == 0)
            {
                return Array.Empty<byte>(); // TODO: figure out why this is better than new byte[] {}
            }

            byte[] blockBuffer = arkbuffer[blockOffset..(blockOffset + BlockLength)];
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
            if ((BlockType == Sections.map_notes) | (BlockType == Sections.automap_infos))
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
                case Sections.level_tilemap_objlist:
                    return new TileMapMasterObjectListBlock(buffer, levelnumber);
                case Sections.object_anim_overlay_info:
                    return new ObjectAnimationOverlayInfoBlock(buffer, levelnumber);
                case Sections.texture_mappings:
                    return new TextureMappingBlock(buffer, levelnumber);
                case Sections.automap_infos:
                    return new AutomapInfosBlock(buffer, levelnumber);
                case Sections.map_notes:
                    return new MapNotesBlock(buffer, levelnumber);
                case Sections.unused:
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
                path = Settings.DefaultArkPath;
            }
            return System.IO.File.ReadAllBytes(path);
        }

    public string? SaveBuffer(string? basePath = null, string extraInfo = "")
        {
            if (basePath is null)
            {
                basePath = Settings.DefaultArkPath;
            }
            if (extraInfo.Length == 0)
            {
                extraInfo = $@"NEWLEV.ARK";
            }
            return StdSaveBuffer(arkbuffer, basePath, extraInfo);
        }

    }
}