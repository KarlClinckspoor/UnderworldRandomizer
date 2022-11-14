namespace UWRandomizerEditor.LEVDotARK.GameObjects.Specifics;

using static UWRandomizerEditor.Utils;

public class Lock : StaticObject
{
    public Lock(byte[] buffer, short idxAtObjArray) : base(buffer, idxAtObjArray)
    {
    }

    public Lock(ushort objid_flagsField, ushort positionField, ushort quality_chainField, ushort link_specialField)
        : base(objid_flagsField, positionField, quality_chainField, link_specialField)
    {
    }

    public Lock() : base()
    {
    }

    public bool IsLocked
    {
        get { return GetBits(objid_flagsField, 0b1, 9) == 1; }
        set { objid_flagsField = (ushort) SetBits(objid_flagsField, value ? 1 : 0, 0b1, 9); }
    }

    public byte KeyID
    {
        get { return (byte) GetBits(link_specialField, 0b111111, 0); }
        set
        {
            if (value >= 0b1000000) // This should only have 6 bits in length 
                throw new InvalidDataException("Invalid range for key ID. Must fit in 6 bits (<=63)");
            link_specialField = (byte) SetBits(link_specialField, value, 0b111111, 0);
        }
    }

    public bool IsOpenedBy(Key KeyObject)
    {
        return KeyID == KeyObject.KeyID;
    }
}