namespace UWRandomizerEditor.LEVDotARK.GameObjects.Specifics;

public class Container: SpecialLinkGameObject
{
    public Container(byte[] buffer, ushort idx) : base(buffer, idx)
    {
        Contents = new UWLinkedList() {startingIdx = QuantityOrSpecialLinkOrSpecialProperty};
    }

    public Container(ushort objIdFlags, ushort position, ushort qualityChain,
        ushort linkSpecial) : base(objIdFlags, position, qualityChain, linkSpecial)
    {
        Contents = new UWLinkedList() {startingIdx = QuantityOrSpecialLinkOrSpecialProperty, RepresentingContainer = true};
    }

    public UWLinkedList Contents { get; }

    public override bool ReconstructBuffer()
    {
        QuantityOrSpecialLinkOrSpecialProperty = (ushort) Contents.startingIdx;
        return base.ReconstructBuffer();
    }
}