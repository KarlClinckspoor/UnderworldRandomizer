namespace UWRandomizerEditor.LEVDotARK.Blocks
{
    public class EmptyBlock : Block
    {
        public override bool ReconstructBuffer()
        {
            Buffer = Array.Empty<byte>();
            return true;
        }
    }
}