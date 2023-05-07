namespace UWRandomizerEditor.LEVdotARK.GameObjects;

/// <summary>
/// This is essentially the same as GameObject, but I decided to have it separate for semantic reasons.
/// </summary>
public class StaticObject : GameObject
{
    public StaticObject(byte[] buffer, ushort idxAtObjArray) : base(buffer, idxAtObjArray)
    {
    }

    public StaticObject(ushort objIdFlags, ushort position, ushort qualityChain, ushort linkSpecial, ushort idxAtObjectArray)
        : base(objIdFlags, position, qualityChain, linkSpecial, idxAtObjectArray) { }
    
    protected StaticObject() { }
}