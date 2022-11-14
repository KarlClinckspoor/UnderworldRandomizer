using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using NUnit.Framework;
using UWRandomizerEditor;
using UWRandomizerEditor.LEVDotARK;
using UWRandomizerEditor.LEVDotARK.GameObjects;

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
        for (int blocknum = 0; blocknum < numOfLevels; blocknum++)
        {
            // Jesus this looks ugly. But it's only Loading the jsons into the lists, and the appropriate ArkLoader isntances
            // streamsPristine[blocknum] =
            //     Assembly.GetExecutingAssembly()
            //         .GetManifestResourceStream(
            //             $"RandomizerUnitTests.testdata.PristineUW1.Block{blocknum}_objects.json") ??
            //     throw new InvalidOperationException();
            streamsPristine[blocknum] =
                File.ReadAllText(
                    @$"C:\Users\Karl\Desktop\UnderworldStudy\UnderworldRandomizer\RandomizerUnitTests\testdata\PristineUW1\Block{blocknum}_objects.json");
            jsonsPristine.Add(JsonSerializer.Deserialize<List<Dictionary<string, int>>>(streamsPristine[blocknum],
                new JsonSerializerOptions() {AllowTrailingCommas = true}) ?? throw new InvalidOperationException());
            arkPristine = new ArkLoader(Settings.DefaultArkPath);

            // streamsCleaned[blocknum] =
            //     Assembly.GetExecutingAssembly()
            //         .GetManifestResourceStream(
            //             $"RandomizerUnitTests.testdata.CleanedUW1.Block{blocknum}_objects.json") ??
            //     throw new InvalidOperationException();
            streamsCleaned[blocknum] = File.ReadAllText(
                    @$"C:\Users\Karl\Desktop\UnderworldStudy\UnderworldRandomizer\RandomizerUnitTests\testdata\CleanedUW1\Block{blocknum}_objects.json");
            jsonsCleaned.Add(JsonSerializer.Deserialize<List<Dictionary<string, int>>>(
                streamsCleaned[blocknum],
                new JsonSerializerOptions() {AllowTrailingCommas = true}) ?? throw new InvalidOperationException());
            // TODO: De-hardcode this
            arkCleaned = new ArkLoader(@"C:\Users\Karl\Desktop\UnderworldStudy\UW - Copy\DATA\LEV.ARK");
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
}