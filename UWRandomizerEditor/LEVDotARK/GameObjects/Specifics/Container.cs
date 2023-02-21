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

    public UWLinkedList Contents { get; set; }

    public override bool ReconstructBuffer()
    {
        // Workaround to avoid infinite loop. TODO: Fix this
        LinkSpecial = (ushort) Utils.SetBits(LinkSpecial, Contents.startingIdx, 0b11_1111_1111, 6);
        base.ReconstructBuffer();
        return base.ReconstructBuffer();
    }
}