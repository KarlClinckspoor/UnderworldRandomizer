using System.Diagnostics;
using UWRandomizerEditor.LEVDotARK.GameObjects;

namespace UWRandomizerEditor.LEVDotARK.Blocks
{
    // TODO: Separate this into TileMap and Object List...?
    public class TileMapMasterObjectListBlock : Block
    {
        // From uw-formats.txt
        // offset  size   description
        // 0000    4000   tilemap (64 x 64 x 4 bytes)
        public const int TileMapOffset = 0;
        public const int TileMapLength = 0x4000;
        public const int TileWidth = 64;
        public const int TileHeight = 64;

        public const int TileMapEntrySize = 4;

        // 4000    1b00   mobile object information (objects 0000-00ff, 256 x 27 bytes)
        public const int MobileObjectInfoOffset = 0x4000;
        public const int MobileObjectInfoLength = 0x1b00;
        public const int MobileObjectInfoEntrySize = MobileObject.TotalLength;
        public const int MobileObjectNum = 256;

        // 5b00    1800   static object information (objects 0100-03ff, 768 x 8 bytes)
        public const int StaticObjectInfoOffset = 0x5b00;
        public const int StaticObjectInfoLength = 0x1800;
        public const int StaticObjectInfoEntrySize = GameObject.TotalLength;
        public const int StaticObjectNum = 768;

        // 7300    01fc   free list for mobile objects (objects 0002-00ff, 254 x 2 bytes)
        public const int FreeListMobileObjectsOffset = 0x7300;
        public const int FreeListMobileObjectsLength = 0x01fc;
        public const int FreeListMobileObjectsEntrySize = 2;
        public const int FreeListMobileObjectsNum = 254;

        // 74fc    0600   free list for static objects (objects 0100-03ff, 768 x 2 bytes)
        public const int FreeListStaticObjectsOffset = 0x74fc;
        public const int FreeListStaticObjectsLength = 0x0600;
        public const int FreeListStaticObjectsEntrySize = 2;
        public const int FreeListStaticObjectsNum = 768;

        // 7afc    0104   unknown(260 bytes)
        public const int UnknownOffset = 0x7afc;
        public const int UnknownLength = 0x104;
        public const int UnknownEntrySize = 1; // irrelevant ATM

        // 7c00    0002
        public const int Unknown2Offset = 0x7c00;
        public const int Unknown2Length = 2;
        public const int Unknown2EntrySize = 2;

        // 7c02    0002   no.entries in mobile free list minus 1
        public const int NumEntriesMobileFreeListAdjOffset = 0x7c02;
        public const int NumEntriesMobileFreeListAdjLength = 2;
        public const int NumEntriesMobileFreeListAdjEntrySize = 2;
        public short NumEntriesInMobileListMinus1
        {
            get { return BitConverter.ToInt16(blockbuffer, NumEntriesMobileFreeListAdjOffset); }
            set { BitConverter.GetBytes(value).CopyTo(blockbuffer, NumEntriesInMobileListMinus1); }
        }
        // 7c04    0002   no. entries in static free list minus 1
        public const int NumEntriesStaticFreeListAdjOffset = 0x7c04;
        public const int NumEntriesStaticFreeListAdjLength = 2;
        public const int NumEntriesStaticFreeListAdjEntrySize = 2;
        public short NumEntriesInStaticListMinus1
        {
            get { return BitConverter.ToInt16(blockbuffer, NumEntriesStaticFreeListAdjOffset); }
            set { BitConverter.GetBytes(value).CopyTo(blockbuffer, NumEntriesStaticFreeListAdjOffset); }
        }
        // 7c06    0002   0x7775 ('uw')
        public const int EndOfBlockConfirmationOffset = 0x7c06;
        public const int EndOfBlockConfirmationDataSize = 2;
        public const short EndOfBlockConfirmationValue = 0x7775; // "uw"

        public static new int TotalBlockLength = 0x7c08;
        
