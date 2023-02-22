namespace UWRandomizerEditor.LEVdotARK.GameObjects;

public class StaticObject : GameObject
{
    public StaticObject(byte[] buffer, ushort idxAtObjArray) : base(buffer, idxAtObjArray)
    {
    }

    public StaticObject(ushort objIdFlags, ushort position, ushort qualityChain, ushort linkSpecial)
        : base(objIdFlags, position, qualityChain, linkSpecial)
    {
    }

    protected StaticObject() : base()
    {
    }
}