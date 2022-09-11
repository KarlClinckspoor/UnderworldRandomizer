namespace Randomizer.LEVDotARK.GameObjects.Specifics;

public class Trap: StaticObject
{
    public bool ShouldBeMoved
    {
        get { return false; }
        set { }
    }
    public Trap(byte[] buffer, short idxAtObjArray) : base(buffer, idxAtObjArray)
    { }
}