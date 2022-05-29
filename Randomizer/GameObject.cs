using System.Diagnostics;
using static Randomizer.Utils;

// TODO: Maybe make a different class of Container objects?
// TODO: What about empty entries? And the null entry (id = 0)?
// TODO: Add checks that prevent modification if id=0.
namespace Randomizer
{
    public class GameObject
    {
        public const int InfoSize = 2;
        public const int InfoNum = 4;
        public const int BaseLength = InfoNum * InfoSize;
        public const int ExtraLength = 0;
        public const int TotalLength = BaseLength + ExtraLength;

        public byte[] Buffer = new byte[TotalLength];
        public short[] GeneralInfo = new short[InfoNum] { 0, 0, 0, 0 };

        public bool HasTexture = false;
        protected short link_specialField;
        protected short objid_flagsField;
        protected short positionField;
        protected short quality_chainField;

        protected GameObject() { }

        // todo: Rider is complaining about the virtual methods. Check if this will be negatively affected here.
        // If not, then just re-implement the methods in this and the derived classes.
        public GameObject(byte[] buffer)
        {
            Debug.Assert(buffer.Length == TotalLength);
            this.Buffer = buffer;
            UpdateEntries();
            // this.objid_flagsField = BitConverter.ToInt16(buffer, 0);
            // this.positionField = BitConverter.ToInt16(buffer, 2);
            // this.quality_chainField = BitConverter.ToInt16(buffer, 4);
            // this.link_specialField = BitConverter.ToInt16(buffer, 6);
        }

        public GameObject(short objid_flagsField, short positionField, short quality_chainField,
            short link_specialField)
        {
            this.objid_flagsField = objid_flagsField;
            this.positionField = positionField;
            this.quality_chainField = quality_chainField;
            this.link_specialField = link_specialField;
            UpdateBuffer();
        }

        public void UpdateEntries()
        {
            this.objid_flagsField = BitConverter.ToInt16(Buffer, 0);
            this.positionField = BitConverter.ToInt16(Buffer, 2);
            this.quality_chainField = BitConverter.ToInt16(Buffer, 4);
            this.link_specialField = BitConverter.ToInt16(Buffer, 6);
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
            set { objid_flagsField = (short)SetBits(objid_flagsField, value, 0b111111111, 0); UpdateBuffer(); }
        }

        public int Flags
        {
            get
            {
                // todo: error in uw-formats.txt? Check hank's. -- He uses a mask with 3 bits, so I changed here.
                // return GetBits(objid_flagsField, 0b1111, 9);
                return GetBits(objid_flagsField, 0b111, 9);
            }
            set { objid_flagsField = (short)SetBits(objid_flagsField, value, 0b1111, 9); UpdateBuffer(); }
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
            set { this.positionField = (short)SetBits(positionField, value, 0b1111111, 0); UpdateBuffer(); }
        }

        public byte Heading
        {
            // get { return (byte) ((positionField >> 7) & 0b111); }
            get { return (byte)GetBits(positionField, 0b111, 7); }
            set { positionField = (short)SetBits(positionField, value, 0b111, 7); UpdateBuffer(); }
        }

        public byte Ypos
        {
            // get { return (byte) ((positionField >> 10) & 0b111); }
            get { return (byte)GetBits(positionField, 0b111, 10); }
            set { positionField = (short)SetBits(positionField, value, 0b111, 10); UpdateBuffer(); }
        }

        public byte Xpos
        {
            // get { return (byte) ((positionField >> 13) & 0b111); }
            get { return (byte)GetBits(positionField, 0b111, 13); }
            set { positionField = (short)SetBits(positionField, value, 0b111, 13); UpdateBuffer(); }
        }
        #endregion
        #region Third Short
        public byte Quality
        {
            // get { return (byte) (quality_chainField & 0b111111); }
            get { return (byte)GetBits(quality_chainField, 0b111111, 0); }
            set { quality_chainField = (short)SetBits(quality_chainField, value, 0b111111, 0); UpdateBuffer(); }
        }

        public byte next
        {
            // get { return (byte) ((quality_chainField >> 6) & 0b1111111111); }
            get { return (byte)GetBits(quality_chainField, 0b1111111111, 6); }
            set { quality_chainField = (short)SetBits(quality_chainField, value, 0b1111111111, 6); UpdateBuffer(); }
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
            set { link_specialField = (short)SetBits(link_specialField, value, 0b111111, 0); UpdateBuffer(); }
        }

        public short QuantityOrSpecialLinkOrSpecialProperty
        {
            // get { return (byte) ((link_specialField >> 6) & 0b1111111111); }
            get { return (byte)GetBits(link_specialField, 0b1111111111, 6); }
            set { link_specialField = (short)SetBits(link_specialField, value, 0b1111111111, 6); UpdateBuffer(); }
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
    }

    public class TexturedGameObject : GameObject
    {
        public new bool HasTexture = true;
        public new readonly int? Flags = null;
        public new readonly int? EnchantFlag = null;
        public new readonly int? Doordir = null;
        public new readonly int? Invis = null;
        public new readonly int? IsQuant = null;

        public int TextureNumber
        {
            get { return GetBits(objid_flagsField, 0b1111111, 9); }
            set { objid_flagsField = (short)SetBits(objid_flagsField, value, 0b1111111, 9); }
        }

        // public new void UpdateBuffer()
        // {
        //     base.UpdateBuffer(); // todo: recheck that this is working as intended.
        // }
    }

