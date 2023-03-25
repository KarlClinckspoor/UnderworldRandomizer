// See https://aka.ms/new-console-template for more information

using UWRandomizerEditor.LEVdotARK;

namespace ExtractingBuffersOfEverything;

public static class Program
{
    public static void Main()
    {
        var path =
            @"C:\Users\Karl\Desktop\UnderworldStudy\Seeing what is being fixed by UltimateEditor\arkcleaned_nodoors_fixed.bin";
        var baseBufferPath =
            @"C:\Users\Karl\Desktop\UnderworldStudy\Seeing what is being fixed by UltimateEditor\Cleaned-nodoors-fixed";

        Directory.CreateDirectory(baseBufferPath);

        var Ark = new ArkLoader(path);

        // Header
        UWRandomizerEditor.Utils.SaveBuffer(Ark.header, baseBufferPath, "header.bin");

        // Blocks
        int counter_block = 0;
        var blockPath = Path.Join(baseBufferPath, "Blocks");
        Directory.CreateDirectory(blockPath);
        foreach (var block in Ark.blocks)
        {
            UWRandomizerEditor.Utils.SaveBuffer(block, blockPath,
                $"Block{counter_block}_level{block.LevelNumber}_length{block.Buffer.Length}.bin");
            counter_block++;
        }

        #region TilemapBlocks

        counter_block = 0;
        var tilemapBlocksPath = Path.Join(baseBufferPath, "TileMaps");
        Directory.CreateDirectory(tilemapBlocksPath);
        foreach (var block in Ark.TileMapObjectsBlocks)
        {
            var nthTileMapBlockPath = Path.Join(tilemapBlocksPath, $"TileMapBlock{counter_block}");
            Directory.CreateDirectory(nthTileMapBlockPath);

            UWRandomizerEditor.Utils.SaveBuffer(block, nthTileMapBlockPath,
                $"TileMapBlock{counter_block}_fullbuffer.bin");
            File.WriteAllBytes(Path.Combine(nthTileMapBlockPath, $"TileMapBuffer{counter_block}_fullbuffer.bin"),
                block.TileMapBuffer);
            File.WriteAllBytes(
                Path.Combine(nthTileMapBlockPath, $"MobileObjectInfoBuffer{counter_block}_fullbuffer.bin"),
                block.MobileObjectInfoBuffer);
            File.WriteAllBytes(
                Path.Combine(nthTileMapBlockPath, $"StaticObjectInfoBuffer{counter_block}_fullbuffer.bin"),
                block.StaticObjectInfoBuffer);
            File.WriteAllBytes(Path.Combine(nthTileMapBlockPath, $"FreeListMobileObject{counter_block}_fullbuffer.bin"),
                block.FreeListMobileObjectBuffer);
            File.WriteAllBytes(Path.Combine(nthTileMapBlockPath, $"FreeListStaticObject{counter_block}_fullbuffer.bin"),
                block.FreeListStaticObjectBuffer);

            var counter_objects = 0;
            // Save Mobile Object buffers
            counter_objects = 0;
            foreach (var mobileObject in block.MobileObjects)
            {
                UWRandomizerEditor.Utils.SaveBuffer(mobileObject, nthTileMapBlockPath,
                    $"MobileObjectIdx{mobileObject.IdxAtObjectArray}_ctr{counter_objects}.bin");
                counter_objects++;
            }

            // Save static Object buffers
            // Doesn't reset to 0.
            foreach (var staticObject in block.StaticObjects)
            {
                UWRandomizerEditor.Utils.SaveBuffer(staticObject, nthTileMapBlockPath,
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
                foreach (var mobileFreeObject in block.FreeListMobileObjects)
                {
                    sw.WriteLine($"Mobile Free Object entry {counter_objects} has value {mobileFreeObject.IdxAtArray}");
                    UWRandomizerEditor.Utils.SaveBuffer(mobileFreeObject, nthTileMapBlockPath,
                        $"mobileFreeObjectIdx{mobileFreeObject.EntryNum}_ctr{counter_objects}.bin");
                    counter_objects++;
                    MobileDuplicateCounter +=
                        setMobile.Add(mobileFreeObject.IdxAtArray)
                            ? 0
                            : 1; // Reminder: Add returns false if element is already present
                }

                var setStatic = new HashSet<int>();
                int StaticDuplicateCounter = 0;
                // Save free list Static objects buffers
                foreach (var staticFreeObject in block.FreeListStaticObjects)
                {
                    sw.WriteLine($"Static Free Object entry {counter_objects} has value {staticFreeObject.IdxAtArray}");
                    UWRandomizerEditor.Utils.SaveBuffer(staticFreeObject, nthTileMapBlockPath,
                        $"staticFreeObjectIdx{staticFreeObject.EntryNum}_ctr{counter_objects}.bin");
                    counter_objects++;
                    StaticDuplicateCounter +=
                        setStatic.Add(staticFreeObject.IdxAtArray)
                            ? 0
                            : 1; // Reminder: Add returns false if element is already present
                }

                sw.WriteLine($"Summary: Mobile list contains {block.FreeListMobileObjects.Length} entries of which" +
                             $" {MobileDuplicateCounter} are duplicates." +
                             $" Indexes present in Mobile List: {string.Join(",", setMobile.OrderBy(x => x))}."
                );
                var allMobileIdxs = Enumerable.Range(0, 255).ToHashSet();
                allMobileIdxs.ExceptWith(setMobile);
                sw.WriteLine($"Indexes not present: {string.Join(",", allMobileIdxs)}");

                sw.WriteLine($"Summary: Static list contains {block.FreeListStaticObjects.Length} entries of which" +
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
                UWRandomizerEditor.Utils.SaveBuffer(tile, nthTileMapBlockPath,
                    $"TileIdx{counter_objects}Offset{tile.Offset},X{tile.XYPos[0]}Y{tile.XYPos[1]}.bin");
                counter_objects++;
            }

            counter_block++;
        }

        #endregion

        #region TextureMappingBlock

        counter_block = 0;
        var TextureMappingBlocksPath = Path.Join(baseBufferPath, "TextureMappingBlock");
        Directory.CreateDirectory(TextureMappingBlocksPath);
        foreach (var textMapBlock in Ark.TextMapBlocks)
        {
            UWRandomizerEditor.Utils.SaveBuffer(textMapBlock, TextureMappingBlocksPath,
                $"fullbuffer_{counter_block}.bin");
            counter_block++;
        }

        #endregion

        #region ObjectAnimationOverlayMap

        counter_block = 0;
        var ObjectAnimationOverlayMapPath = Path.Join(baseBufferPath, "ObjectAnimationOverlayBlock");
        Directory.CreateDirectory(ObjectAnimationOverlayMapPath);
        foreach (var objAnimBlock in Ark.ObjAnimBlocks)
        {
            UWRandomizerEditor.Utils.SaveBuffer(objAnimBlock, ObjectAnimationOverlayMapPath,
                $"fullbuffer_{counter_block}.bin");
            counter_block++;
        }

        #endregion

        #region MapNotesBlock

        counter_block = 0;
        var MapNotesBlockPath = Path.Join(baseBufferPath, "MapNotesBlock");
        Directory.CreateDirectory(MapNotesBlockPath);
        foreach (var mapNotesBlock in Ark.MapNotesBlocks)
        {
            UWRandomizerEditor.Utils.SaveBuffer(mapNotesBlock, MapNotesBlockPath, $"fullbuffer_{counter_block}.bin");
            counter_block++;
        }

        #endregion

        #region AutomapInfosBlock

        counter_block = 0;
        var AutomapInfosBlockPath = Path.Join(baseBufferPath, "AutomapInfosBlock");
        Directory.CreateDirectory(AutomapInfosBlockPath);
        foreach (var automapInfosBlock in Ark.AutomapBlocks)
        {
            UWRandomizerEditor.Utils.SaveBuffer(automapInfosBlock, AutomapInfosBlockPath,
                $"fullbuffer_{counter_block}.bin");
            counter_block++;
        }

        #endregion
    }
}