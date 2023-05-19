// See https://aka.ms/new-console-template for more information

using InfoExtraction;
using Randomizer;

class Program
{
    public static void Main(string[] args)
    {
        string arkpath;
        string binarypath;
        
        if (args.Length == 0)
        {
            arkpath = Settings.DefaultArkPath;
            binarypath = Settings.DefaultBinaryTestsPath;
        }
        else if (args.Length == 1)
        {
            arkpath = args[0];
            binarypath = Settings.DefaultBinaryTestsPath;
        }
        else
        {
            arkpath = args[0];
            binarypath = args[1];
        }
        Console.WriteLine(arkpath);
        Console.WriteLine($"Loading LEV.ARK from {arkpath}");
        var extractor = new LoadAndCreateBufferExamples(arkpath, binarypath);
        Console.WriteLine("Saving generalized blocks");
        extractor.ExtractBlocks();
        Console.WriteLine("Saving Tilemap and object data");
        extractor.ExtractTileMapObjectBlocks();
        Console.WriteLine("Saving ObjAnimBlocks");
        extractor.ExtractObjAnimBlocks();
        Console.WriteLine("Saving TextMapBlocks");
        extractor.ExtractTextMapBlocks();
        Console.WriteLine("Saving AutomapBlocks");
        extractor.ExtractAutomapBlocks();
        Console.WriteLine("Saving MapNotesBlocks");
        extractor.ExtractMapNotesBlocks();
        
        Console.WriteLine("Saving each tile");
        extractor.ExtractTiles();
    }
}
