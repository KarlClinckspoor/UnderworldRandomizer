using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using NUnit.Framework;
using UWRandomizer;
using UWRandomizerEditor;
using UWRandomizerEditor.LEVDotARK;
using UWRandomizerEditor.LEVDotARK.Blocks;
using UWRandomizerEditor.LEVDotARK.GameObjects;
using UWRandomizerEditor.LEVDotARK.GameObjects.Specifics;
using System.Configuration;

namespace RandomizerUnitTests;

[TestFixture]
public class TestGameObjectProperties
{
    private const int numOfLevels = 9;
    // Removing this to temporarily avoid problems with building in github actions. Keeping the same for now.
    // TODO: Can I put the .json test files without problems?
    // private Stream[] streamsPristine = new Stream[numOfLevels];
    private String[] streamsPristine = new String[numOfLevels];
    private List<List<Dictionary<string, int>>> jsonsPristine = new List<List<Dictionary<string, int>>>(numOfLevels);
    private ArkLoader arkPristine;

    // private Stream[] streamsCleaned = new Stream[numOfLevels];
    private String[] streamsCleaned = new String[numOfLevels];
    private List<List<Dictionary<string, int>>> jsonsCleaned = new List<List<Dictionary<string, int>>>(numOfLevels);
    private ArkLoader arkCleaned;

    private Configuration config;

    public enum PossibleLevArkToTest
    {
        pristine = 0,
        cleaned = 1
    }

    private static readonly Dictionary<string, string> json_to_UWR = new Dictionary<string, string>()
    {
        {"item_id", "ItemID"},
        {"flags", "Flags"},
        {"enchantment", "EnchantFlag"},
        {"doordir", "Doordir"},
        {"invis", "Invis"},
        {"is_quant", "IsQuant"},
        {"zpos", "Zpos"},
        {"heading", "Heading"},
        {"xpos", "Xpos"},
        {"ypos", "Ypos"},
        {"quality", "Quality"},
        {"next", "next"},
        {"owner", "Owner_or_special"},
        {"link", "QuantityOrSpecialLinkOrSpecialProperty"}
    };

    [OneTimeSetUp]
    [Category("RequiresArk")]
    public void Setup()
    {
        // var configMap = new ConfigurationFileMap("./ReSharperTestRunner.dll.config");
        // string path = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;
        config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        // var config = ConfigurationManager.OpenMappedMachineConfiguration(configMap);
        // var config = ConfigurationManager.OpenMappedMachineConfiguration(configMap);
        for (int blocknum = 0; blocknum < numOfLevels; blocknum++)
        {
            // Jesus this looks ugly. But it's only Loading the jsons into the lists, and the appropriate ArkLoader isntances
            // streamsPristine[blocknum] =
            //     Assembly.GetExecutingAssembly()
            //         .GetManifestResourceStream(
            //             $"RandomizerUnitTests.testdata.PristineUW1.Block{blocknum}_objects.json") ??
            //     throw new InvalidOperationException();
            streamsPristine[blocknum] =
                File.ReadAllText(Path.Join(
                    config.AppSettings.Settings["RUTTestDataPath"].Value,
                    @$"PristineUW1\Block{blocknum}_objects.json")
                );
            // @$"C:\Users\Karl\Desktop\UnderworldStudy\UnderworldRandomizer\RandomizerUnitTests\testdata\PristineUW1\Block{blocknum}_objects.json");
            jsonsPristine.Add(JsonSerializer.Deserialize<List<Dictionary<string, int>>>(streamsPristine[blocknum],
                new JsonSerializerOptions() {AllowTrailingCommas = true}) ?? throw new InvalidOperationException());
            arkPristine = new ArkLoader(Settings.DefaultArkPath);

            // streamsCleaned[blocknum] =
            //     Assembly.GetExecutingAssembly()
            //         .GetManifestResourceStream(
            //             $"RandomizerUnitTests.testdata.CleanedUW1.Block{blocknum}_objects.json") ??
            //     throw new InvalidOperationException();
            streamsCleaned[blocknum] = File.ReadAllText(
                Path.Join(config.AppSettings.Settings["RUTTestDataPath"].Value,
                    $@"CleanedUW1\Block{blocknum}_objects.json"));
            // @$"C:\Users\Karl\Desktop\UnderworldStudy\UnderworldRandomizer\RandomizerUnitTests\testdata\CleanedUW1\Block{blocknum}_objects.json");
            jsonsCleaned.Add(JsonSerializer.Deserialize<List<Dictionary<string, int>>>(
                streamsCleaned[blocknum],
                new JsonSerializerOptions() {AllowTrailingCommas = true}) ?? throw new InvalidOperationException());
            // TODO: De-hardcode this
            arkCleaned = new ArkLoader(config.AppSettings.Settings["UWArkCleanedPath"].Value);
            // arkCleaned = new ArkLoader(ConfigurationManager.AppSettings["UWArkCleanedPath"]);
        }
    }


