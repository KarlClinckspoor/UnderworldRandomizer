using UWRandomizerEditor.LEVdotARK;

namespace VisualizationHelperPrograms;

public static class Program
{
    public static void Main()
    {
        var path =
            @"C:\Users\Karl\Desktop\UnderworldStudy\Seeing what is being fixed by UltimateEditor\arkcleaned_nodoors_fixed.bin";
        var baseBufferPath =
            @"C:\Users\Karl\Desktop\UnderworldStudy\Seeing what is being fixed by UltimateEditor\Cleaned-nodoors-fixed";

        Directory.CreateDirectory(baseBufferPath);

        var ark = new ArkLoader(path);

        // Header
        UWRandomizerEditor.Utils.SaveBuffer(ark.header, baseBufferPath, "header.bin");

        // Blocks
        int counterBlock = 0;
        var blockPath = Path.Join(baseBufferPath, "Blocks");
        Directory.CreateDirectory(blockPath);
        foreach (var block in ark.blocks)
        {
            UWRandomizerEditor.Utils.SaveBuffer(block, blockPath,
                $"Block{counterBlock}_level{block.LevelNumber}_length{block.Buffer.Length}.bin");
            counterBlock++;
        }

        #region TilemapBlocks

        counterBlock = 0;
        var tilemapBlocksPath = Path.Join(baseBufferPath, "TileMaps");
        Directory.CreateDirectory(tilemapBlocksPath);
        foreach (var block in ark.TileMapObjectsBlocks)
        {
            var nthTileMapBlockPath = Path.Join(tilemapBlocksPath, $"TileMapBlock{counterBlock}");
            Directory.CreateDirectory(nthTileMapBlockPath);

            UWRandomizerEditor.Utils.SaveBuffer(block, nthTileMapBlockPath,
                $"TileMapBlock{counterBlock}_fullbuffer.bin");
            File.WriteAllBytes(Path.Combine(nthTileMapBlockPath, $"TileMapBuffer{counterBlock}_fullbuffer.bin"),
                block.TileMapBuffer);
            File.WriteAllBytes(
                Path.Combine(nthTileMapBlockPath, $"MobileObjectInfoBuffer{counterBlock}_fullbuffer.bin"),
                block.MobileObjectInfoBuffer);
            File.WriteAllBytes(
                Path.Combine(nthTileMapBlockPath, $"StaticObjectInfoBuffer{counterBlock}_fullbuffer.bin"),
                block.StaticObjectInfoBuffer);
            File.WriteAllBytes(Path.Combine(nthTileMapBlockPath, $"FreeListMobileObject{counterBlock}_fullbuffer.bin"),
                block.FreeListMobileObjectBuffer);
            File.WriteAllBytes(Path.Combine(nthTileMapBlockPath, $"FreeListStaticObject{counterBlock}_fullbuffer.bin"),
                block.FreeListStaticObjectBuffer);

            var counterObjects = 0;
            // Save Mobile Object buffers
            foreach (var mobileObject in block.MobileObjects)
            {
                UWRandomizerEditor.Utils.SaveBuffer(mobileObject, nthTileMapBlockPath,
                    $"MobileObjectIdx{mobileObject.IdxAtObjectArray}_ctr{counterObjects}.bin");
                counterObjects++;
            }

            // Save static Object buffers
            // Doesn't reset to 0.
            foreach (var staticObject in block.StaticObjects)
            {
                UWRandomizerEditor.Utils.SaveBuffer(staticObject, nthTileMapBlockPath,
                    $"StaticObjectIdx{staticObject.IdxAtObjectArray}_ctr{counterObjects}.bin");
                counterObjects++;
            }

            using (StreamWriter sw = new StreamWriter(Path.Join(tilemapBlocksPath,
                       $"FreeListObjectDescription_block{counterBlock}.txt")))
            {
                var setMobile = new HashSet<int>(); // To get which values are referenced in the end
                int mobileDuplicateCounter = 0;
                // Save free list Mobile objects buffers
                counterObjects = 0;
                foreach (var mobileFreeObject in block.FreeMobileObjectSlots)
                {
                    sw.WriteLine($"Mobile Free Object entry {counterObjects} has value {mobileFreeObject.IdxAtFullArray}");
                    UWRandomizerEditor.Utils.SaveBuffer(mobileFreeObject, nthTileMapBlockPath,
                        $"mobileFreeObjectIdx{mobileFreeObject.EntryNum}_ctr{counterObjects}.bin");
                    counterObjects++;
                    mobileDuplicateCounter +=
                        setMobile.Add(mobileFreeObject.IdxAtFullArray)
                            ? 0
                            : 1; // Reminder: Add returns false if element is already present
                }

                var setStatic = new HashSet<int>();
                int staticDuplicateCounter = 0;
                // Save free list Static objects buffers
                foreach (var staticFreeObject in block.FreeStaticObjectSlots)
                {
                    sw.WriteLine($"Static Free Object entry {counterObjects} has value {staticFreeObject.IdxAtFullArray}");
                    UWRandomizerEditor.Utils.SaveBuffer(staticFreeObject, nthTileMapBlockPath,
                        $"staticFreeObjectIdx{staticFreeObject.EntryNum}_ctr{counterObjects}.bin");
                    counterObjects++;
                    staticDuplicateCounter +=
                        setStatic.Add(staticFreeObject.IdxAtFullArray)
                            ? 0
                            : 1; // Reminder: Add returns false if element is already present
                }

                sw.WriteLine($"Summary: Mobile list contains {block.FreeMobileObjectSlots.Length} entries of which" +
                             $" {mobileDuplicateCounter} are duplicates." +
                             $" Indexes present in Mobile List: {string.Join(",", setMobile.OrderBy(x => x))}."
                );
                var allMobileIdxs = Enumerable.Range(0, 255).ToHashSet();
                allMobileIdxs.ExceptWith(setMobile);
                sw.WriteLine($"Indexes not present: {string.Join(",", allMobileIdxs)}");

                sw.WriteLine($"Summary: Static list contains {block.FreeStaticObjectSlots.Length} entries of which" +
                             $" {staticDuplicateCounter} are duplicates." +
                             $" Indexes present in Mobile List: {string.Join(",", setStatic.OrderBy(x => x))}");
                var allStaticIdxs = Enumerable.Range(256, 1024 - 256).ToHashSet();
                allStaticIdxs.ExceptWith(setStatic);
                sw.WriteLine($"Indexes not present: {string.Join(",", allStaticIdxs)}");
            }

            // Iterate through tiles
            counterObjects = 0;
            foreach (var tile in block.Tiles)
            {
                UWRandomizerEditor.Utils.SaveBuffer(tile, nthTileMapBlockPath,
                    $"TileIdx{counterObjects}Offset{tile.Offset},X{tile.XYPos[0]}Y{tile.XYPos[1]}.bin");
                counterObjects++;
            }

            counterBlock++;
        }

        #endregion

        #region TextureMappingBlock

        counterBlock = 0;
        var textureMappingBlocksPath = Path.Join(baseBufferPath, "TextureMappingBlock");
        Directory.CreateDirectory(textureMappingBlocksPath);
        foreach (var textMapBlock in ark.TextMapBlocks)
        {
            UWRandomizerEditor.Utils.SaveBuffer(textMapBlock, textureMappingBlocksPath,
                $"fullbuffer_{counterBlock}.bin");
            counterBlock++;
        }

        #endregion

        #region ObjectAnimationOverlayMap

        counterBlock = 0;
        var objectAnimationOverlayMapPath = Path.Join(baseBufferPath, "ObjectAnimationOverlayBlock");
        Directory.CreateDirectory(objectAnimationOverlayMapPath);
        foreach (var objAnimBlock in ark.ObjAnimBlocks)
        {
            UWRandomizerEditor.Utils.SaveBuffer(objAnimBlock, objectAnimationOverlayMapPath,
                $"fullbuffer_{counterBlock}.bin");
            counterBlock++;
        }

        #endregion

        #region MapNotesBlock

        counterBlock = 0;
        var mapNotesBlockPath = Path.Join(baseBufferPath, "MapNotesBlock");
        Directory.CreateDirectory(mapNotesBlockPath);
        foreach (var mapNotesBlock in ark.MapNotesBlocks)
        {
            UWRandomizerEditor.Utils.SaveBuffer(mapNotesBlock, mapNotesBlockPath, $"fullbuffer_{counterBlock}.bin");
            counterBlock++;
        }

        #endregion

        #region AutomapInfosBlock

        counterBlock = 0;
        var automapInfosBlockPath = Path.Join(baseBufferPath, "AutomapInfosBlock");
        Directory.CreateDirectory(automapInfosBlockPath);
        foreach (var automapInfosBlock in ark.AutomapBlocks)
        {
            UWRandomizerEditor.Utils.SaveBuffer(automapInfosBlock, automapInfosBlockPath,
                $"fullbuffer_{counterBlock}.bin");
            counterBlock++;
        }

        #endregion
    }
}