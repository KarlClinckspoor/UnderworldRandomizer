using UWRandomizerEditor.LEVdotARK.Blocks;

namespace UWRandomizerEditor.LEVdotARK;

/// <summary>
/// Static class used to reshape, print and translate tile info buffers into more human readable formats.
/// </summary>
public static class TileMap
{
    static readonly IDictionary<int, string> TileTypeDescriptors = new Dictionary<int, string>()
    {
        {0, "#SOLID"},
        {1, "#OPEN"},
        {2, "#DIAG_SE"},
        {3, "#DIAG_SW"},
        {4, "#DIAG_NE"},
        {5, "#DIAG_NW"},
        {6, "#SLP_N"},
        {7, "#SLP_S"},
        {8, "#SLP_E"},
        {9, "#SLP_W"}
    };

    static readonly IDictionary<int, char> TileCharReplacements = new Dictionary<int, char>()
    {
        {0, '#'},
        {1, '_'},
        {2, '/'},
        {3, '\\'},
        {4, '\\'},
        {5, '/'},
        {6, '^'},
        {7, 'v'},
        {8, '>'},
        {9, '<'}
    };

    /// <summary>
    /// Converts a 4096 long array of tile numbers (0-9) into a 64x64 array of tile numbers. Can technically have other shapes,
    /// but depends on TileMapMasterObjectListBlock.TileHeight and TileWidth, which are const 64.
    /// </summary>
    /// <param name="TileNumbers">4096 long array of tile numbers.</param>
    /// <returns>Reshaped [64,64] array of numbers </returns>
    static int[,] ReshapeTileNumbers(int[] TileNumbers)
    {
        return Utils.ReshapeArray(TileNumbers, TileMapMasterObjectListBlock.TileHeight,
            TileMapMasterObjectListBlock.TileWidth);
    }

    /// <summary>
    /// Prints, from bottom to top, the tile numbers. Should output something human readable
    /// </summary>
    /// <param name="TileNumbers">64x64 array of tile numbers</param>
    static void PrintReshapedTiles(int[,] TileNumbers)
    {
        for (int i = TileMapMasterObjectListBlock.TileHeight - 1; i > 0; i--) // Start is lower left
        {
            for (int j = 0; j < TileMapMasterObjectListBlock.TileWidth; j++) // Cols work as normal
            {
                Console.Write(TileNumbers[i, j].ToString());
            }

            Console.Write("\n");
        }
    }

    /// <summary>
    /// Prints, from bottom to top, the tile characters. Should output something human readable.
    /// </summary>
    /// <param name="TileChars">64x64 array of tile characters</param>
    static void PrintReshapedTiles(char[,] TileChars)
    {
        for (int i = TileMapMasterObjectListBlock.TileHeight - 1; i > 0; i--) // Start is lower left
        {
            for (int j = 0; j < TileMapMasterObjectListBlock.TileWidth; j++) // Cols work as normal
            {
                Console.Write(TileChars[i, j]);
            }

            Console.Write("\n");
        }
    }

    /// <summary>
    /// Writes to a text file the tile numbers of a specific block. Writes a .txt file with the block number appended.
    /// </summary>
    /// <param name="TileNumbers">64x64 array of tile numbers</param>
    /// <param name="blocknum">block number that generated the tile numbers</param>
    /// <param name="basepath">string containing the start of the path and name of the file</param>
    static void SaveReshapedTiles(int[,] TileNumbers, int blocknum,
        string basepath = @"D:\Dropbox\UnderworldStudy\studies\tilenumbers_ints")
    {
        using StreamWriter sw = new($"{basepath}_${blocknum}.txt");

        for (int i = TileMapMasterObjectListBlock.TileHeight - 1; i > 0; i--) // Start is lower left
        {
            for (int j = 0; j < TileMapMasterObjectListBlock.TileWidth; j++) // Cols work as normal
            {
                sw.Write(TileNumbers[i, j].ToString());
            }

            sw.Write('\n');
        }

        sw.Close();
    }

    /// <summary>
    /// Writes to a text file the tile characters of a specific block. Writes a .txt file with the block number appended.
    /// </summary>
    /// <param name="TileNumbers">64x64 array of tile numbers</param>
    /// <param name="blocknum">block number that generated the tile numbers</param>
    /// <param name="basepath">string containing the start of the path and name of the file</param>
    static void SaveReshapedTiles(char[,] TileNumbers, int blocknum,
        string basepath = @"D:\Dropbox\UnderworldStudy\studies\tilenumbers_chars")
    {
        using StreamWriter sw = new($"{basepath}_${blocknum}.txt");

        for (int i = TileMapMasterObjectListBlock.TileHeight - 1; i > 0; i--) // Start is lower left
        {
            for (int j = 0; j < TileMapMasterObjectListBlock.TileWidth; j++)
            {
                sw.Write(TileNumbers[i, j]);
            }

            sw.Write('\n');
        }

        sw.Close();
    }
}