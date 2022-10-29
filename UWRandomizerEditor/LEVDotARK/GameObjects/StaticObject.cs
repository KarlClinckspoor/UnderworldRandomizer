namespace UWRandomizerEditor.LEVDotARK.GameObjects;

public class StaticObject: GameObject
{
    public StaticObject(byte[] buffer, short idxAtObjArray) : base(buffer, idxAtObjArray) { }

    public StaticObject(short objid_flagsField, short positionField, short quality_chainField, short link_specialField)
        : base(objid_flagsField, positionField, quality_chainField, link_specialField)
    {
    }

    public StaticObject() : base()
    {
    }
}