using System.ComponentModel.Design.Serialization;

namespace Randomizer.LEVDotARK.GameObjects.Specifics;

// Should this inherit from textured game object?
public class Furniture: StaticObject
{
    public new bool ShouldBeMoved = false;
    public Furniture(byte[] buffer, short idxAtObjArray) : base(buffer, idxAtObjArray)
    {
    }
}