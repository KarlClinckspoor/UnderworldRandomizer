using UWRandomizerEditor.Interfaces;
using static UWRandomizerEditor.Utils;

// TODO: Maybe make a different class of Container objects?
// TODO: What about empty entries? And the null entry (id = 0)?
// TODO: Add checks that prevent modification if id=0.
namespace UWRandomizerEditor.LEVDotARK.GameObjects
{
    public class GameObject: IEquatable<GameObject>, IShouldIMove, ISaveBinary
    {
        public const int InfoSize = 2;
        public const int InfoNum = 4;
        public const int BaseLength = InfoNum * InfoSize;
        public const int ExtraLength = 0;
        public const int TotalLength = BaseLength + ExtraLength;
        public short IdxAtObjectArray;

        public virtual bool ShouldBeMoved { get; set; } = true;

        public byte[] Buffer = new byte[TotalLength];
        public short[] GeneralInfo = new short[InfoNum] { 0, 0, 0, 0 };

        public ushort link_specialField;
        protected ushort objid_flagsField;
        protected ushort positionField;
        protected ushort quality_chainField;

        /// <summary>
        /// Tells whether or not an item has matching itemID and buffer size, i.e., can't have a Sword as a Mobile Object
        /// </summary>
        public bool Invalid { get; set; } = false;

        protected GameObject() { }

        // todo: Rider is complaining about the virtual methods. Check if this will be negatively affected here.
        // If not, then just re-implement the methods in this and the derived classes.
        public GameObject(byte[] buffer, short idxAtObjArray)
        {
            // Debug.Assert(buffer.Length == TotalLength);
            this.Buffer = buffer;
            this.IdxAtObjectArray = idxAtObjArray;
            UpdateEntries();
            // this.objid_flagsField = BitConverter.ToInt16(buffer, 0);
            // this.positionField = BitConverter.ToInt16(buffer, 2);
            // this.quality_chainField = BitConverter.ToInt16(buffer, 4);
            // this.link_specialField = BitConverter.ToInt16(buffer, 6);
        }

        public GameObject(ushort objid_flagsField, ushort positionField, ushort quality_chainField,
            ushort link_specialField)
        {
            this.objid_flagsField = objid_flagsField;
            this.positionField = positionField;
            this.quality_chainField = quality_chainField;
            this.link_specialField = link_specialField;
            UpdateBuffer();
        }

        public void UpdateEntries()
        {
            this.objid_flagsField = BitConverter.ToUInt16(Buffer, 0);
            this.positionField = BitConverter.ToUInt16(Buffer, 2);
            this.quality_chainField = BitConverter.ToUInt16(Buffer, 4);
            this.link_specialField = BitConverter.ToUInt16(Buffer, 6);
        }
        
        public void UpdateBuffer()
        { // todo: this can be made more elegant.
            byte[] tempbuffer = new byte[TotalLength];
            byte[] field1 = BitConverter.GetBytes(this.objid_flagsField);
            byte[] field2 = BitConverter.GetBytes(this.positionField);
            byte[] field3 = BitConverter.GetBytes(this.quality_chainField);
            byte[] field4 = BitConverter.GetBytes(this.link_specialField);

            field1.CopyTo(tempbuffer, 0);
            field2.CopyTo(tempbuffer, 2);
            field3.CopyTo(tempbuffer, 4);
            field4.CopyTo(tempbuffer, 6);

            tempbuffer.CopyTo(Buffer, 0);
        }

        #region First Short
        public int ItemID
        {
            get { return GetBits(objid_flagsField, 0b111111111, 0); }
            set { objid_flagsField = (ushort)SetBits(objid_flagsField, value, 0b111111111, 0); UpdateBuffer(); }
        }

        public int Flags
        {
            get
            {
                // todo: error in uw-formats.txt? Check hank's. -- He uses a mask with 3 bits, so I changed here.
                // return GetBits(objid_flagsField, 0b1111, 9);
                return GetBits(objid_flagsField, 0b111, 9);
            }
            set { objid_flagsField = (ushort)SetBits(objid_flagsField, value, 0b1111, 9); UpdateBuffer(); }
        }

        public int EnchantFlag
        {
            // get { return Flags & 0b1; }
            get { return GetBits(Flags, 0b1, 0); }
            set { this.Flags = (short)SetBits(Flags, value, 0b1, 0); }
        } // These update "Flags", which updates the buffer, so no need to call it every time.

