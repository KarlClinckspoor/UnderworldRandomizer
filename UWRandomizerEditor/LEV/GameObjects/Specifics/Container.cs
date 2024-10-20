namespace UWRandomizerEditor.LEVdotARK.GameObjects.Specifics;

public class Container: SpecialLinkGameObject, IContainer
{
    public Container(byte[] buffer, ushort idx) : base(buffer, idx)
    {
        Contents = new UWLinkedList() {StartingIdx = QuantityOrSpecialLinkOrSpecialProperty, RepresentingContainer = true};
    }

    public Container(ushort objIdFlags, ushort position, ushort qualityChain,
        ushort linkSpecial, ushort idxAtObjectArray) : base(objIdFlags, position, qualityChain, linkSpecial, idxAtObjectArray)
    {
        Contents = new UWLinkedList() {StartingIdx = QuantityOrSpecialLinkOrSpecialProperty, RepresentingContainer = true};
    }

    public UWLinkedList Contents { get; set; }

    public override bool ReconstructBuffer()
    {
        // Workaround to avoid infinite loop. TODO: Fix this
        LinkSpecial = (ushort) Utils.SetBits(LinkSpecial, Contents.StartingIdx, 0b11_1111_1111, 6);
        return base.ReconstructBuffer();
    }
}