namespace UWRandomizerEditor.LEVDotARK.Blocks
{
    /// <summary>
    /// Block that stores map notes. Its length is 0 if there are no block notes
    /// </summary>
    public class MapNotesBlock: Block
    {
        public MapNotesBlock(byte[] buffer, int levelnumber): base(buffer, levelnumber)
        { }
        public override string SaveBuffer(string? basePath = null, string filename = "")
        {
            return base.SaveBuffer(basePath, filename.Length == 0 ? $@"_MAPNOTES_{LevelNumber}" : filename);
        }
    }
}