        public TileInfo[] TileInfos = new TileInfo[TileMapLength / TileMapEntrySize];

        public GameObject[] AllGameObjects = new GameObject[MobileObjectNum + StaticObjectNum];
        public MobileObject[] MobileObjects = new MobileObject[MobileObjectNum];
        public StaticObject[] StaticObjects = new StaticObject[StaticObjectNum];
        
        public FreeListObjectEntry[] FreeListMobileObject = new FreeListObjectEntry[FreeListMobileObjectsNum];
        public FreeListObjectEntry[] FreeListStaticObject = new FreeListObjectEntry[FreeListStaticObjectsNum];
        // public int LevelNumber;  // for safekeeping, irrelevant to the buffers.

        // todo: Recheck and make sure the number of entries is correct.
        public void UpdateBuffer()
        {
            TileMapBuffer.CopyTo(blockbuffer, TileMapOffset);
            MobileObjectInfoBuffer.CopyTo(blockbuffer, MobileObjectInfoOffset);
            StaticObjectInfoBuffer.CopyTo(blockbuffer, StaticObjectInfoOffset);
            FreeListMobileObjectBuffer.CopyTo(blockbuffer, FreeListMobileObjectsOffset);
            FreeListStaticObjectBuffer.CopyTo(blockbuffer, FreeListStaticObjectsOffset);
            UnknownBuffer.CopyTo(blockbuffer, UnknownOffset);
            Unknown2Buffer.CopyTo(blockbuffer, Unknown2Offset);
            // todo: do I really need these 2? Seems this is always kept updated.
            BitConverter.GetBytes(NumEntriesInMobileListMinus1).CopyTo(blockbuffer, NumEntriesMobileFreeListAdjOffset);
            BitConverter.GetBytes(NumEntriesInStaticListMinus1).CopyTo(blockbuffer, NumEntriesStaticFreeListAdjOffset); 
            BitConverter.GetBytes(EndOfBlockConfirmationValue).CopyTo(blockbuffer, EndOfBlockConfirmationOffset);
            Debug.Assert(blockbuffer.Length == TotalBlockLength);
        }

        public void UpdateObjectInfoBuffer()
        {
            UpdateMobileObjectInfoBuffer();
            UpdateStaticObjectInfoBuffer();
            UpdateFreeListMobileObjectBuffer();
            UpdateFreeListStaticObjectBuffer();
        }

        public void UpdateMobileObjectInfoBuffer()
        {
            // Let's use the object's own index to decide where it goes
            foreach (MobileObject mobj in MobileObjects)
            {
                mobj.Buffer.CopyTo(MobileObjectInfoBuffer, mobj.IdxAtObjectArray * MobileObject.TotalLength);
            }
        }

        public void UpdateStaticObjectInfoBuffer()
        {
            foreach (GameObject obj in StaticObjects)
            {
                obj.Buffer.CopyTo(StaticObjectInfoBuffer, obj.IdxAtObjectArray * GameObject.TotalLength);
            }
        }

        public void UpdateFreeListStaticObjectBuffer()
        {
            throw new NotImplementedException();//todo
        }
        
        public void UpdateFreeListMobileObjectBuffer()
        {
            throw new NotImplementedException();//todo
        }

        public void Populate_StaticObjectsFromBuffer()
        {
            for (short i = 0; i < StaticObjectNum; i++)
            {
                byte[] currbuffer =
                    StaticObjectInfoBuffer[(i * StaticObject.TotalLength)..((i + 1) * StaticObject.TotalLength)];
                var currobj = (StaticObject) GameObjectFactory.CreateFromBuffer(currbuffer, (short) (i +  MobileObjectNum));

                if (currobj.IdxAtObjectArray < MobileObjectNum)
                {
                    throw new Exception(
                        "Attempted to add a static object to the region of mobile objects. Should not happen!");
                }
                
                StaticObjects[i] = currobj;
                AllGameObjects[currobj.IdxAtObjectArray] = currobj;
            }
        }
        
