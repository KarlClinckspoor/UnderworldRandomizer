using System.ComponentModel.Design.Serialization;

namespace Randomizer.LEVDotARK.GameObjects.Specifics;

// Should this inherit from textured game object?
public class Furniture: StaticObject
{
    public Furniture(byte[] buffer, short idxAtObjArray) : base(buffer, idxAtObjArray)
    { }

    public bool ShouldBeMoved
    {
        get { return false; }
        set { }
    }
}