    // is_quant is true, quantity < 512 (coins, etc)
    public class QuantityGameObject : GameObject
    {
        public short Quantity
        {
            // get { return (byte) ((link_specialField >> 6) & 0b1111111111); }
            get { return QuantityOrSpecialLinkOrSpecialProperty; }
            set
            {
                if (value > 512)
                {
                    throw new Exception("Cannot have a Quantity Game Object with quantity > 512");
                }
                link_specialField = (short)SetBits(link_specialField, value, 0b1111111111, 6); UpdateBuffer();
            }
        }
    }

    // is_quant is true, quantity > 512 (special property)
    public class SpecialPropertyGameObject : GameObject
    {
        public short RawSpecialLink
        {
            get { return Convert.ToInt16(SpecialLink + 512); }
            set { SpecialLink = Convert.ToInt16(value + 512); }
        }
        public short SpecialLink
        {
            // get { return (byte) ((link_specialField >> 6) & 0b1111111111); }
            get { return Convert.ToInt16(QuantityOrSpecialLinkOrSpecialProperty - 512); }
            set
            {
                if (value < 512)
                {
                    throw new Exception("Cannot have a SpecialLink with value < 512");
                }
                link_specialField = (short)SetBits(link_specialField, value, 0b1111111111, 6); UpdateBuffer();
            }
        }
    }

    // is_quant is false. Enchantments, wands, etc
    public class SpecialLinkGameObject : GameObject
    {
        public new readonly int IsQuant = 0;
        public short SpecialIdx
        {
            get { return QuantityOrSpecialLinkOrSpecialProperty; }
            set { link_specialField = (short)SetBits(link_specialField, value, 0b1111111111, 6); UpdateBuffer(); }
        }
    }

    // Todo: Get enchantment name in strings chunk 5
    // Todo: create an enum or something with spell names, and their indices, to use here.
    public class EnchantedObject : SpecialLinkGameObject
    {
        public new readonly int EnchantFlag = 1;

        public int Enchantment
        {
            get { return SpecialIdx - 512; }
            set { SpecialIdx = (short) (value + 512); }
        }

        public int Spell
        {
            get
            {
                if ((Enchantment >= 0) | (Enchantment <= 63))
                    return Enchantment + 256;
                else if (ItemID == 0x01c9) // a fountain
                    return Enchantment;
                else
                    return Enchantment + 144;
            }
            set
            {
                if ((Enchantment >= 0) | (Enchantment <= 63))
                    Enchantment = value - 256;
                else if (ItemID == 0x01c9) // a fountain
                    Enchantment = value;
                else
                    Enchantment = value - 144;
            } // todo: these will UpdateBuffer too right?
        }

        public EnchantedObject()
        {
            throw new NotImplementedException();
        }
        
    }

    public class EnchantedWeapon : SpecialLinkGameObject
    {
        
        public new readonly int EnchantFlag = 1;

        public int Enchantment
        {
            get { return SpecialIdx - 512; }
            set { SpecialIdx = (short) (value + 512); }
        }

        // Oh boy. This is more complicated. Need to have logic to differentiate between Acc/Dam/Prot/Tough and other spells
        public int Spell
        {
            get
            {
                return Enchantment + 256;
            }
            set
            {
                Enchantment = value - 256;
            } // todo: these will UpdateBuffer too right?
        }

        public EnchantedWeapon()
        {
            throw new NotImplementedException();
        }
        
    }

    public class EnchantedArmor : SpecialLinkGameObject
    {
        public new readonly int EnchantFlag = 1;
        public int Enchantment
        {
            get { return SpecialIdx - 512; }
            set { SpecialIdx = (short) (value + 512); }
        }

        // Oh boy. This is more complicated. Need to have logic to differentiate between Acc/Dam/Prot/Tough and other spells
        public int Spell
        {
            get { return Enchantment + 256 + 16; }
            set { Enchantment = value - 256 - 16; } // todo: these will UpdateBuffer too right?
        }

        public EnchantedArmor()
        {
            throw new NotImplementedException();
        }
    }

    public class EnchantedWand : SpecialLinkGameObject
    {
        public new readonly int EnchantFlag = 1;

        public int SpellObjectLink
        {
            get { return SpecialIdx; }
            set { SpecialIdx = (short) value; }
        }

        public GameObject SpellObject;

        public EnchantedWand()
        {
            throw new NotImplementedException();
        }

    }

    public class FreeListObjectEntry
    {
        // FreeListMobileObjectEntrySize is also 4 (short).
        public const int EntrySize = TileMapMasterObjectListBlock.FreeListStaticObjectsEntrySize;
        public byte[] Buffer = new byte[EntrySize];
        public short Entry;
        public int EntryNum;

        public FreeListObjectEntry(byte[] buffer, int EntryNum)
        {
            Debug.Assert(buffer.Length == TileMapMasterObjectListBlock.FreeListMobileObjectsEntrySize);
            this.Buffer = buffer;
            this.EntryNum = EntryNum;
            UpdateEntry();
        }

        public FreeListObjectEntry(short Entry, int EntryNum)
        {
            this.Entry = Entry;
            this.EntryNum = EntryNum;
            UpdateBuffer();
        }

        public FreeListObjectEntry() // For non-entries.
        { }

        public void UpdateBuffer()
        {
            Buffer = BitConverter.GetBytes(Entry);
        }

        public void UpdateEntry()
        {
            Entry = BitConverter.ToInt16(Buffer);
        }
    }


}
