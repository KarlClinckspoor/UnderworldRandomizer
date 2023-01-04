namespace UWRandomizerEditor.LEVDotARK.GameObjects.Specifics;

// TODO: special link or special property?
public class Trigger : SpecialLinkGameObject
{
    public override bool ShouldBeMoved { get; set; } = false;

    public Trigger(byte[] buffer, ushort IdxAtObjArray) : base(buffer, IdxAtObjArray)
    {
    }
}