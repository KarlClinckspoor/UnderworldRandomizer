using System.Data;
using System.Diagnostics;
using UWRandomizerEditor.LEVDotARK.GameObjects;

namespace UWRandomizerEditor.LEVDotARK.Blocks
{
    // TODO: Separate this into TileMap and Object List...?
    public partial class TileMapMasterObjectListBlock : Block
    {
        public TileInfo[] TileInfos = new TileInfo[TileMapLength / TileMapEntrySize];

        public GameObject[] AllGameObjects = new GameObject[MobileObjectNum + StaticObjectNum];
        public MobileObject[] MobileObjects = new MobileObject[MobileObjectNum];
        public StaticObject[] StaticObjects = new StaticObject[StaticObjectNum];

        public FreeListObjectEntry[] FreeListMobileObjects = new FreeListObjectEntry[FreeListMobileObjectsNum];
        public FreeListObjectEntry[] FreeListStaticObjects = new FreeListObjectEntry[FreeListStaticObjectsNum];

        public short NumEntriesInMobileListMinus1
        {
            get { return BitConverter.ToInt16(Buffer, NumEntriesMobileFreeListAdjOffset); }
            set { BitConverter.GetBytes(value).CopyTo(Buffer, NumEntriesInMobileListMinus1); }
        }

        public short NumEntriesInStaticListMinus1
        {
            get { return BitConverter.ToInt16(Buffer, NumEntriesStaticFreeListAdjOffset); }
            set { BitConverter.GetBytes(value).CopyTo(Buffer, NumEntriesStaticFreeListAdjOffset); }
        }

        // todo: Recheck and make sure the number of entries is correct.
        public override bool ReconstructBuffer()
        {
            if (Buffer.Length != FixedBlockLength)
            {
                throw new ConstraintException(
                    $"Somehow the length of TileMapMasterObjectListBlock has the invalid length of {Buffer.Length}");
            }

            ReconstructSubBuffers();

            TileMapBuffer.CopyTo(Buffer, TileMapOffset);
            MobileObjectInfoBuffer.CopyTo(Buffer, MobileObjectInfoOffset);
            StaticObjectInfoBuffer.CopyTo(Buffer, StaticObjectInfoOffset);
            FreeListMobileObjectBuffer.CopyTo(Buffer, FreeListMobileObjectsOffset);
            FreeListStaticObjectBuffer.CopyTo(Buffer, FreeListStaticObjectsOffset);
            UnknownBuffer.CopyTo(Buffer, UnknownOffset);
            Unknown2Buffer.CopyTo(Buffer, Unknown2Offset);
            // todo: do I really need these 2? Seems this is always kept updated.
            BitConverter.GetBytes(NumEntriesInMobileListMinus1).CopyTo(Buffer, NumEntriesMobileFreeListAdjOffset);
            BitConverter.GetBytes(NumEntriesInStaticListMinus1).CopyTo(Buffer, NumEntriesStaticFreeListAdjOffset);
            BitConverter.GetBytes(EndOfBlockConfirmationValue).CopyTo(Buffer, EndOfBlockConfirmationOffset);
            return true;
        }

        private void ReconstructSubBuffers()
        {
            ReconstructMobileObjectInfoBuffer();
            ReconstructStaticObjectInfoBuffer();
            ReconstructFreeListMobileObjectBuffer();
            ReconstructFreeListStaticObjectBuffer();
            ReconstructTileMapBuffer();
        }

        private void ReconstructMobileObjectInfoBuffer()
        {
            // Let's use the object's own index to decide where it goes...
            foreach (var mobj in MobileObjects)
            {
                mobj.ReconstructBuffer();
                mobj.Buffer.CopyTo(MobileObjectInfoBuffer, mobj.IdxAtObjectArray * MobileObject.FixedTotalLength);
            }
        }

        private void ReconstructStaticObjectInfoBuffer()
        {
            foreach (var obj in StaticObjects)
            {
                obj.ReconstructBuffer();
                // obj.Buffer.CopyTo(StaticObjectInfoBuffer, (obj.IdxAtObjectArray - MobileObjectNum) * StaticObject.FixedTotalLength);
                obj.Buffer.CopyTo(StaticObjectInfoBuffer,
                    (obj.IdxAtObjectArray - MobileObjectNum) * StaticObject.FixedTotalLength);
            }
        }

        private void ReconstructFreeListStaticObjectBuffer()
        {
            foreach (var obj in FreeListStaticObjects)
            {
                obj.ReconstructBuffer();
                obj.Buffer.CopyTo(FreeListStaticObjectBuffer, obj.EntryNum * FreeListObjectEntry.FixedSize);
            }
        }

        private void ReconstructFreeListMobileObjectBuffer()
        {
            foreach (var obj in FreeListMobileObjects)
            {
                obj.ReconstructBuffer();
                obj.Buffer.CopyTo(FreeListMobileObjectBuffer, obj.EntryNum * FreeListObjectEntry.FixedSize);
            }
        }

        private void ReconstructTileMapBuffer()
        {
            // Todo: Check that TileInfos is the required length AND there aren't repeat indices.
            foreach (TileInfo currInfo in TileInfos)
            {
                currInfo.ReconstructBuffer();
                currInfo.Buffer.CopyTo(TileMapBuffer, currInfo.Offset);
            }
        }

