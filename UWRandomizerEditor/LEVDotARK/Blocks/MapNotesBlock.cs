namespace UWRandomizerEditor.LEVDotARK.Blocks
{
    /// <summary>
    /// Block that stores map notes. Its length is 0 if there are no block notes
    /// </summary>
    public class MapNotesBlock : Block
    {
        public MapNotesBlock(byte[] buffer, int levelnumber) : base(buffer, levelnumber)
        {
        }
    }
}