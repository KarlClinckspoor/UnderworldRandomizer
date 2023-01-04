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
        get { return Utils.GetBits(ObjIdFlags, 0b1111111, 9); }
        set { ObjIdFlags = (ushort) Utils.SetBits(ObjIdFlags, value, 0b1111111, 9); }
    }

    public TexturedGameObject(byte[] buffer, ushort idx) : base(buffer, idx)
    {
    }

    public TexturedGameObject(ushort objIdFlags, ushort position, ushort qualityChain,
        ushort linkSpecial) : base(objIdFlags, position, qualityChain, linkSpecial)
    {
    }
}