    [Test]
    [Category("RequiresArk")]
    public void TestStaticObjectProperties(
        [Range(0, numOfLevels - 1, 1)] int blocknum, // Reminder: Range is [from, to], not [from, to[
        [Values(PossibleLevArkToTest.pristine, PossibleLevArkToTest.cleaned)]
        PossibleLevArkToTest pristine,
        [Values("item_id", "flags", "enchantment", "doordir", "invis", "is_quant", "zpos", "heading", "ypos", "xpos",
            "quality", "next", "owner", "link")]
        string key
    )
    {
        var (ark, json) = selectArkAndJson(blocknum, pristine);

        // For whatever reason, hank's loader has objects [0-1024], but it should have been [0-1024[, right? krokot's goes
        // goes up to 1023.
        Assert.True(json.Count - 1 == ark.TileMapObjectsBlocks[blocknum].AllGameObjects.Length);

        IterateAndCompareAttributesStaticObject(json, key, ark, json_to_UWR[key], blocknum);
    }

    private void IterateAndCompareAttributesStaticObject(List<Dictionary<string, int>> json, string correctLabel,
        ArkLoader ark, string compareLabel, int blocknum)
    {
        for (int i = 0; i < json.Count - 1; i++)
        {
            var correct = json[i];

            if (i < 256)
            {
                var compare = ark.TileMapObjectsBlocks[blocknum].MobileObjects[i];
                var correctProperty = correct[correctLabel];
                var compareProperty = GetPropertyValue(compare, compareLabel);
                Assert.True(correctProperty == compareProperty,
                    $"Mobile object {i}: Correct: {correctProperty}. Got {compareProperty}");
            }
            else
            {
                var compare = ark.TileMapObjectsBlocks[blocknum].StaticObjects[i - 256];
                var correctProperty = correct[correctLabel];
                var compareProperty = GetPropertyValue(compare, compareLabel);
                Assert.True(correctProperty == compareProperty,
                    $"Static object {i}: Correct: {correctProperty}. Got {compareProperty}");
            }
        }
    }


    /// <summary>
    /// This is used to get a property from StaticObject or Mobile object, which are typically bytes, shorts, ints,
    /// and convert them into ints.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="propname"></param>
    /// <returns></returns>
    private static int GetPropertyValue(object obj, string propname)
    {
        return Convert.ToInt32(obj.GetType().GetProperty(propname).GetValue(obj, null));
    }

    /// <summary>
    /// This selects if I'm using a "Pristine" UW1 (from GOG copy) or a "Cleaned" UW1 (from Ultimate Editor)
    /// </summary>
    /// <param name="blocknum"></param>
    /// <param name="pristine"></param>
    /// <returns></returns>
    private Tuple<ArkLoader, List<Dictionary<string, int>>> selectArkAndJson(int blocknum,
        PossibleLevArkToTest pristine)
    {
        ArkLoader ark;
        List<Dictionary<string, int>> json;
        if (pristine == PossibleLevArkToTest.pristine)
        {
            json = jsonsPristine[blocknum];
            ark = arkPristine;
        }
        else
        {
            json = jsonsCleaned[blocknum];
            ark = arkCleaned;
        }

        return new Tuple<ArkLoader, List<Dictionary<string, int>>>(ark, json);
    }
}

