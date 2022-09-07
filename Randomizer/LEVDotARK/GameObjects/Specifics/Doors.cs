namespace Randomizer.LEVDotARK.GameObjects.Specifics;

public class Door: SpecialLinkGameObject
{
    public new bool ShouldBeMoved = false;
    public Door(byte[] buffer, short idxAtObjArray): base(buffer, idxAtObjArray) {}
}