using System.ComponentModel.Design.Serialization;

namespace Randomizer.LEVDotARK.GameObjects.Specifics;

// TODO: special link or special property?
public class Trigger: SpecialLinkGameObject
{
    public bool ShouldBeMoved
    {
        get { return false; }
        set { }
    }
    public Trigger(byte[] buffer, short IdxAtObjArray) : base(buffer, IdxAtObjArray)
    {
    }
}