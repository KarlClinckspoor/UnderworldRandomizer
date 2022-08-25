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
        int i = 0;
        foreach (var block in Ark.TextMapBlocks)
        {
            Console.WriteLine($"Extracting TextMapBlock {i+1}/{Ark.TextMapBlocks.Length}");
            string filename = Path.Combine(Settings.DefaultBinaryTestsPath, $"TextMapBlock{i:D3}.bin");
            WriteToFile(block.blockbuffer, filename);
            i++;
        }
    }

    public void ExtractMapNotesBlocks()
    {
        int i = 0;
        foreach (var block in Ark.MapNotesBlocks)
        {
            Console.WriteLine($"Extracting MapNotesBlock {i+1}/{Ark.MapNotesBlocks.Length}");
            string filename = Path.Combine(Settings.DefaultBinaryTestsPath, $"MapNotesBlock{i:D3}.bin");
            WriteToFile(block.blockbuffer, filename);
            i++;
        }
    }

    public void ExtractAutomapBlocks()
    {
        int i = 0;
        foreach (var block in Ark.AutomapBlocks)
        {
            Console.WriteLine($"Extracting AutomapBlock {i+1}/{Ark.AutomapBlocks.Length}");
            string filename = Path.Combine(Settings.DefaultBinaryTestsPath, $"AutomapBlock{i:D3}.bin");
            WriteToFile(block.blockbuffer, filename);
            i++;
        }
    }

    public void ExtractObjAnimBlocks()
    {
        int i = 0;
        foreach (var block in Ark.ObjAnimBlocks)
        {
            Console.WriteLine($"Extracting ObjAnimBlock {i+1}/{Ark.ObjAnimBlocks.Length}");
            string filename = Path.Combine(Settings.DefaultBinaryTestsPath, $"ObjAnimBlock{i:D3}.bin");
            WriteToFile(block.blockbuffer, filename);
            i++;
        }
    }

    public void ExtractTiles()
    {
        int level = 0;
        int tile = 0;
        foreach (var block in Ark.TileMapObjectsBlocks)
        {
            Console.WriteLine($"Processing level {level+1}/{Ark.TileMapObjectsBlocks.Length}");
            block.ExtractInfoFromTileMapBuffer();
            foreach (var tileInfo in block.TileInfos)
            {
                Console.WriteLine($"Saving Tile {tile+1}/{block.TileInfos.Length}");
                string filename = Path.Combine(Settings.DefaultBinaryTestsPath, $"BLOCK{level}_TILE{tile}.bin");
                WriteToFile(tileInfo.TileBuffer, filename);
                tile++;
            }

            level++;
            tile = 0;
            break; // Saving only 1 level for now. Way too much.
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
    
    public LoadAndCreateBufferExamples(string? pathToArk = null, string? pathToBinaries = null)
    {
        this.pathToArk = pathToArk is null ? Settings.DefaultArkPath : pathToArk;
        this.pathToBinaries = pathToBinaries is null ? Settings.DefaultBinaryTestsPath : pathToBinaries;
        Ark = new ArkLoader(this.pathToArk);
    }
}