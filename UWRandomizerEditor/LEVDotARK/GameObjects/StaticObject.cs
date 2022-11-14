namespace UWRandomizerEditor.LEVDotARK.GameObjects;

public class StaticObject: GameObject
{
    public StaticObject(byte[] buffer, short idxAtObjArray) : base(buffer, idxAtObjArray) { }

    public StaticObject(ushort objid_flagsField, ushort positionField, ushort quality_chainField, ushort link_specialField)
        : base(objid_flagsField, positionField, quality_chainField, link_specialField)
    { }

    public StaticObject() : base()
    {
    }
}