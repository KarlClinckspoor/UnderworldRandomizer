namespace UWRandomizerEditor.LEVDotARK.GameObjects.Specifics;

// Should this inherit from textured game object?
public class Furniture: StaticObject
{
    public Furniture(byte[] buffer, short idxAtObjArray) : base(buffer, idxAtObjArray)
    { }

    public override bool ShouldBeMoved { get; set; } = false;
}