namespace UWRandomizerEditor.LEV.GameObjects.Specifics;

public class Countable: StaticObject
{
    public Countable(byte[] buffer, ushort idx) : base(buffer, idx)
    {
    }

    public Countable(ushort objIdFlags, ushort position, ushort qualityChain,
        ushort linkSpecial, ushort idxAtObjectArray) : base(objIdFlags, position, qualityChain, linkSpecial, idxAtObjectArray)
    {
    }
}