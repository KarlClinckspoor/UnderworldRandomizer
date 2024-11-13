namespace UWRandomizerEditor.LEV.GameObjects.Specifics;

public class Trap : SpecialLinkGameObject
{
    public Trap(byte[] buffer, ushort idxAtObjArray) : base(buffer, idxAtObjArray)
    {
    }
}

public class DoorTrap : Trap
{
    public DoorTrap(byte[] buffer, ushort idxAtObjArray) : base(buffer, idxAtObjArray)
    {
    }
}