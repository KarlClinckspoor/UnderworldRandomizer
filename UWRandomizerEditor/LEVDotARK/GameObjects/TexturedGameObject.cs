namespace UWRandomizerEditor.LEVdotARK.GameObjects;

public class TexturedGameObject : StaticObject
{
    public int TextureNumber
    {
        get => Utils.GetBits(ObjIdFlags, 0b1111111, 9);
        set => ObjIdFlags = (ushort) Utils.SetBits(ObjIdFlags, value, 0b1111111, 9);
    }

    public TexturedGameObject(byte[] buffer, ushort idx) : base(buffer, idx)
    {
    }

    public TexturedGameObject(ushort objIdFlags, ushort position, ushort qualityChain,
        ushort linkSpecial, ushort idxAtObjectArray) : base(objIdFlags, position, qualityChain, linkSpecial, idxAtObjectArray)
    {
    }
}