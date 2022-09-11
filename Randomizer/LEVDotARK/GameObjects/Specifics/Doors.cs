namespace Randomizer.LEVDotARK.GameObjects.Specifics;

public class Door: SpecialLinkGameObject
{
    public Door(byte[] buffer, short idxAtObjArray): base(buffer, idxAtObjArray) {}
    
    public new bool ShouldBeMoved
    {
        get { return false;}
        set { }
    }
}