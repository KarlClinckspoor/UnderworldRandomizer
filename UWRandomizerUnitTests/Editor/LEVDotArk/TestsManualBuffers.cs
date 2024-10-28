using NUnit.Framework;
using UWRandomizerEditor.LEV;
using UWRandomizerEditor.LEV.GameObjects.Specifics;

namespace RandomizerUnitTests.Editor.LEV;

[TestFixture]
public class ManualTests
{
    [Test]
    public void TestTileInfoComparingToUltimateEditor_manual()
    {
        var tile = new Tile(1609, new byte[] {0x11, 0x20, 0x1E, 0x00}, 6436, 0);
        // Some water tile near that island with a lurker nearby
        var reference = new Tile(1609, 0, 6436, 0);
        reference.TileType = (int) Tile.TileTypes.Open;
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
        var doorUnlocked = new Door(new byte[] {0x41, 0x01, 0x50, 0x6F, 0x28, 0x00, 0x00, 0x00}, 1012);
        var doorLocked = new Door(new byte[] {0x41, 0x01, 0x50, 0x6F, 0x28, 0x00, 0xC0, 0xF8}, 1012);
        var doorToModify = new Door(new byte[] {0x41, 0x01, 0x50, 0x6F, 0x28, 0x00, 0xC0, 0xF8}, 1012);

        Assert.False(doorLocked.Equals(doorUnlocked));
        Assert.True(doorLocked.Equals(doorToModify));
        Assert.False(doorToModify.Equals(doorUnlocked));

        doorToModify.RemoveLock();

        Assert.False(doorLocked.Equals(doorUnlocked));
        Assert.False(doorLocked.Equals(doorToModify));
        Assert.True(doorToModify.Equals(doorUnlocked));
    }
}