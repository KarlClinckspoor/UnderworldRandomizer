namespace UWRandomizerEditor.LEVdotARK.GameObjects.Specifics;

public class Key : StaticObject
{
    public Key(byte[] buffer, ushort idxAtObjArray) : base(buffer, idxAtObjArray)
    {
    }

    public Key(ushort objIdFlags, ushort position, ushort qualityChain, ushort linkSpecial)
        : base(objIdFlags, position, qualityChain, linkSpecial)
    {
    }

    public Key()
    {
    }

    public byte KeyID
    {
        get { return OwnerOrSpecial; }
        set
        {
            if (value >= 0b1000000) // This should only have 6 bits in length 
                throw new InvalidDataException("Invalid range for key ID. Must fit in 6 bits (<=63)");
            OwnerOrSpecial = value;
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