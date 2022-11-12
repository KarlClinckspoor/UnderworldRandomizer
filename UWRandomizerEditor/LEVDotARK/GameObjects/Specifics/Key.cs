namespace UWRandomizerEditor.LEVDotARK.GameObjects.Specifics;

public class Key: StaticObject
{
    public Key(byte[] buffer, short idxAtObjArray) : base(buffer, idxAtObjArray) { }
    
    public Key(ushort objid_flagsField, ushort positionField, ushort quality_chainField, ushort link_specialField)
        : base(objid_flagsField, positionField, quality_chainField, link_specialField)
    { }

    public Key() : base()
    { }

    public byte KeyID
    {
        get
        {
            return Owner_or_special;
        }
        set
        {
            if (value >= 0b1000000) // This should only have 6 bits in length 
                throw new InvalidDataException("Invalid range for key ID. Must fit in 6 bits (<=63)");
            Owner_or_special = value;
        }
    }

    public bool FitsLock(Lock LockObject)
    {
        if (KeyID == LockObject.KeyID)
        {
            return true;
        }

        return false;
    }
}