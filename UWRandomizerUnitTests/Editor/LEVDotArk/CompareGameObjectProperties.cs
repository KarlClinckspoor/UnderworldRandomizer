using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using NUnit.Framework;
using UWRandomizerEditor;
using UWRandomizerEditor.LEV;
using UWRandomizerEditor.LEV.Blocks;
using UWRandomizerEditor.LEV.GameObjects;
using UWRandomizerEditor.LEV.GameObjects.Specifics;
using System.Configuration;

namespace RandomizerUnitTests;

[TestFixture]
[Category("PropertyComparisons")]
[Category("FishyTests")]
public class CompareWithHanksEditor
{
    private const int numOfLevels = 9;

    // Removing this to temporarily avoid problems with building in github actions. Keeping the same for now.
    // TODO: Can I put the .json test files without problems?
    // private Stream[] streamsPristine = new Stream[numOfLevels];
    private String[] streamsPristine = new String[numOfLevels];
    private List<List<Dictionary<string, int>>> jsonsPristine = new List<List<Dictionary<string, int>>>(numOfLevels);
    private LevLoader arkPristine;

    // private Stream[] streamsCleaned = new Stream[numOfLevels];
    private String[] streamsCleaned = new String[numOfLevels];
    private List<List<Dictionary<string, int>>> jsonsCleaned = new List<List<Dictionary<string, int>>>(numOfLevels);
    private LevLoader arkCleaned;

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
        {"owner", "OwnerOrSpecial"},
        {"link", "QuantityOrSpecialLinkOrSpecialProperty"}
    };

    [OneTimeSetUp]
    public void Setup()
    {
        for (int blocknum = 0; blocknum < numOfLevels; blocknum++)
        {
            // Jesus this looks ugly. But it's only Loading the jsons into the lists, and the appropriate ArkLoader instances
            // streamsPristine[blocknum] =
            //     File.ReadAllText(Path.Join(Paths.RUT_TestDataPath, @$"PristineUW1\Block{blocknum}_objects.json"));
            streamsPristine[blocknum] =
                File.ReadAllText(Path.Join(Paths.TestDataPath, @$"PristineUW1\Block{blocknum}_objects.json"));
            jsonsPristine.Add(JsonSerializer.Deserialize<List<Dictionary<string, int>>>(streamsPristine[blocknum],
                new JsonSerializerOptions() {AllowTrailingCommas = true}) ?? throw new InvalidOperationException());
            arkPristine = new LevLoader(Paths.UW1_ArkOriginalPath);

            streamsCleaned[blocknum] = File.ReadAllText(
                Path.Join(Paths.TestDataPath, $@"CleanedUW1\Block{blocknum}_objects.json"));
            jsonsCleaned.Add(JsonSerializer.Deserialize<List<Dictionary<string, int>>>(
                streamsCleaned[blocknum],
                new JsonSerializerOptions() {AllowTrailingCommas = true}) ?? throw new InvalidOperationException());
            arkCleaned = new LevLoader(Paths.UW1_ArkOriginalPath);
        }
    }


    [Test]
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
        LevLoader ark, string compareLabel, int blocknum)
    {
        for (int i = 0; i < json.Count - 1; i++)
        {
            var correct = json[i];

            if (i < 256)
            {
                var compare = ark.TileMapObjectsBlocks[blocknum].MobileObjects[i];
                if (compare.Invalid) continue;
                var correctProperty = correct[correctLabel];
                var compareProperty = GetPropertyValue(compare, compareLabel);
                Assert.True(correctProperty == compareProperty,
                    $"Mobile object {i}: Correct: {correctProperty}. Got {compareProperty}");
            }
            else
            {
                var compare = ark.TileMapObjectsBlocks[blocknum].StaticObjects[i - 256];
                if (compare.Invalid) continue;
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
    private Tuple<LevLoader, List<Dictionary<string, int>>> selectArkAndJson(int blocknum,
        PossibleLevArkToTest pristine)
    {
        LevLoader ark;
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

        return new Tuple<LevLoader, List<Dictionary<string, int>>>(ark, json);
    }
}