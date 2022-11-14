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

            using (StreamWriter sw = new StreamWriter(Path.Join(tilemapBlocksPath,
                       $"FreeListObjectDescription_block{counter_block}.txt")))
            {
                var setMobile = new HashSet<int>(); // To get which values are referenced in the end
                int MobileDuplicateCounter = 0;
                // Save free list Mobile objects buffers
                counter_objects = 0;
                foreach (var mobileFreeObject in block.FreeListMobileObject)
                {
                    sw.WriteLine($"Mobile Free Object entry {counter_objects} has value {mobileFreeObject.Entry}");
                    StdSaveBuffer(mobileFreeObject.Buffer, nthTileMapBlockPath,
                        $"mobileFreeObjectIdx{mobileFreeObject.EntryNum}_ctr{counter_objects}.bin");
                    counter_objects++;
                    MobileDuplicateCounter += setMobile.Add(mobileFreeObject.Entry) ? 0 : 1; // Reminder: Add returns false if element is already present
                }

                var setStatic = new HashSet<int>();
                int StaticDuplicateCounter = 0;
                // Save free list Static objects buffers
                foreach (var staticFreeObject in block.FreeListStaticObject)
                {
                    sw.WriteLine($"Static Free Object entry {counter_objects} has value {staticFreeObject.Entry}");
                    StdSaveBuffer(staticFreeObject.Buffer, nthTileMapBlockPath,
                        $"staticFreeObjectIdx{staticFreeObject.EntryNum}_ctr{counter_objects}.bin");
                    counter_objects++;
                    StaticDuplicateCounter += setStatic.Add(staticFreeObject.Entry) ? 0 : 1; // Reminder: Add returns false if element is already present
                }

                sw.WriteLine($"Summary: Mobile list contains {block.FreeListMobileObject.Length} entries of which" +
                             $" {MobileDuplicateCounter} are duplicates." +
                             $" Indexes present in Mobile List: {string.Join(",", setMobile.OrderBy(x => x))}."
                );
                var allMobileIdxs = Enumerable.Range(0, 255).ToHashSet();
                allMobileIdxs.ExceptWith(setMobile);
                sw.WriteLine($"Indexes not present: {string.Join(",", allMobileIdxs)}");
                
                sw.WriteLine($"Summary: Static list contains {block.FreeListStaticObject.Length} entries of which" +
                             $" {StaticDuplicateCounter} are duplicates." +
                             $" Indexes present in Mobile List: {string.Join(",", setStatic.OrderBy(x => x))}");
                var allStaticIdxs = Enumerable.Range(256, 1024 - 256).ToHashSet();
                allStaticIdxs.ExceptWith(setStatic);
                sw.WriteLine($"Indexes not present: {string.Join(",", allStaticIdxs)}");
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