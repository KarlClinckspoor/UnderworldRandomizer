namespace UWRandomizerEditor.LEVDotARK.Blocks
{
    /// <summary>
    /// Block that stores map notes. Its length is 0 if there are no block notes
    /// </summary>
    public class MapNotesBlock : Block
    {
        public MapNotesBlock(byte[] buffer, int levelnumber)
        {
            Buffer = new byte[buffer
                .Length]; // This is getting the length from the input buffer because the length of this block is variable
            buffer.CopyTo(Buffer, 0);
            LevelNumber = levelnumber;
        }

        public override bool ReconstructBuffer()
        {
            // Since there's no operations that can change the buffer, this won't do anything.
            return true;
        }
    }
}