public class ManualTests
{
    private Configuration config;

    [OneTimeSetUp]
    public void SetUp()
    {
        config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
    }
    

    [Test]
    public void TestTileInfoComparingToUltimateEditor_manual()
    {
        var tile = new TileInfo(1609, new byte[] {0x11, 0x20, 0x1E, 0x00}, 6436, 0);
        // Some water tile near that island with a lurker nearby
        var reference = new TileInfo(1609, 0, 6436, 0);
        reference.TileType = (int) TileInfo.TileTypes.open;
        reference.TileHeight = 1;
        reference.DoorBit = 0;
        reference.NoMagic = 0;
        reference.FloorTextureIdx = 8;
        reference.WallTextureIdx = 30;
        reference.FirstObjIdx = 0;


        Assert.True(tile.Equals(reference));
    }

    [Test]
    public void TestRemovingLockFromDoor_ManualBuffers()
    {
        var doorUnlocked = new Door( new byte[] {0x41, 0x01, 0x50, 0x6F, 0x28, 0x00, 0x00, 0x00} , 1012);
        var doorLocked = new Door( new byte[] {0x41, 0x01, 0x50, 0x6F, 0x28, 0x00, 0xC0, 0xF8}, 1012);
        var doorToModify = new Door( new byte[] {0x41, 0x01, 0x50, 0x6F, 0x28, 0x00, 0xC0, 0xF8}, 1012);
        
        Assert.False(doorLocked.Equals(doorUnlocked));
        Assert.True(doorLocked.Equals(doorToModify));
        Assert.False(doorToModify.Equals(doorUnlocked));
        
        doorToModify.RemoveLock();
        
        Assert.False(doorLocked.Equals(doorUnlocked));
        Assert.False(doorLocked.Equals(doorToModify));
        Assert.True(doorToModify.Equals(doorUnlocked));
    }

    [Test]
    [Category("RequiresArk")]
    public void TestRemovingLockFromDoor_WithArk()
    {
        // var ArkOriginal = new ArkLoader(@"C:\Users\Karl\Desktop\UnderworldStudy\UW\DATA\LEV.ARK");
        // var ArkEditor = new ArkLoader(@"C:\Users\Karl\Desktop\UnderworldStudy\UW - Doors\DATA\LEV.ARK");
        var ArkOriginal = new ArkLoader(config.AppSettings.Settings["UWArkOriginalPath"].Value);
        var ArkEditor = new ArkLoader(Path.Join(config.AppSettings.Settings["BasePath"].Value, @"UW - Doors\Data\Lev.ark"));

        var doorToUnlock = (Door) ArkOriginal.TileMapObjectsBlocks[0].AllGameObjects[1012];
        var doorUnlockedByEditor = (Door)  ArkEditor.TileMapObjectsBlocks[0].AllGameObjects[1012];

        Assert.True(doorToUnlock.HasLock());
        Assert.False(doorUnlockedByEditor.HasLock());
        Assert.False(doorToUnlock.Equals(doorUnlockedByEditor));
        
        doorToUnlock.RemoveLock();
        Assert.False(doorToUnlock.HasLock());
        Assert.True(doorToUnlock.Equals(doorUnlockedByEditor));
        Assert.True(doorToUnlock.Equals( new Door( new byte[] {0x41, 0x01, 0x50, 0x6F, 0x28, 0x00, 0x00, 0x00} , 1012) ));

        ArkOriginal.TileMapObjectsBlocks[0].UpdateStaticObjectInfoBuffer();
        ArkOriginal.TileMapObjectsBlocks[0].UpdateBuffer();
        ArkOriginal.ReconstructBufferFromBlocks();
        var path = ArkOriginal.SaveBuffer(@"C:\Users\Karl\Desktop\UnderworldStudy\Buffers - Doors study",
            "ark_withdoor1012unlocked.bin");

        var ArkModified = new ArkLoader(path);
        var doorUnlockedHere = (Door) ArkModified.TileMapObjectsBlocks[0].AllGameObjects[1012];
        Assert.False(doorUnlockedHere.HasLock());
        Assert.True(doorUnlockedHere.Equals(doorUnlockedByEditor));
        Assert.True(doorUnlockedHere.Equals( new Door( new byte[] {0x41, 0x01, 0x50, 0x6F, 0x28, 0x00, 0x00, 0x00} , 1012) ));
    }