        // todo: consider also providing an entry number, for safekeeping
        public void Populate_MobileObjectsFromBuffer()
        {
            for (short i = 0; i < MobileObjectNum; i++)
            {
                byte[] currbuffer =
                    MobileObjectInfoBuffer[(i * MobileObject.TotalLength)..((i + 1) * MobileObject.TotalLength)];
                var currobj = (MobileObject) GameObjectFactory.CreateFromBuffer(currbuffer, i);
                
                if (currobj.IdxAtObjectArray >= MobileObjectNum)
                {
                    throw new Exception(
                        "Attempted to add a static object to the region of mobile objects. Should not happen!");
                }
                
                MobileObjects[i] = currobj;
                AllGameObjects[currobj.IdxAtObjectArray] = currobj;
            }
        }

        public void Populate_AllGameObjectsFromBuffer()
        {
            Populate_MobileObjectsFromBuffer();
            Populate_StaticObjectsFromBuffer();
        }
        
        public void Populate_FreeListMobileObjectArrFromBuffer()
        {
            for (int i = 0; i < FreeListMobileObjectsNum; i++)
            {
                byte[] currbuffer =
                    FreeListMobileObjectBuffer[(i * FreeListObjectEntry.EntrySize)..((i + 1) * FreeListObjectEntry.EntrySize)];
                var currobj = new FreeListObjectEntry(currbuffer, i);
                FreeListMobileObject[i] = currobj;
            }
        }
        
        public void Populate_FreeListStaticObjectArrFromBuffer()
        {
            for (int i = 0; i < FreeListStaticObjectsNum; i++)
            {
                byte[] currbuffer =
                    FreeListStaticObjectBuffer[(i * FreeListObjectEntry.EntrySize)..((i + 1) * FreeListObjectEntry.EntrySize)];
                var currobj = new FreeListObjectEntry(currbuffer, i);
                FreeListStaticObject[i] = currobj;
            }
        }

        public TileMapMasterObjectListBlock(byte[] blockbuffer, int levelNumber)
        {
            // Debug.Assert(blockbuffer.Length == TotalBlockLength);
            this.blockbuffer = blockbuffer;
            this.LevelNumber = levelNumber;
        }

        #region information extraction

        public void ExtractInfoFromTileMapBuffer()
        {
            // TileInfo[] tileInfos = new TileInfo[TileMapLength / TileMapEntrySize];

            for (int i = 0; i < TileMapLength / TileMapEntrySize; i++)
            {
                
                int offset = i * TileMapEntrySize;
                // Todo: Seems a bit weird to convert and de-convert later. Think better.
                int entry = BitConverter.ToInt32(TileMapBuffer, offset);
                TileInfo currInfo = new TileInfo(i, entry, offset, LevelNumber);

                TileInfos[i] = currInfo;
            }
        }

        
        public void UpdateTileMapBuffer()
        {
            // Todo: Check that TileInfos is the required length AND there aren't repeat indices.
            foreach (TileInfo currInfo in TileInfos)
            {
                currInfo.TileBuffer.CopyTo(TileMapBuffer, currInfo.Offset);
            }
            // byte[] newbuffer = new byte[TileInfos.Length * TileInfo.Size];
            
            // Thinking... which approach is better? I stored the offset for a reason... I think the second method can allow for
            // more flexibility though.

            //int i = 0;
            //foreach (TileInfo currInfo in TileInfos)
            //{
            //    currInfo.TileBuffer.CopyTo(newbuffer, i * TileInfo.Size);
            //    i++;
            //}

            // foreach (TileInfo currInfo in TileInfos)
            // {
            //     currInfo.TileBuffer.CopyTo(newbuffer, currInfo.Offset);
            // }
            // return newbuffer;
            // return newbuffer;
        }

