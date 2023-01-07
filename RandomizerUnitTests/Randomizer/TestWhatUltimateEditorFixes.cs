using System;
using NUnit.Framework;
using UWRandomizerEditor.LEVDotARK;
using UWRandomizerEditor.LEVDotARK.Blocks;

namespace RandomizerUnitTests;

public class TestWhatUWEditorFixes
{
    [Test]
    [Category("RequiresArk")]
    public void DetectDifferencesAfterLoadingInUltimateEditor()
    {
        // var ArkCleanedToModify = new ArkLoader(@"C:\Users\Karl\Desktop\UnderworldStudy\Buffers - Cleaned 15-11-2022\LEV.ARK");
        // var ArkCleaned = new ArkLoader(@"C:\Users\Karl\Desktop\UnderworldStudy\Buffers - Cleaned 15-11-2022\LEV.ARK");
        // RandoTools.RemoveAllDoorReferencesToLocks(ArkCleaned);
        //
        // var path = ArkCleanedToModify.SaveBuffer(@"C:\Users\Karl\Desktop\UnderworldStudy\Buffers - Cleaned - No Doors - 15-11-2022",
        //     "arkcleaned_nodoors.bin");
        // // ArkCleanedToModify.SaveBuffer(@"C:\Users\Karl\Desktop\UnderworldStudy\UW - Cleaned - NoDoors\DATA\",
        // //     "LEV.ARK");
        //
        // Console.WriteLine("Load with Ultimate Editor, save, press enter");
        // Console.ReadLine();
        var ArkCleaned =
            new ArkLoader(
                @"C:\Users\Karl\Desktop\UnderworldStudy\Seeing what is being fixed by UltimateEditor\arkcleaned.bin");
        var ArkCleanedNoDoors =
            new ArkLoader(
                @"C:\Users\Karl\Desktop\UnderworldStudy\Seeing what is being fixed by UltimateEditor\arkcleaned_nodoors.bin");
        var ArkCleanedNoDoorsFixed =
            new ArkLoader(
                @"C:\Users\Karl\Desktop\UnderworldStudy\Seeing what is being fixed by UltimateEditor\arkcleaned_nodoors_fixed.bin");

        // TODO: Extract this into a function
        // Check if every byte of every block outside the StaticObject arrays is the same
        for (int blocknum = 0; blocknum < ArkLoader.NumOfLevels; blocknum++)
        {
            // var TileMapBlockOriginal = ArkCleaned.TileMapObjectsBlocks[blocknum];
            // var AutoMapBlockOriginal = ArkCleaned.AutomapBlocks[blocknum];
            // var MapNotesOriginal = ArkCleaned.MapNotesBlocks[blocknum];
            // var ObjectAnimOverlayOriginal = ArkCleaned.ObjAnimBlocks[blocknum];
            // var TextureMappingOriginal = ArkCleaned.TextMapBlocks[blocknum];
            var TileMapBlockCleaned = ArkCleaned.TileMapObjectsBlocks[blocknum];
            var AutoMapBlockCleaned = ArkCleaned.AutomapBlocks[blocknum];
            var MapNotesCleaned = ArkCleaned.MapNotesBlocks[blocknum];
            var ObjectAnimOverlayCleaned = ArkCleaned.ObjAnimBlocks[blocknum];
            var TextureMappingCleaned = ArkCleaned.TextMapBlocks[blocknum];

            var TileMapBlockNoDoors = ArkCleanedNoDoors.TileMapObjectsBlocks[blocknum];
            var AutoMapBlockNoDoors = ArkCleanedNoDoors.AutomapBlocks[blocknum];
            var MapNotesNoDoors = ArkCleanedNoDoors.MapNotesBlocks[blocknum];
            var ObjectAnimOverlayNoDoors = ArkCleanedNoDoors.ObjAnimBlocks[blocknum];
            var TextureMappingNoDoors = ArkCleanedNoDoors.TextMapBlocks[blocknum];

            var TileMapBlockNoDoorsFixed = ArkCleanedNoDoorsFixed.TileMapObjectsBlocks[blocknum];
            var AutoMapBlockNoDoorsFixed = ArkCleanedNoDoorsFixed.AutomapBlocks[blocknum];
            var MapNotesNoDoorsFixed = ArkCleanedNoDoorsFixed.MapNotesBlocks[blocknum];
            var ObjectAnimOverlayNoDoorsFixed = ArkCleanedNoDoorsFixed.ObjAnimBlocks[blocknum];
            var TextureMappingNoDoorsFixed = ArkCleanedNoDoorsFixed.TextMapBlocks[blocknum];

            // Tilemap

            for (int i = 0; i < TileMapBlockNoDoors.MobileObjectInfoBuffer.Length; i++)
            {
                // Assert.True(TileMapBlockOriginal.MobileObjectInfoBuffer[i] == TileMapBlockToModify.MobileObjectInfoBuffer[i]);
                if (TileMapBlockNoDoors.MobileObjectInfoBuffer[i] != TileMapBlockNoDoorsFixed.MobileObjectInfoBuffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of MobileObjectInfoBuffer (NoDoors-Fixed)");
                if (TileMapBlockCleaned.MobileObjectInfoBuffer[i] != TileMapBlockNoDoorsFixed.MobileObjectInfoBuffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of MobileObjectInfoBuffer (Cleaned-Fixed)");
            }

            for (int i = 0; i < TileMapBlockNoDoors.FreeListMobileObjectBuffer.Length; i++)
            {
                // Assert.True(TileMapBlockOriginal.FreeListMobileObjectBuffer[i] == TileMapBlockToModify.FreeListMobileObjectBuffer[i]);
                if (TileMapBlockNoDoors.FreeListMobileObjectBuffer[i] !=
                    TileMapBlockNoDoorsFixed.FreeListMobileObjectBuffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of FreeListMobileObject (NoDoors-Fixed)");
                if (TileMapBlockCleaned.FreeListMobileObjectBuffer[i] !=
                    TileMapBlockNoDoorsFixed.FreeListMobileObjectBuffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of FreeListMobileObject (Cleaned-Fixed)");
            }

            for (int i = 0; i < TileMapBlockNoDoors.FreeListStaticObjectBuffer.Length; i++)
            {
                // Assert.True(TileMapBlockOriginal.FreeListStaticObjectBuffer[i] == TileMapBlockToModify.FreeListStaticObjectBuffer[i]);
                if (TileMapBlockNoDoors.FreeListStaticObjectBuffer[i] !=
                    TileMapBlockNoDoorsFixed.FreeListStaticObjectBuffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of FreeListStaticObject (NoDoors-Fixed)");
                if (TileMapBlockCleaned.FreeListStaticObjectBuffer[i] !=
                    TileMapBlockNoDoorsFixed.FreeListStaticObjectBuffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of FreeListStaticObject (Cleaned-Fixed)");
            }

            for (int i = 0; i < TileMapBlockNoDoors.TileMapBuffer.Length; i++)
            {
                // Assert.True(TileMapBlockOriginal.TileMapBuffer[i] == TileMapBlockToModify.TileMapBuffer[i]);
                if (TileMapBlockNoDoors.TileMapBuffer[i] != TileMapBlockNoDoorsFixed.TileMapBuffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of TileMapBuffer (NoDoors-Fixed)");
                if (TileMapBlockCleaned.TileMapBuffer[i] != TileMapBlockNoDoorsFixed.TileMapBuffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of TileMapBuffer (Cleaned-Fixed)");
            }

            for (int objnum = 0; objnum < TileMapMasterObjectListBlock.StaticObjectNum; objnum++)
            {
                var objNoDoors = TileMapBlockNoDoors.StaticObjects[objnum];
                var objFixed = TileMapBlockNoDoorsFixed.StaticObjects[objnum];
                var objCleaned = TileMapBlockCleaned.StaticObjects[objnum];

                // Assert.True(objOriginal.QuantityOrSpecialLinkOrSpecialProperty == objModified.QuantityOrSpecialLinkOrSpecialProperty);
                // if (objOriginal is Door doorOriginal)
                // {
                //     //     var doorModified = (Door) objModified;
                //     //     Assert.False(doorModified.HasLock(out ushort temp), $"For object " +
                //     //                                                         $"{doorModified.IdxAtObjectArray} " +
                //     //                                                         $"of level {blocknum}, a lock remained {temp}");
                //     continue;
                //
                // }

                for (int i = 0; i < objNoDoors.Buffer.Length; i++)
                {
                    // Assert.True(objOriginal.Buffer[i] == objModified.Buffer[i]);
                    if (objNoDoors.Buffer[i] != objFixed.Buffer[i])
                        Console.WriteLine(
                            $"Detected difference in byte {i} of StaticObject number {objnum} buffer (NoDoors-Fixed)");
                    if (objCleaned.Buffer[i] != objFixed.Buffer[i])
                        Console.WriteLine(
                            $"Detected difference in byte {i} of StaticObject number {objnum} buffer (Cleaned-Fixed)");
                }
            }

            // Other blocks
            for (int i = 0; i < AutoMapBlockNoDoors.Buffer.Length; i++)
            {
                // Assert.True(AutoMapBlockOriginal.blockbuffer[i] == AutoMapBlockToModify.blockbuffer[i]);
                if (AutoMapBlockNoDoors.Buffer[i] != AutoMapBlockNoDoorsFixed.Buffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of AutoMapBlock (NoDoors-Fixed)");
                if (AutoMapBlockCleaned.Buffer[i] != AutoMapBlockNoDoorsFixed.Buffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of AutoMapBlock (Cleaned-Fixed)");
            }

            for (int i = 0; i < MapNotesNoDoors.Buffer.Length; i++)
            {
                // Assert.True(MapNotesOriginal.blockbuffer[i] == MapNotesToModify.blockbuffer[i]);
                if (MapNotesNoDoors.Buffer[i] != MapNotesNoDoorsFixed.Buffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of MapNotes (NoDoors-Fixed)");
                if (MapNotesCleaned.Buffer[i] != MapNotesNoDoorsFixed.Buffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of MapNotes (Cleaned-Fixed)");
            }

            for (int i = 0; i < ObjectAnimOverlayNoDoors.Buffer.Length; i++)
            {
                // Assert.True(ObjectAnimOverlayOriginal.blockbuffer[i] == ObjectAnimOverlayToModify.blockbuffer[i]);
                if (ObjectAnimOverlayNoDoors.Buffer[i] != ObjectAnimOverlayNoDoorsFixed.Buffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of ObjAnimOverlay (NoDoors-Fixed)");
                if (ObjectAnimOverlayCleaned.Buffer[i] != ObjectAnimOverlayNoDoorsFixed.Buffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of ObjAnimOverlay (Cleaned-Fixed)");
            }

            for (int i = 0; i < TextureMappingNoDoors.Buffer.Length; i++)
            {
                // Assert.True(TextureMappingOriginal.blockbuffer[i] == TextureMappingToModify.blockbuffer[i]);
                if (TextureMappingNoDoors.Buffer[i] != TextureMappingNoDoorsFixed.Buffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of TextureMapping (NoDoors-Fixed)");
                if (TextureMappingCleaned.Buffer[i] != TextureMappingNoDoorsFixed.Buffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of TextureMapping (Cleaned-Fixed)");
            }
        }
    }
}