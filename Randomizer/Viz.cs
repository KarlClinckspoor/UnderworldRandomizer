namespace Randomizer
{
    // Help visualization
    public class TileMap
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

        static int[,] ReshapeTileNumbers(int[] TileNumbers)
        {
            int[,] array = new int[TileMapMasterObjectListBlock.TileHeight, TileMapMasterObjectListBlock.TileWidth];
            for (int i = 0; i < TileNumbers.Length; i++)
            {
                int col = i % TileMapMasterObjectListBlock.TileWidth;
                int row = i / TileMapMasterObjectListBlock.TileHeight;
                array[row, col] = TileNumbers[i];
            }

            return array;
        }

        // TODO: Remove this function, get replacements directly. Reduce function number below by half.
        static char[,] ReshapeReplaceTileNumbers(int[] TileNumbers)
        {
            char[,] array = new char[TileMapMasterObjectListBlock.TileHeight, TileMapMasterObjectListBlock.TileWidth];
            for (int i = 0; i < TileNumbers.Length; i++)
            {
                int col = i % TileMapMasterObjectListBlock.TileWidth;
                int row = i / TileMapMasterObjectListBlock.TileHeight;
                array[row, col] = TileCharReplacements[TileNumbers[i]];
            }

            return array;
        }

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
}