        #endregion



        
        // #region properties
        // TODO: Verify this implementation!
        // public bool IsLengthOfMobileListValid()
        // {
        //     int counter = 0;
        //     foreach (MobileObject mobj in FreeMobileObject)
        //     {
        //         if (mobj.ItemID != 0)
        //         {
        //             counter += 1;
        //         }
        //     }
        //
        //     if (counter - 1 != NumEntriesInMobileListMinus1)
        //     {
        //         return false;
        //     }
        //     else
        //     {
        //         return true;
        //     }
        // }
        // // TODO: Verify this implementation!
        // public bool IsLengthOfStaticListValid()
        // {
        //     int counter = 0;
        //     foreach (GameObject mobj in FreeStaticObject)
        //     {
        //         if (mobj.ItemID != 0)
        //         {
        //             counter += 1;
        //         }
        //     }
        //
        //     if (counter - 1 != NumEntriesInStaticListMinus1)
        //     {
        //         return false;
        //     }
        //     else
        //     {
        //         return true;
        //     }
        // }
        // #endregion

        #region buffer definitions

        protected byte[] TileMapBuffer
        {
            get { return blockbuffer[TileMapOffset..(TileMapOffset + TileMapLength)]; }
            set
            {
                Trace.Assert(value.Length == TileMapLength); // better to throw exception?
                value.CopyTo(blockbuffer, TileMapOffset);
            }
        }

        protected byte[] MobileObjectInfoBuffer
        {
            get { return blockbuffer[MobileObjectInfoOffset..(MobileObjectInfoOffset + MobileObjectInfoLength)]; }
            set
            {
                Trace.Assert(value.Length == MobileObjectInfoLength); // better to throw exception?
                value.CopyTo(blockbuffer, MobileObjectInfoOffset);
            }
        }

        protected byte[] StaticObjectInfoBuffer
        {
            get { return blockbuffer[StaticObjectInfoOffset..(StaticObjectInfoOffset + StaticObjectInfoLength)]; }
            set
            {
                Trace.Assert(value.Length == StaticObjectInfoLength); // better to throw exception?
                value.CopyTo(blockbuffer, StaticObjectInfoOffset);
            }
        }

        protected byte[] FreeListMobileObjectBuffer
        {
            get
            {
                return blockbuffer[
                    FreeListMobileObjectsOffset..(FreeListMobileObjectsOffset + FreeListMobileObjectsLength)];
            }
            set
            {
                Trace.Assert(value.Length == FreeListMobileObjectsLength);
                value.CopyTo(blockbuffer, FreeListMobileObjectsOffset);
            }
        }

        protected byte[] FreeListStaticObjectBuffer
        {
            get
            {
                return blockbuffer[
                    FreeListStaticObjectsOffset..(FreeListStaticObjectsOffset + FreeListStaticObjectsLength)];
            }
            set
            {
                Trace.Assert(value.Length == FreeListStaticObjectsLength);
                value.CopyTo(blockbuffer, FreeListStaticObjectsOffset);
            }
        }

        protected byte[] UnknownBuffer
        {
            get { return blockbuffer[UnknownOffset..(UnknownOffset + UnknownLength)]; }
            set
            {
                Trace.Assert(value.Length == UnknownLength);
                value.CopyTo(blockbuffer, UnknownOffset);
            }
        }

        protected byte[] Unknown2Buffer
        {
            get { return blockbuffer[Unknown2Offset..(Unknown2Offset + Unknown2Length)]; }
            set
            {
                Trace.Assert(value.Length == Unknown2Length);
                value.CopyTo(blockbuffer, Unknown2Offset);
            }
        }

        #endregion

        public override string SaveBuffer(string? basePath = null, string filename = "")
        {
            if (basePath is null)
            {
                basePath = Settings.DefaultBinaryTestsPath;
            }
            return base.SaveBuffer(basePath, filename.Length == 0 ? $@"_TileMapObjList_{LevelNumber}" : filename);
        }
    }
}