        private void Populate_StaticObjectsFromBuffer()
        {
            for (short i = 0; i < StaticObjectNum; i++)
            {
                byte[] currbuffer =
                    StaticObjectInfoBuffer[
                        (i * StaticObject.FixedTotalLength)..((i + 1) * StaticObject.FixedTotalLength)];
                var currobj =
                    (StaticObject) GameObjectFactory.CreateFromBuffer(currbuffer, (ushort) (i + MobileObjectNum));

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
        private void Populate_MobileObjectsFromBuffer()
        {
            for (ushort i = 0; i < MobileObjectNum; i++)
            {
                byte[] currbuffer =
                    MobileObjectInfoBuffer[
                        (i * MobileObject.FixedTotalLength)..((i + 1) * MobileObject.FixedTotalLength)];
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

        private void Populate_FreeListMobileObjectArrFromBuffer()
        {
            for (int i = 0; i < FreeListMobileObjectsNum; i++)
            {
                byte[] currbuffer =
                    FreeListMobileObjectBuffer[
                        (i * FreeListObjectEntry.FixedSize)..((i + 1) * FreeListObjectEntry.FixedSize)];
                var currobj = new FreeListObjectEntry(currbuffer, i);
                FreeListMobileObjects[i] = currobj;
            }
        }

        private void Populate_FreeListStaticObjectArrFromBuffer()
        {
            for (int i = 0; i < FreeListStaticObjectsNum; i++)
            {
                byte[] currbuffer =
                    FreeListStaticObjectBuffer[
                        (i * FreeListObjectEntry.FixedSize)..((i + 1) * FreeListObjectEntry.FixedSize)];
                var currobj = new FreeListObjectEntry(currbuffer, i);
                FreeListStaticObjects[i] = currobj;
            }
        }

        public TileMapMasterObjectListBlock(byte[] buffer, int levelNumber)
        {
            if (buffer.Length != FixedBlockLength)
            {
                throw new ArgumentException(
                    $"Length of buffer ({buffer.Length}) is incompatible with expected TextureMappingBlock length ({FixedBlockLength})");
            }

            Buffer = new byte[FixedBlockLength];
            buffer.CopyTo(Buffer, 0);
            LevelNumber = levelNumber;
            Populate_MobileObjectsFromBuffer();
            Populate_StaticObjectsFromBuffer();
            Populate_FreeListStaticObjectArrFromBuffer();
            Populate_FreeListMobileObjectArrFromBuffer();
            Populate_TileInfos();
        }

        private void Populate_TileInfos()
        {
            for (int i = 0; i < TileMapLength / TileMapEntrySize; i++)
            {
                int offset = i * TileMapEntrySize;
                // Todo: Seems a bit weird to convert and de-convert later. Think better.
                int entry = BitConverter.ToInt32(TileMapBuffer, offset);
                TileInfo currInfo = new TileInfo(i, entry, offset, LevelNumber);

                TileInfos[i] = currInfo;
            }
        }


        public byte[] TileMapBuffer
        {
            get { return Buffer[TileMapOffset..(TileMapOffset + TileMapLength)]; }
            set
            {
                if (value.Length != TileMapLength)
                {
                    throw new ArgumentException($"Invalid length of TileMapBuffer");
                }

                value.CopyTo(Buffer, TileMapOffset);
            }
        }

        public byte[] MobileObjectInfoBuffer
        {
            get { return Buffer[MobileObjectInfoOffset..(MobileObjectInfoOffset + MobileObjectInfoLength)]; }
            set
            {
                if (value.Length != MobileObjectInfoLength)
                {
                    throw new ArgumentException($"Invalid length of MobileObjectInfoBuffer");
                }

                value.CopyTo(Buffer, MobileObjectInfoOffset);
            }
        }

        public byte[] StaticObjectInfoBuffer
        {
            get { return Buffer[StaticObjectInfoOffset..(StaticObjectInfoOffset + StaticObjectInfoLength)]; }
            set
            {
                if (value.Length != StaticObjectInfoLength)
                {
                    throw new ArgumentException($"Invalid length of StaticObjectInfoBuffer");
                }

                value.CopyTo(Buffer, StaticObjectInfoOffset);
            }
        }

        public byte[] FreeListMobileObjectBuffer
        {
            get
            {
                return Buffer[
                    FreeListMobileObjectsOffset..(FreeListMobileObjectsOffset + FreeListMobileObjectsLength)];
            }
            set
            {
                if (value.Length != FreeListMobileObjectsLength)
                {
                    throw new ArgumentException($"Invalid length of FreeListMobileObjectBuffer");
                }

                value.CopyTo(Buffer, FreeListMobileObjectsOffset);
            }
        }

        public byte[] FreeListStaticObjectBuffer
        {
            get
            {
                return Buffer[
                    FreeListStaticObjectsOffset..(FreeListStaticObjectsOffset + FreeListStaticObjectsLength)
                ];
            }
            set
            {
                if (value.Length != FreeListMobileObjectsLength)
                {
                    throw new ArgumentException($"Invalid length of FreeListStaticObjectBuffer");
                }

                value.CopyTo(Buffer, FreeListStaticObjectsOffset);
            }
        }

        protected byte[] UnknownBuffer
        {
            get { return Buffer[UnknownOffset..(UnknownOffset + UnknownLength)]; }
            set
            {
                if (value.Length != UnknownLength)
                {
                    throw new ArgumentException($"Invalid length");
                }

                value.CopyTo(Buffer, UnknownOffset);
            }
        }

        protected byte[] Unknown2Buffer
        {
            get { return Buffer[Unknown2Offset..(Unknown2Offset + Unknown2Length)]; }
            set
            {
                if (value.Length != Unknown2Length)
                {
                    throw new ArgumentException($"Invalid length");
                }

                value.CopyTo(Buffer, Unknown2Offset);
            }
        }
    }
}