namespace UWRandomizerEditor.LEVDotARK.GameObjects.Specifics;

public class Container: SpecialLinkGameObject
{
    public Container(byte[] buffer, ushort idx) : base(buffer, idx)
    {
    }

    public Container(ushort objIdFlags, ushort position, ushort qualityChain,
        ushort linkSpecial) : base(objIdFlags, position, qualityChain, linkSpecial)
    {
    }
}