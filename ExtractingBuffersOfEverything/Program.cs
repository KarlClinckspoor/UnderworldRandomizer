// See https://aka.ms/new-console-template for more information

using UWRandomizerEditor;
using UWRandomizerEditor.LEVDotARK;
using static UWRandomizerEditor.Utils;

namespace ExtractingEverything;

public static class Program {
    public static void Main()
    {
        var path = @"C:\Users\Karl\Desktop\UnderworldStudy\UW\DATA\LEV.ARK";
        var baseBufferPath = @"C:\Users\Karl\Desktop\UnderworldStudy\Buffers";

        Directory.CreateDirectory(baseBufferPath);
        
        var Ark = new ArkLoader(path);
        
        // Header
        Ark.header.SaveBuffer(baseBufferPath, "header.bin");
        
        // Blocks
        int counter_block = 0;
        var blockPath = Path.Join(baseBufferPath, "Blocks");
        Directory.CreateDirectory(blockPath);
        foreach (var block in Ark.blocks)
        {
            block.SaveBuffer(blockPath, $"Block{counter_block}_level{block.LevelNumber}_length{block.TotalBlockLength}.bin");
            counter_block++;
        }
        
        // Tilemap Blocks
        counter_block = 0;
        var tilemapBlocksPath = Path.Join(baseBufferPath, "TileMaps");
        Directory.CreateDirectory(tilemapBlocksPath);
        foreach (var block in Ark.TileMapObjectsBlocks)
        {
            var nthTileMapBlockPath = Path.Join(tilemapBlocksPath, $"TileMapBlock{counter_block}");
            Directory.CreateDirectory(nthTileMapBlockPath);
            
            block.SaveBuffer(nthTileMapBlockPath, $"TileMapBlock{counter_block}_fullbuffer.bin");
            StdSaveBuffer(block.TileMapBuffer, nthTileMapBlockPath, $"TileMapBuffer{counter_block}_fullbuffer.bin");
            StdSaveBuffer(block.MobileObjectInfoBuffer, nthTileMapBlockPath,
                $"MobileObjectInfoBuffer{counter_block}_fullbuffer.bin");
            StdSaveBuffer(block.StaticObjectInfoBuffer, nthTileMapBlockPath,
                $"StaticObjectInfoBuffer{counter_block}_fullbuffer.bin");
            StdSaveBuffer(block.FreeListMobileObjectBuffer, nthTileMapBlockPath,
                $"FreeListMobileObject{counter_block}_fullbuffer.bin");
            StdSaveBuffer(block.FreeListStaticObjectBuffer, nthTileMapBlockPath,
                $"FreeListStaticObject{counter_block}_fullbuffer.bin");
            
            var counter_objects = 0;
            // GameObjects
            // foreach (var gameObject in block.AllGameObjects)
            // {
            //     gameObject.SaveBuffer(nthTileMapBlockPath, 
            //         $"GameObjectIdx{gameObject.IdxAtObjectArray}_ctr{counter_objects}_id{gameObject.ItemID}.bin");
            //     counter_objects++;
            // }
            
            // Save Mobile Object buffers
            counter_objects = 0;
            foreach (var mobileObject in block.MobileObjects)
            {
                mobileObject.SaveBuffer(nthTileMapBlockPath,
                    $"MobileObjectIdx{mobileObject.IdxAtObjectArray}_ctr{counter_objects}.bin");
                counter_objects++;
            }
            
            // Save static Object buffers
            // Doesn't reset to 0.
            foreach (var staticObject in block.StaticObjects)
            {
                staticObject.SaveBuffer(nthTileMapBlockPath,
                    $"StaticObjectIdx{staticObject.IdxAtObjectArray}_ctr{counter_objects}.bin");
                counter_objects++;
            }
            
            // Save free list Mobile objects buffers
            counter_objects = 0;
            foreach (var mobileFreeObject in block.FreeListMobileObject)
            {
                StdSaveBuffer(mobileFreeObject.Buffer, nthTileMapBlockPath,
                    $"mobileFreeObjectIdx{mobileFreeObject.EntryNum}_ctr{counter_objects}.bin");
                counter_objects++;
            }
            // Save free list Static objects buffers
            foreach (var staticFreeObject in block.FreeListStaticObject)
            {
                StdSaveBuffer(staticFreeObject.Buffer, nthTileMapBlockPath,
                    $"staticFreeObjectIdx{staticFreeObject.EntryNum}_ctr{counter_objects}.bin");
                counter_objects++;
            }
            
            // Iterate through tiles
            counter_objects = 0;
            foreach (var tile in block.TileInfos)
            {
                tile.SaveBuffer(nthTileMapBlockPath, $"TileIdx{counter_objects}Offset{tile.Offset},X{tile.XYPos[0]}Y{tile.XYPos[1]}.bin");
                counter_objects++;
            }
            counter_block++;
        }
    }
}