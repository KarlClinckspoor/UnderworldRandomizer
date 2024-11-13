namespace UWRandomizerEditor.LEV.GameObjects.Specifics;

public class VerticalTexture: StaticObject
{
    public VerticalTexture(byte[] buffer, ushort idx) : base(buffer, idx)
    {
    }

    public VerticalTexture(ushort objIdFlags, ushort position, ushort qualityChain,
        ushort linkSpecial, ushort idxAtObjectArray) : base(objIdFlags, position, qualityChain, linkSpecial, idxAtObjectArray)
    {
    }
}