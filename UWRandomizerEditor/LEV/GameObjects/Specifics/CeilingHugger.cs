namespace UWRandomizerEditor.LEV.GameObjects.Specifics;

public class CeilingHugger: StaticObject
{
    public CeilingHugger(byte[] buffer, ushort idxAtObjArray) : base(buffer, idxAtObjArray)
    {
    }

    public CeilingHugger(ushort objIdFlags, ushort position, ushort qualityChain, ushort linkSpecial, ushort idxAtObjectArray) : base(objIdFlags, position, qualityChain, linkSpecial, idxAtObjectArray)
    {
    }
}