namespace UWRandomizerEditor.LEVdotARK.GameObjects.Specifics;

public class Key : StaticObject
{
    public Key(byte[] buffer, ushort idxAtObjArray) : base(buffer, idxAtObjArray)
    {
    }

    public Key(ushort objIdFlags, ushort position, ushort qualityChain, ushort linkSpecial, ushort idxAtObjectArray)
        : base(objIdFlags, position, qualityChain, linkSpecial, idxAtObjectArray)
    {
    }

    public Key()
    {
    }

    public byte KeyID
    {
        get => OwnerOrSpecial;
        set
        {
            if (value >= 0b1000000) // This should only have 6 bits in length 
                throw new InvalidDataException("Invalid range for key ID. Must fit in 6 bits (<=63)");
            OwnerOrSpecial = value;
        }
    }

    public bool FitsLock(Lock lockObject)
    {
        return KeyID == lockObject.KeyID;
    }
}