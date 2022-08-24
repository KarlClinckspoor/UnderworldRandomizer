using Randomizer;
using System;

namespace InfoExtraction;

public class LoadAndCreateBufferExamples
{
    private ArkLoader Ark;
    private string pathToBinaries;
    private string pathToArk;

    private void WriteToFile(byte[] buffer, string filename)
    {
        using (var stream = new FileStream(filename, FileMode.Create))
        {
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(buffer);
            }
        }
    }

    public void ExtractTileMapObjectBlocks()
    {
        int i = 0;
        foreach (var block in Ark.TileMapObjectsBlocks)
        {
            Console.WriteLine($"Extracting TileMapObjectBlock {i+1}/{Ark.TileMapObjectsBlocks.Length}");
            string filename = Path.Combine(Settings.DefaultBinaryTestsPath, $"TileMapObjects{i:D3}.bin");
            WriteToFile(block.blockbuffer, filename);
            i++;
        }
    }

    public void ExtractTextMapBlocks()
    {
        foreach (var block in Ark.TextMapBlocks)
        {
            
        }
    }

    public void ExtractMapNotesBlocks()
    {
        foreach (var block in Ark.MapNotesBlocks)
        {
            
        }
    }

    public void ExtractAutomapBlocks()
    {
        foreach (var block in Ark.AutomapBlocks)
        {
            
        }
    }

    public void ExtractObjAnimBlocks()
    {
        foreach (var block in Ark.ObjAnimBlocks)
        {
            
        }
    }

    public void ExtractBlocks()
    {
        int i = 0;
        foreach (var block in Ark.blocks)
        {
            Console.WriteLine($"Extracting Block {i+1}/{Ark.blocks.Length}");
            string filename = Path.Combine(pathToBinaries, $"Block_{i:D3}.bin");
            WriteToFile(block.blockbuffer, filename);
            i++;
        }
    }
    
    public LoadAndCreateBufferExamples(string pathToArk = Settings.DefaultArkPath, string pathToBinaries = Settings.DefaultBinaryTestsPath)
    {
        this.pathToBinaries = pathToBinaries;
        this.pathToArk = pathToArk;
        Ark = new ArkLoader(pathToArk);
    }
}