        public int Doordir
        {
            // get { return (Flags >> 1) & 0b1; }
            get { return GetBits(Flags, 0b1, 1); }
            set { this.Flags = (short)SetBits(Flags, value, 0b1, 1); }
        }

        public int Invis
        {
            // get { return (Flags >> 2) & 0b1; }
            get { return GetBits(Flags, 0b1, 2); }
            set { this.Flags = (short)SetBits(Flags, value, 0b1, 2); }
        }

        public int IsQuant
        {
            // get { return (Flags >> 3) & 0b1; }
            get { return GetBits(Flags, 0b1, 3); }
            set { this.Flags = (short)SetBits(Flags, value, 0b1, 3); }
        }
        #endregion

        #region Second Short
        public byte Zpos
        {
            // get { return (byte) (positionField & 0b1111111); }
            get { return (byte)GetBits(positionField, 0b1111111, 0); }
            set { this.positionField = (ushort)SetBits(positionField, value, 0b1111111, 0); UpdateBuffer(); }
        }

        public byte Heading
        {
            // get { return (byte) ((positionField >> 7) & 0b111); }
            get { return (byte)GetBits(positionField, 0b111, 7); }
            set { positionField = (ushort)SetBits(positionField, value, 0b111, 7); UpdateBuffer(); }
        }

        public byte Ypos
        {
            // get { return (byte) ((positionField >> 10) & 0b111); }
            get { return (byte)GetBits(positionField, 0b111, 10); }
            set { positionField = (ushort)SetBits(positionField, value, 0b111, 10); UpdateBuffer(); }
        }

        public byte Xpos
        {
            // get { return (byte) ((positionField >> 13) & 0b111); }
            get { return (byte)GetBits(positionField, 0b111, 13); }
            set { positionField = (ushort)SetBits(positionField, value, 0b111, 13); UpdateBuffer(); }
        }
        #endregion
        #region Third Short
        public byte Quality
        {
            // get { return (byte) (quality_chainField & 0b111111); }
            get { return (byte)GetBits(quality_chainField, 0b111111, 0); }
            set { quality_chainField = (ushort)SetBits(quality_chainField, value, 0b111111, 0); UpdateBuffer(); }
        }

        public short next
        {
            // get { return (byte) ((quality_chainField >> 6) & 0b1111111111); }
            get { return (short)GetBits(quality_chainField, 0b1111111111, 6); }
            set { quality_chainField = (ushort)SetBits(quality_chainField, value, 0b1111111111, 6); UpdateBuffer(); }
        }


        public bool IsEndOfList
        {
            get { return next == 0; }
        }
        #endregion
        #region Short 4
        public byte Owner_or_special
        {
            // get { return (byte) (link_specialField & 0b111111); }
            get { return (byte)GetBits(link_specialField, 0b111111, 0); }
            set { link_specialField = (ushort)SetBits(link_specialField, value, 0b111111, 0); UpdateBuffer(); }
        }

        public short QuantityOrSpecialLinkOrSpecialProperty
        {
            // get { return (byte) ((link_specialField >> 6) & 0b1111111111); }
            get { return (byte)GetBits(link_specialField, 0b1111111111, 6); }
            set { link_specialField = (ushort)SetBits(link_specialField, value, 0b1111111111, 6); UpdateBuffer(); }
        }
        #endregion

        public bool HasOwner
        {
            // get { return Owner_or_special == 0; }
            // todo: which one?
            get { return ItemOwnerStrIdx == 0; }
        }

        public int ItemOwnerStrIdx
        {
            get { return Owner_or_special - 1 + 370; }
            set { Owner_or_special = (byte) (value + 1 - 370); }
        }

        // TODO: Revise all these "Is..." functions. Would they be needed by the GameObjectFactory?
        public static bool IsTexturedObject(byte[] buffer)
        {
            byte firstByte = buffer[0];
            int start = 0x160; // TODO: I haven't checked these indices thoroughly!
            int end = 0x17f;   // TODO: I haven't checked these indices thoroughly!
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

            if (this.GetType() != other.GetType())
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

        public virtual string SaveBuffer(string? basePath = null, string filename = "")
        {
            if (basePath is null)
            {
                basePath = Settings.DefaultBinaryTestsPath;
            }
            return StdSaveBuffer(Buffer, basePath, filename.Length == 0 ? $@"_GameObject_{IdxAtObjectArray}" : filename);
        }
    }
}