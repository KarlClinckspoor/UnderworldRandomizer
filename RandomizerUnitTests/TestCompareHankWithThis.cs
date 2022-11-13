using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using NUnit.Framework;
using UWRandomizerEditor;
using UWRandomizerEditor.LEVDotARK;
using UWRandomizerEditor.LEVDotARK.GameObjects;

namespace RandomizerUnitTests;

[TestFixture]
public class TestArkLoading
{
    [Test]
    [Category("RequiresArk")]
    public void TestItemIDsBlocks([Range(0, 8, 1)]int blocknum)
    {
        var path =
            $@"C:\Users\Karl\Desktop\UnderworldStudy\UnderworldRandomizer\RandomizerUnitTests\testdata\Block{blocknum}_objects.json";
        // var json = JsonSerializer.Deserialize(File.ReadAllText(path), Dictionary<string, int>);
        var json = JsonSerializer.Deserialize<List<Dictionary<string, int>>>(File.ReadAllText(path), new JsonSerializerOptions() {AllowTrailingCommas = true});

        var ark = new ArkLoader(Settings.DefaultArkPath);

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

        // for (int i = 0)
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