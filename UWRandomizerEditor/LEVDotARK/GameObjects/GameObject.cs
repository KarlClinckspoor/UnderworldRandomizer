using System.Diagnostics;
using UWRandomizerEditor.Interfaces;

// TODO: Maybe make a different class of Container objects?
// TODO: What about empty entries? And the null entry (id = 0)?
// TODO: Add checks that prevent modification if id=0.
namespace UWRandomizerEditor.LEVDotARK.GameObjects
{
    public class GameObject : IEquatable<GameObject>, IShouldIMove, IBufferObject
    {
        public const int InfoSize = 2;
        public const int InfoNum = 4;
        public const int BaseLength = InfoNum * InfoSize;
        public const int ExtraLength = 0;
        public const int FixedTotalLength = BaseLength + ExtraLength;
        public ushort IdxAtObjectArray;

        public virtual bool ShouldBeMoved { get; set; } = true;

        public byte[] Buffer { get; set; }
        // public short[] GeneralInfo = new short[InfoNum] {0, 0, 0, 0};

        protected ushort LinkSpecial;
        protected ushort ObjIdFlags;
        protected ushort Position;
        protected ushort QualityChain;

        /// <summary>
        /// Tells whether or not an item has matching itemID and buffer size, i.e., can't have a Sword as a Mobile Object
        /// </summary>
        public bool Invalid { get; set; } = false;

        protected GameObject()
        {
            Buffer = Array.Empty<byte>();
            Invalid = true;
        }

        public GameObject(byte[] buffer, ushort idxAtObjArray)
        {
            if (buffer.Length != FixedTotalLength)
            {
                throw new ArgumentException(
                    $"Length of buffer ({buffer.Length}) is incompatible with GameObject length ({FixedTotalLength})");
            }

            Buffer = new byte[FixedTotalLength];
            buffer.CopyTo(Buffer, 0);
            IdxAtObjectArray = idxAtObjArray;
            UpdateEntries();
        }

        public GameObject(ushort objIdFlags, ushort position, ushort qualityChain,
            ushort linkSpecial)
        {
            ObjIdFlags = objIdFlags;
            Position = position;
            QualityChain = qualityChain;
            LinkSpecial = linkSpecial;

            Buffer = new byte[FixedTotalLength];
            ReconstructBuffer();
        }

        protected virtual void UpdateEntries()
        {
            ObjIdFlags = BitConverter.ToUInt16(Buffer, 0);
            Position = BitConverter.ToUInt16(Buffer, 2);
            QualityChain = BitConverter.ToUInt16(Buffer, 4);
            LinkSpecial = BitConverter.ToUInt16(Buffer, 6);
        }

        public virtual bool ReconstructBuffer()
        {
            byte[] field1 = BitConverter.GetBytes(ObjIdFlags);
            byte[] field2 = BitConverter.GetBytes(Position);
            byte[] field3 = BitConverter.GetBytes(QualityChain);
            byte[] field4 = BitConverter.GetBytes(LinkSpecial);

            field1.CopyTo(Buffer, 0);
            field2.CopyTo(Buffer, 2);
            field3.CopyTo(Buffer, 4);
            field4.CopyTo(Buffer, 6);
            return true;
        }

        #region First Short

        public int ItemID
        {
            get { return Utils.GetBits(ObjIdFlags, 0b111111111, 0); }
            set
            {
                ObjIdFlags = (ushort) Utils.SetBits(ObjIdFlags, value, 0b111111111, 0);
                ReconstructBuffer();
            }
        }

        public int Flags
        {
            get
            {
                // todo: error in uw-formats.txt? Check hank's. -- He uses a mask with 3 bits, so I changed here.
                return Utils.GetBits(ObjIdFlags, 0b111, 9);
            }
            set
            {
                ObjIdFlags = (ushort) Utils.SetBits(ObjIdFlags, value, 0b1111, 9);
                ReconstructBuffer();
            }
        }

        public int EnchantFlag
        {
            get { return Utils.GetBits(Flags, 0b1, 0); }
            set
            {
                Flags = (short) Utils.SetBits(Flags, value, 0b1, 0);
                ReconstructBuffer();
            }
        }

        public int Doordir
        {
            get { return Utils.GetBits(Flags, 0b1, 1); }
            set
            {
                Flags = (short) Utils.SetBits(Flags, value, 0b1, 1);
                ReconstructBuffer();
            }
        }

        public int Invis
        {
            get { return Utils.GetBits(Flags, 0b1, 2); }
            set
            {
                this.Flags = (short) Utils.SetBits(Flags, value, 0b1, 2);
                ReconstructBuffer();
            }
        }

        public int IsQuant
        {
            get { return Utils.GetBits(Flags, 0b1, 3); }
            set
            {
                this.Flags = (short) Utils.SetBits(Flags, value, 0b1, 3);
                ReconstructBuffer();
            }
        }

        #endregion

        #region Second Short

