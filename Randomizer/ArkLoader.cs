using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Diagnostics;
using static Randomizer.Utils;

namespace Randomizer
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
            {Sections.automap_infos,AutomapInfosBlock.TotalBlockLength},
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
            
            CheckEqualityToPristineLevDotArk();
            LoadBlocks();
        }

        public void LoadBlocks()
        {
            // Load blocks
            int currblocktypecount = 0;
            int currblocktype = 0;
            for (short blocknum = 0; blocknum < header.NumEntries; blocknum++)
            {
                if (currblocktype >= 6)
                {
                    currblocktype = 5; // Hack
                }
                blocks[blocknum] = GetBlock(blocknum, (Sections) currblocktype, currblocktypecount);
                currblocktypecount++;
                if (currblocktypecount >= NumOfLevels) // TODO: looks like I could do a % here. Think better
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

        public Block GetBlock(short BlockNum, Sections BlockType, int levelnumber = -1)
        {
            int BlockLength;
            if (BlockType == Sections.map_notes)
            {
                // TODO: When I'm less tired, check if this will go back in the last block.
                // Here I'm assuming the map notes are of variable length. I should test and see if that's the case.
                BlockLength = header.BlockOffsets[BlockNum + 1] - header.BlockOffsets[BlockNum];
                //BlockLength = TotalBlockLength - BlockLengths[Sections.level_tilemap_objlist] - BlockLengths[Sections.object_anim_overlay_info] -
                //    BlockLengths[Sections.automap_infos];
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

        static byte[] LoadArkfile(string path = Settings.DefaultArkPath)
        {
            return System.IO.File.ReadAllBytes(path);
        }

    public bool SaveBuffer(string basepath = Settings.DefaultBinaryTestsPath, string extrainfo = "")
        {
            if (extrainfo.Length == 0)
            {
                extrainfo = $@"NEWLEV.ARK";
            }

            return StdSaveBuffer(arkbuffer, basepath, extrainfo);
        }

    }
}