    /// <summary>
    /// Returns -1 if buffers are of different length, 0 if they are equal or the index where they differed
    /// </summary>
    /// <param name="buffer1"></param>
    /// <param name="buffer2"></param>
    /// <returns></returns>
    private int CompareBuffers(byte[] buffer1, byte[] buffer2)
    {
        if (buffer1.Length != buffer2.Length)
        {
            return -1;
        }

        for (int i = 0; i < buffer1.Length; i++)
        {
            if (buffer1[i] != buffer2[i])
            {
                return i;
            }
        }

        return 0;
    }

    // This test isn't working ATM because somehow an object isn't updating its buffer. But checking with UE and UWE, it's fine
    [Test]
    [Category("RequiresArk")]
    public void TestRemovingAllLocks_WithArk()
    {
        // I'm testing here both the original and the "cleaned" version from UltimateEditor
        // var ArkOriginal = new ArkLoader(@"C:\Users\Karl\Desktop\UnderworldStudy\UW\DATA\LEV.ARK");
        // var ArkOriginalToModify = new ArkLoader(@"C:\Users\Karl\Desktop\UnderworldStudy\UW\DATA\LEV.ARK");
        var ArkOriginal = new ArkLoader(config.AppSettings.Settings["UWArkOriginalPath"].Value);
        var ArkOriginalToModify = new ArkLoader(config.AppSettings.Settings["UWArkOriginalPath"].Value);
        
        var ArkCleaned = new ArkLoader(config.AppSettings.Settings["UWArkCleanedPath"].Value);
        var ArkCleanedToModify = new ArkLoader(config.AppSettings.Settings["UWArkCleanedPath"].Value);

        RandoTools.RemoveAllDoorReferencesToLocks(ArkOriginalToModify);
        RandoTools.RemoveAllDoorReferencesToLocks(ArkCleanedToModify);

        var pathOriginal = ArkOriginalToModify.SaveBuffer(
            // @"C:\Users\Karl\Desktop\UnderworldStudy\BufferTests\RemovingDoors",
            Path.Join(config.AppSettings.Settings["BufferTestsPath"].Value, "RemovingDoors"),
            "ark_nodoors.bin");
        var pathCleaned = ArkCleanedToModify.SaveBuffer(
            Path.Join(config.AppSettings.Settings["BufferTestsPath"].Value, "RemovingDoors"),
            "ark_cleaned_nodoors.bin");

        var ArkOriginalModified = new ArkLoader(pathOriginal);
        var ArkCleanedModified = new ArkLoader(pathCleaned);
        
        // Seeing if the buffers were saved correctly and if the buffers were altered in any way
        Assert.AreEqual(CompareBuffers(ArkOriginalModified.arkbuffer, ArkOriginalToModify.arkbuffer), 0);
        Assert.AreEqual(CompareBuffers(ArkCleanedModified.arkbuffer, ArkCleanedToModify.arkbuffer), 0);
        
        // Check if every byte of every block outside the StaticObject arrays is the same
        for (int blocknum = 0; blocknum < ArkLoader.NumOfLevels; blocknum++)
        {
            // TileMap
            Assert.AreEqual(CompareBuffers(
                ArkOriginal.TileMapObjectsBlocks[blocknum].TileMapBuffer,
                ArkOriginalModified.TileMapObjectsBlocks[blocknum].TileMapBuffer
            ), 0);
            
            Assert.AreEqual(CompareBuffers(
                ArkOriginal.TileMapObjectsBlocks[blocknum].MobileObjectInfoBuffer,
                ArkOriginalModified.TileMapObjectsBlocks[blocknum].MobileObjectInfoBuffer
            ), 0);
            
            Assert.AreEqual(CompareBuffers(
                ArkOriginal.TileMapObjectsBlocks[blocknum].FreeListMobileObjectBuffer,
                ArkOriginalModified.TileMapObjectsBlocks[blocknum].FreeListMobileObjectBuffer
            ), 0);
            
            Assert.AreEqual(CompareBuffers(
                ArkOriginal.TileMapObjectsBlocks[blocknum].FreeListStaticObjectBuffer,
                ArkOriginalModified.TileMapObjectsBlocks[blocknum].FreeListStaticObjectBuffer
            ), 0);
            
            // Should have modified only the StaticObjectBuffer (Doors)
            Assert.AreNotEqual(CompareBuffers(
                ArkOriginal.TileMapObjectsBlocks[blocknum].StaticObjectInfoBuffer,
                ArkOriginalModified.TileMapObjectsBlocks[blocknum].StaticObjectInfoBuffer
            ), 0);
            
            Assert.AreEqual(CompareBuffers(
                ArkCleaned.TileMapObjectsBlocks[blocknum].TileMapBuffer,
                ArkCleanedModified.TileMapObjectsBlocks[blocknum].TileMapBuffer
            ), 0);
            
            Assert.AreEqual(CompareBuffers(
                ArkCleaned.TileMapObjectsBlocks[blocknum].MobileObjectInfoBuffer,
                ArkCleanedModified.TileMapObjectsBlocks[blocknum].MobileObjectInfoBuffer
            ), 0);
            
            Assert.AreEqual(CompareBuffers(
                ArkCleaned.TileMapObjectsBlocks[blocknum].FreeListMobileObjectBuffer,
                ArkCleanedModified.TileMapObjectsBlocks[blocknum].FreeListMobileObjectBuffer
            ), 0);
            
            Assert.AreEqual(CompareBuffers(
                ArkCleaned.TileMapObjectsBlocks[blocknum].FreeListStaticObjectBuffer,
                ArkCleanedModified.TileMapObjectsBlocks[blocknum].FreeListStaticObjectBuffer
            ), 0);
            
            // Should have modified only the StaticObjectBuffer (Doors)
            Assert.AreNotEqual(CompareBuffers(
                ArkCleaned.TileMapObjectsBlocks[blocknum].StaticObjectInfoBuffer,
                ArkCleanedModified.TileMapObjectsBlocks[blocknum].StaticObjectInfoBuffer
            ), 0);
            
            // AutomapBlocks
            Assert.AreEqual(CompareBuffers(
                ArkOriginal.AutomapBlocks[blocknum].blockbuffer,
                ArkOriginalModified.AutomapBlocks[blocknum].blockbuffer
            ), 0);
            
            Assert.AreEqual(CompareBuffers(
                ArkCleaned.AutomapBlocks[blocknum].blockbuffer,
                ArkCleanedModified.AutomapBlocks[blocknum].blockbuffer
            ), 0);
            
            // Map Notes
            Assert.AreEqual(CompareBuffers(
                ArkOriginal.MapNotesBlocks[blocknum].blockbuffer,
                ArkOriginalModified.MapNotesBlocks[blocknum].blockbuffer
            ), 0);
            
            Assert.AreEqual(CompareBuffers(
                ArkCleaned.MapNotesBlocks[blocknum].blockbuffer,
                ArkCleanedModified.MapNotesBlocks[blocknum].blockbuffer
            ), 0);
            
            // ObjectAnimOverlay
            Assert.AreEqual(CompareBuffers(
                ArkOriginal.ObjAnimBlocks[blocknum].blockbuffer,
                ArkOriginalModified.ObjAnimBlocks[blocknum].blockbuffer
            ), 0);
            
            Assert.AreEqual(CompareBuffers(
                ArkCleaned.ObjAnimBlocks[blocknum].blockbuffer,
                ArkCleanedModified.ObjAnimBlocks[blocknum].blockbuffer
            ), 0);
            
            // TextureMapping
            Assert.AreEqual(CompareBuffers(
                ArkOriginal.TextMapBlocks[blocknum].blockbuffer,
                ArkOriginalModified.TextMapBlocks[blocknum].blockbuffer
            ), 0);
            
            Assert.AreEqual(CompareBuffers(
                ArkCleaned.TextMapBlocks[blocknum].blockbuffer,
                ArkCleanedModified.TextMapBlocks[blocknum].blockbuffer
            ), 0);
            
            
            for (int objnum = 0; objnum < TileMapMasterObjectListBlock.StaticObjectNum; objnum++)
            {
                var objOriginal = ArkOriginal.TileMapObjectsBlocks[blocknum].StaticObjects[objnum];
                var objModified = ArkOriginalModified.TileMapObjectsBlocks[blocknum].StaticObjects[objnum];
                
                var objCleaned = ArkCleaned.TileMapObjectsBlocks[blocknum].StaticObjects[objnum];
                var objCleanedModified = ArkCleanedModified.TileMapObjectsBlocks[blocknum].StaticObjects[objnum];
                
                if (objOriginal is Door doorOriginal) // Ignore doors
                {
                    if (doorOriginal.HasLock()) // If they have a lock, they should be different
                    {
                        Assert.True(CompareBuffers(doorOriginal.Buffer, ((Door) objModified).Buffer) > 0);
                        Assert.True(CompareBuffers(objOriginal.Buffer, objModified.Buffer) > 0); // Will this be different?
                    }
                    else // Otherwise should be the same
                    {
                        Assert.True(CompareBuffers(doorOriginal.Buffer, ((Door) objModified).Buffer) == 0);
                        Assert.True(CompareBuffers(objOriginal.Buffer, objModified.Buffer) == 0);
                        
                    }

                    continue;
                }
                
                if (objCleaned is Door doorCleaned) // Ignore doors
                {
                    if (doorCleaned.HasLock()) // If they have a lock, they should be different
                    {
                        Assert.True(CompareBuffers(doorCleaned.Buffer, ((Door) objCleanedModified).Buffer) > 0);
                        Assert.True(CompareBuffers(objCleaned.Buffer, objCleanedModified.Buffer) > 0); // Will this be different?
                    }
                    else // Otherwise should be the same
                    {
                        Assert.True(CompareBuffers(doorCleaned.Buffer, ((Door) objCleanedModified).Buffer) == 0);
                        Assert.True(CompareBuffers(objCleaned.Buffer, objCleanedModified.Buffer) == 0);
                        
                    }

                    continue;
                }
                
                Assert.AreEqual(CompareBuffers(objOriginal.Buffer, objModified.Buffer), 0);
                Assert.AreEqual(CompareBuffers(objCleaned.Buffer, objCleanedModified.Buffer), 0);
            }
            
        }
    }

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
            new ArkLoader(@"C:\Users\Karl\Desktop\UnderworldStudy\Seeing what is being fixed by UltimateEditor\arkcleaned.bin");
        var ArkCleanedNoDoors =
            new ArkLoader(
                @"C:\Users\Karl\Desktop\UnderworldStudy\Seeing what is being fixed by UltimateEditor\arkcleaned_nodoors.bin");
        var ArkCleanedNoDoorsFixed = new ArkLoader(@"C:\Users\Karl\Desktop\UnderworldStudy\Seeing what is being fixed by UltimateEditor\arkcleaned_nodoors_fixed.bin");
        
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
                if (TileMapBlockNoDoors.FreeListMobileObjectBuffer[i] != TileMapBlockNoDoorsFixed.FreeListMobileObjectBuffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of FreeListMobileObject (NoDoors-Fixed)");
                if (TileMapBlockCleaned.FreeListMobileObjectBuffer[i] != TileMapBlockNoDoorsFixed.FreeListMobileObjectBuffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of FreeListMobileObject (Cleaned-Fixed)");
            }
            
            for (int i = 0; i < TileMapBlockNoDoors.FreeListStaticObjectBuffer.Length; i++)
            {
                // Assert.True(TileMapBlockOriginal.FreeListStaticObjectBuffer[i] == TileMapBlockToModify.FreeListStaticObjectBuffer[i]);
                if (TileMapBlockNoDoors.FreeListStaticObjectBuffer[i] != TileMapBlockNoDoorsFixed.FreeListStaticObjectBuffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of FreeListStaticObject (NoDoors-Fixed)");
                if (TileMapBlockCleaned.FreeListStaticObjectBuffer[i] != TileMapBlockNoDoorsFixed.FreeListStaticObjectBuffer[i])
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
                        Console.WriteLine($"Detected difference in byte {i} of StaticObject number {objnum} buffer (NoDoors-Fixed)");
                    if (objCleaned.Buffer[i] != objFixed.Buffer[i])
                        Console.WriteLine($"Detected difference in byte {i} of StaticObject number {objnum} buffer (Cleaned-Fixed)");
                }
            }
            
            // Other blocks
            for (int i = 0; i < AutoMapBlockNoDoors.blockbuffer.Length; i++)
            {
                // Assert.True(AutoMapBlockOriginal.blockbuffer[i] == AutoMapBlockToModify.blockbuffer[i]);
                if (AutoMapBlockNoDoors.blockbuffer[i] != AutoMapBlockNoDoorsFixed.blockbuffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of AutoMapBlock (NoDoors-Fixed)");
                if (AutoMapBlockCleaned.blockbuffer[i] != AutoMapBlockNoDoorsFixed.blockbuffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of AutoMapBlock (Cleaned-Fixed)");
                    
            }

            for (int i = 0; i < MapNotesNoDoors.blockbuffer.Length; i++)
            {
                // Assert.True(MapNotesOriginal.blockbuffer[i] == MapNotesToModify.blockbuffer[i]);
                if (MapNotesNoDoors.blockbuffer[i] != MapNotesNoDoorsFixed.blockbuffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of MapNotes (NoDoors-Fixed)");
                if (MapNotesCleaned.blockbuffer[i] != MapNotesNoDoorsFixed.blockbuffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of MapNotes (Cleaned-Fixed)");
            }

            for (int i = 0; i < ObjectAnimOverlayNoDoors.blockbuffer.Length; i++)
            {
                // Assert.True(ObjectAnimOverlayOriginal.blockbuffer[i] == ObjectAnimOverlayToModify.blockbuffer[i]);
                if (ObjectAnimOverlayNoDoors.blockbuffer[i] != ObjectAnimOverlayNoDoorsFixed.blockbuffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of ObjAnimOverlay (NoDoors-Fixed)");
                if (ObjectAnimOverlayCleaned.blockbuffer[i] != ObjectAnimOverlayNoDoorsFixed.blockbuffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of ObjAnimOverlay (Cleaned-Fixed)");
            }

            for (int i = 0; i < TextureMappingNoDoors.blockbuffer.Length; i++)
            {
                // Assert.True(TextureMappingOriginal.blockbuffer[i] == TextureMappingToModify.blockbuffer[i]);
                if (TextureMappingNoDoors.blockbuffer[i] != TextureMappingNoDoorsFixed.blockbuffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of TextureMapping (NoDoors-Fixed)");
                if (TextureMappingCleaned.blockbuffer[i] != TextureMappingNoDoorsFixed.blockbuffer[i])
                    Console.WriteLine($"Detected difference in byte {i} of TextureMapping (Cleaned-Fixed)");
            }

        }
        

    }
}