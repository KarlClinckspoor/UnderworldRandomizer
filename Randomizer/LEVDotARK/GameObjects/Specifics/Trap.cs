namespace Randomizer.LEVDotARK.GameObjects.Specifics;

public class Trap: StaticObject
{
    public new bool ShouldBeMoved = false;
    public Trap(byte[] buffer, short idxAtObjArray) : base(buffer, idxAtObjArray)
    { }
}