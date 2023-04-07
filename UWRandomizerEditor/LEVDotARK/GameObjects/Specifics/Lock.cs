namespace UWRandomizerEditor.LEVdotARK.GameObjects.Specifics;

public class Lock : StaticObject
{
    public Lock(byte[] buffer, ushort idxAtObjArray) : base(buffer, idxAtObjArray)
    {
    }

    public Lock(ushort objIdFlags, ushort position, ushort qualityChain, ushort linkSpecial)
        : base(objIdFlags, position, qualityChain, linkSpecial)
    {
    }

    public Lock()
    {
    }

    public bool IsLocked
    {
        get { return Utils.GetBits(ObjIdFlags, 0b1, 9) == 1; }
        set { ObjIdFlags = (ushort) Utils.SetBits(ObjIdFlags, value ? 1 : 0, 0b1, 9); }
    }

    public byte KeyID
    {
        get { return (byte) Utils.GetBits(LinkSpecial, 0b111111, 0); }
        set
        {
            if (value >= 0b1000000) // This should only have 6 bits in length 
                throw new InvalidDataException("Invalid range for key ID. Must fit in 6 bits (<=63)");
            LinkSpecial = (byte) Utils.SetBits(LinkSpecial, value, 0b111111, 0);
        }
    }

    public bool IsOpenedBy(Key KeyObject)
    {
        return KeyID == KeyObject.KeyID;
    }
}