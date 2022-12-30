namespace UWRandomizerEditor.LEVDotARK.GameObjects;

public class TexturedGameObject : StaticObject
{
    public new bool HasTexture = true;
    public new readonly int? Flags = null;
    public new readonly int? EnchantFlag = null;
    public new readonly int? Doordir = null;
    public new readonly int? Invis = null;
    public new readonly int? IsQuant = null;

    public override bool ShouldBeMoved { get; set; } = false;

    public int TextureNumber
    {
        get { return Utils.GetBits(objid_flagsField, 0b1111111, 9); }
        set { objid_flagsField = (ushort) Utils.SetBits(objid_flagsField, value, 0b1111111, 9); }
    }

    public TexturedGameObject(byte[] buffer, short idx) : base(buffer, idx)
    {
    }

    public TexturedGameObject(ushort objid_flagsField, ushort positionField, ushort quality_chainField,
        ushort link_specialField) : base(objid_flagsField, positionField, quality_chainField, link_specialField)
    {
    }
}