        public byte Zpos
        {
            get { return (byte) Utils.GetBits(Position, 0b1111111, 0); }
            set
            {
                Position = (ushort) Utils.SetBits(Position, value, 0b1111111, 0);
                ReconstructBuffer();
            }
        }

        public byte Heading
        {
            // get { return (byte) ((position >> 7) & 0b111); }
            get { return (byte) Utils.GetBits(Position, 0b111, 7); }
            set
            {
                Position = (ushort) Utils.SetBits(Position, value, 0b111, 7);
                ReconstructBuffer();
            }
        }

        public byte Ypos
        {
            // get { return (byte) ((position >> 10) & 0b111); }
            get { return (byte) Utils.GetBits(Position, 0b111, 10); }
            set
            {
                Position = (ushort) Utils.SetBits(Position, value, 0b111, 10);
                ReconstructBuffer();
            }
        }

        public byte Xpos
        {
            // get { return (byte) ((position >> 13) & 0b111); }
            get { return (byte) Utils.GetBits(Position, 0b111, 13); }
            set
            {
                Position = (ushort) Utils.SetBits(Position, value, 0b111, 13);
                ReconstructBuffer();
            }
        }

        #endregion

        #region Third Short

        public byte Quality
        {
            // get { return (byte) (QualityChain & 0b111111); }
            get { return (byte) Utils.GetBits(QualityChain, 0b111111, 0); }
            set
            {
                QualityChain = (ushort) Utils.SetBits(QualityChain, value, 0b111111, 0);
                ReconstructBuffer();
            }
        }

        public ushort next
        {
            // get { return (byte) ((QualityChain >> 6) & 0b1111111111); }
            get { return (ushort) Utils.GetBits(QualityChain, 0b1111111111, 6); }
            set
            {
                QualityChain = (ushort) Utils.SetBits(QualityChain, value, 0b1111111111, 6);
                ReconstructBuffer();
            }
        }


        public bool IsEndOfList
        {
            get { return next == 0; }
        }

        #endregion

        #region Short 4

        public byte OwnerOrSpecial
        {
            // get { return (byte) (linkSpecial & 0b111111); }
            get { return (byte) Utils.GetBits(LinkSpecial, 0b111111, 0); }
            set
            {
                LinkSpecial = (ushort) Utils.SetBits(LinkSpecial, value, 0b111111, 0);
                ReconstructBuffer();
            }
        }

        public short QuantityOrSpecialLinkOrSpecialProperty
        {
            // get { return (byte) ((linkSpecial >> 6) & 0b1111111111); }
            get { return (byte) Utils.GetBits(LinkSpecial, 0b1111111111, 6); }
            set
            {
                LinkSpecial = (ushort) Utils.SetBits(LinkSpecial, value, 0b1111111111, 6);
                ReconstructBuffer();
            }
        }

        #endregion

        public bool HasOwner
        {
            // get { return OwnerOrSpecial == 0; }
            // todo: which one?
            get { return ItemOwnerStrIdx == 0; }
        }

        public int ItemOwnerStrIdx
        {
            get { return OwnerOrSpecial - 1 + 370; }
            set { OwnerOrSpecial = (byte) (value + 1 - 370); }
        }

        // TODO: Revise all these "Is..." functions. Would they be needed by the GameObjectFactory?
        public static bool IsTexturedObject(byte[] buffer)
        {
            byte firstByte = buffer[0];
            int start = 0x160; // TODO: I haven't checked these indices thoroughly!
            int end = 0x17f; // TODO: I haven't checked these indices thoroughly!
            if ((firstByte > start) | (firstByte < end))
                return true;
            return false;
        }

        public static bool IsQuantityObject(byte[] buffer)
        {
            if (IsTexturedObject(buffer))
            {
                return false;
            }

            short first2bytes = BitConverter.ToInt16(buffer, 0);
            bool quantitybit = ((first2bytes >> 15) & 1) == 1 ? true : false;
            return quantitybit;
        }

        public static bool IsSpecialPropertyObject(byte[] buffer)
        {
            if (IsTexturedObject(buffer))
            {
                return false;
            }

            short lastShort = BitConverter.ToInt16(buffer, 6);
            int quantity = (lastShort >> 6) & 0b1111111111;
            if ((quantity > 512) & IsQuantityObject(buffer))
            {
                return true;
            }
            else if ((quantity < 512) & IsQuantityObject(buffer))
            {
                return false;
            }
            else
            {
                throw new Exception("Has to be a quantity object.");
            }
        }

        public static bool IsSpecialLinkObject(byte[] buffer)
        {
            if (IsTexturedObject(buffer))
            {
                return false;
            }

            if (!IsQuantityObject(buffer))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if two GameObject instances are the same by comparing their buffers
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool Equals(GameObject? other)
        {
            if (other == null)
            {
                return false;
            }

            if (Buffer.Length != other.Buffer.Length)
            {
                return false;
            }

            if (GetType() != other.GetType())
            {
                return false;
            }

            for (int i = 0; i < Buffer.Length; i++)
            {
                if (Buffer[i] != other.Buffer[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}