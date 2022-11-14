using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using NUnit.Framework;
using UWRandomizerEditor;
using UWRandomizerEditor.LEVDotARK;
using UWRandomizerEditor.LEVDotARK.GameObjects;

namespace RandomizerUnitTests;

[TestFixture]
public class TestGameObjectLists_Cleaned
{
    private const int numOfLevels = 9;
    private Stream[] streams = new Stream[numOfLevels];
    private List<List<Dictionary<string, int>>> jsons = new List<List<Dictionary<string, int>>>(numOfLevels);

    [SetUp]
    [Category("RequiresArk")]
    public void Setup()
    {
        
    }
    
}

[TestFixture]
public class TestGameObjectLists_Pristine
{
    private const int numOfLevels = 9;
    private Stream[] streamsPristine = new Stream[numOfLevels];
    private List<List<Dictionary<string, int>>> jsonsPristine = new List<List<Dictionary<string, int>>>(numOfLevels);
    private ArkLoader arkPristine;
    
    private Stream[] streamsCleaned = new Stream[numOfLevels];
    private List<List<Dictionary<string, int>>> jsonsCleaned = new List<List<Dictionary<string, int>>>(numOfLevels);
    private ArkLoader arkCleaned;

    [SetUp]
    [Category("RequiresArk")]
    public void Setup()
    {
        for (int blocknum = 0; blocknum < numOfLevels; blocknum++)
        {
            // Jesus this looks ugly. But it's only Loading the jsons into the lists, and the appropriate ArkLoader isntances
            streamsPristine[blocknum] =
                Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream(
                        $"RandomizerUnitTests.testdata.PristineUW1.Block{blocknum}_objects.json") ??
                throw new InvalidOperationException();
            jsonsPristine.Add(JsonSerializer.Deserialize<List<Dictionary<string, int>>>(streamsPristine[blocknum],
                new JsonSerializerOptions() {AllowTrailingCommas = true}) ?? throw new InvalidOperationException());
            arkPristine = new ArkLoader(Settings.DefaultArkPath);

            streamsCleaned[blocknum] =
                Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream(
                        $"RandomizerUnitTests.testdata.CleanedUW1.Block{blocknum}_objects.json") ??
                throw new InvalidOperationException();
            jsonsCleaned.Add(JsonSerializer.Deserialize<List<Dictionary<string, int>>>(
                streamsCleaned[blocknum],
                new JsonSerializerOptions() {AllowTrailingCommas = true}) ?? throw new InvalidOperationException());
            // TODO: De-hardcode this
            arkCleaned = new ArkLoader(@"C:\Users\Karl\Desktop\UnderworldStudy\UW - Copy\DATA\LEV.ARK");
        }
    }
    
    [Test]
    [Category("RequiresArk")]
    // Range is [from, to], not [from, to[
    public void TestGameObjectIDs([Range(0, numOfLevels - 1, 1)] int blocknum, [Values(true, false)] bool pristine)
    {
        var (ark, json) = selectArkAndJson(blocknum, pristine);
        
        // For whatever reason, hank's loader has objects [0-1024], but it should have been [0-1024[, right? krokot's goes
        // goes up to 1023.
        Assert.True(json.Count - 1 == ark.TileMapObjectsBlocks[blocknum].AllGameObjects.Length);

        for (int i = 0; i < json.Count - 1; i++)
        {
            var correct = json[i];
            
            if (i < 256)
            {
                var compare = ark.TileMapObjectsBlocks[blocknum].MobileObjects[i];
                var correctID = correct["item_id"];
                Assert.True(correctID == compare.ItemID, $"Mobile object {i}: Correct: {correctID}. Got {compare.ItemID}");
            }
            else
            {
                var compare = ark.TileMapObjectsBlocks[blocknum].StaticObjects[i - 256];
                var correctID = correct["item_id"];
                Assert.True(correctID == compare.ItemID, $"Static object {i}: Correct: {correctID}. Got {compare.ItemID}");
            }
            
        }
    }

    private Tuple<ArkLoader, List<Dictionary<string, int>>> selectArkAndJson(int blocknum, bool pristine)
    {
        ArkLoader ark;
        List<Dictionary<string, int>> json;
        if (pristine)
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

    // [Test]
    // public void TestObjectFromUltimateEditor_manual()
    // {
    //     // // A torch to the west of the tile above
    //     // var obj = new StaticObject(new byte[] {0x91, 0x80, 0x48, 0x34, 0x28, 0x00, 0x40, 0x00}, 540);
    //     // var reference = new StaticObject(new byte[] {0, 0, 0, 0, 0, 0, 0, 0}, 540);
    //     // reference.Xpos = 1;
    //     // reference.Ypos = 5;
    //     // reference.Zpos = 72;
    //     // reference.IsQuant = 1;
    //     // reference.Invis = 0;
    //     // reference.EnchantFlag = 0;
    //     // reference.Heading = 0;
    //     // reference.Flags = 0;
    //     // reference.Quality = 40;
    //     // reference.Owner_or_special = 0;
    //     // reference.next = 0;
    //     // reference.ItemID = 145;
    //     // reference.QuantityOrSpecialLinkOrSpecialProperty = 1;
    //     // reference.link_specialField = 64;
    //     // reference.UpdateBuffer();
    //     //
    //     // Assert.True(obj.Equals(reference));
    // }
}