namespace UWRandomizerEditor.LEVDotARK.GameObjects.Specifics;

public class Trap: StaticObject
{
    public override bool ShouldBeMoved { get; set; } = false;
    public Trap(byte[] buffer, short idxAtObjArray) : base(buffer, idxAtObjArray)
    { }
}