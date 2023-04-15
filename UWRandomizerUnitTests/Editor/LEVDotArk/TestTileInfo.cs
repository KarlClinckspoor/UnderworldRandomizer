using System;
using System.Linq;
using System.Security.Cryptography;
using NUnit.Framework;
using UWRandomizerEditor.LEVdotARK;

namespace RandomizerUnitTests.Editor.LEVDotArk;

[TestFixture]
public class TestTileInfo
{
    // Let's create one random entry from an int.
    // And another from a buffer
#pragma warning disable CS8618
    private Tile tinfo1;
    private Tile tinfo2;
    private Tile tinfo3;
#pragma warning restore CS8618

    [SetUp]
    public void Setup()
    {
        tinfo1 = new Tile(0, 240, 0, 0);
        tinfo2 = new Tile(0, BitConverter.GetBytes(240), 0, 0);
        tinfo3 = new Tile(0, BitConverter.GetBytes(241), 0, 0);
    }

    [Test]
    public void ComparingConstructors()
    {
        Assert.True(tinfo1.Equals(tinfo2));
        Assert.False(tinfo1.Equals(tinfo3));
        Assert.False(tinfo2.Equals(tinfo3));
    }

    [Category("RequiresSettings")]
    [Test]
    public void SavingBufferAndReloading()
    {
        // Compare the buffers as-is
        Assert.True(tinfo1.Buffer.SequenceEqual(tinfo2.Buffer));
        string tinfo1Path =
            UWRandomizerEditor.Utils.SaveBuffer(tinfo1, Paths.BufferTestsPath, filename: "buffer_tinfo1");
        string tinfo2Path =
            UWRandomizerEditor.Utils.SaveBuffer(tinfo2, Paths.BufferTestsPath, filename: "buffer_tinfo2");
        string tinfo3Path =
            UWRandomizerEditor.Utils.SaveBuffer(tinfo3, Paths.BufferTestsPath, filename: "buffer_tinfo3");

        // Compare their hashes
        SHA256 mySHA256 = SHA256.Create();
        var tinfo1Hash = mySHA256.ComputeHash(tinfo1.Buffer);
        var tinfo2Hash = mySHA256.ComputeHash(tinfo2.Buffer);
        var tinfo3Hash = mySHA256.ComputeHash(tinfo3.Buffer);
        Assert.True(tinfo1Hash.SequenceEqual(tinfo2Hash));
        Assert.False(tinfo1Hash.SequenceEqual(tinfo3Hash));
        Assert.False(tinfo2Hash.SequenceEqual(tinfo3Hash));

        // Reload the buffers
        var tinfo1RelBuffer = LoadTileData(tinfo1Path);
        var tinfo2RelBuffer = LoadTileData(tinfo2Path);
        var tinfo3RelBuffer = LoadTileData(tinfo3Path);

        // Compare the hashes again
        var rectinfo1Hash = mySHA256.ComputeHash(tinfo1RelBuffer);
        var rectinfo2Hash = mySHA256.ComputeHash(tinfo2RelBuffer);
        var rectinfo3Hash = mySHA256.ComputeHash(tinfo3RelBuffer);
        Assert.True(rectinfo1Hash.SequenceEqual(rectinfo2Hash));
        Assert.False(rectinfo2Hash.SequenceEqual(rectinfo3Hash));
        Assert.False(rectinfo2Hash.SequenceEqual(rectinfo3Hash));

        // Compare the buffers
        Assert.True(tinfo1RelBuffer.SequenceEqual(tinfo2RelBuffer));
        Assert.False(tinfo1RelBuffer.SequenceEqual(tinfo3RelBuffer));
        Assert.False(tinfo2RelBuffer.SequenceEqual(tinfo3RelBuffer));

        // Rebuild the objects
        var rebuiltTinfo1 = new Tile(0, tinfo1RelBuffer, 0, 0);
        var rebuiltTinfo2 = new Tile(0, tinfo2RelBuffer, 0, 0);
        var rebuiltTinfo3 = new Tile(0, tinfo3RelBuffer, 0, 0);

        // Compare their buffers
        Assert.True(rebuiltTinfo1.Buffer.SequenceEqual(rebuiltTinfo2.Buffer));
        Assert.True(rebuiltTinfo1.Buffer.SequenceEqual(tinfo1.Buffer));
        Assert.True(rebuiltTinfo2.Buffer.SequenceEqual(tinfo2.Buffer));
        Assert.True(rebuiltTinfo1.Buffer.SequenceEqual(tinfo2.Buffer));
        Assert.True(rebuiltTinfo2.Buffer.SequenceEqual(tinfo1.Buffer));
        Assert.True(rebuiltTinfo3.Buffer.SequenceEqual(tinfo3.Buffer));

        Assert.False(rebuiltTinfo1.Buffer.SequenceEqual(rebuiltTinfo3.Buffer));
        Assert.False(rebuiltTinfo2.Buffer.SequenceEqual(tinfo3.Buffer));
        Assert.False(rebuiltTinfo1.Buffer.SequenceEqual(tinfo3.Buffer));
        Assert.False(rebuiltTinfo2.Buffer.SequenceEqual(tinfo3.Buffer));

        Assert.True(rebuiltTinfo1.Equals(tinfo1));
        Assert.True(rebuiltTinfo1.Equals(tinfo2));
        Assert.True(rebuiltTinfo1.Equals(rebuiltTinfo2));
        Assert.True(rebuiltTinfo2.Equals(rebuiltTinfo1));
        Assert.False(rebuiltTinfo1.Equals(rebuiltTinfo3));
        Assert.False(rebuiltTinfo2.Equals(rebuiltTinfo3));
    }

    private byte[] LoadTileData(string path)
    {
        return System.IO.File.ReadAllBytes(path);
    }
}

[TestFixture]
public class TestGettersAndSetters
{
    [Test]
    [Category("RequiresSettings")]
    public void TestingGettersAndSettersThatModifyBuffer()
    {
        var ark = new ArkLoader(Paths.UW_ArkOriginalPath);
        foreach (var block in ark.TileMapObjectsBlocks)
        {
            foreach (var tile in block.Tiles)
            {
                byte[] tempbuffer = new byte[tile.Buffer.Length];
                tile.Buffer.CopyTo(tempbuffer, 0);
                
                tile.TileHeight = tile.TileHeight;
                tile.TileType = tile.TileType;
                tile.Bit9 = tile.Bit9;
                tile.Light = tile.Light;
                tile.DoorBit = tile.DoorBit;
                tile.NoMagic = tile.NoMagic;
                tile.FirstObjIdx = tile.FirstObjIdx;
                tile.FloorTextureIdx = tile.FloorTextureIdx;
                tile.WallTextureIdx = tile.WallTextureIdx;
                
                Assert.True(tempbuffer.SequenceEqual(tile.Buffer));
            }
        }
    }

    [Test]
    [Category("RequiresSettings")]
    public void TestCreatingTileFromSetters()
    {
        var ark = new ArkLoader(Paths.UW_ArkOriginalPath);
        foreach (var block in ark.TileMapObjectsBlocks)
        {
            foreach (var tile in block.Tiles)
            {
                var Tile = new Tile(0, new byte[4], 0, 0);
                
                Tile.TileHeight = tile.TileHeight;
                Tile.TileType = tile.TileType;
                Tile.Bit9 = tile.Bit9;
                Tile.Light = tile.Light;
                Tile.DoorBit = tile.DoorBit;
                Tile.NoMagic = tile.NoMagic;
                Tile.FirstObjIdx = tile.FirstObjIdx;
                Tile.FloorTextureIdx = tile.FloorTextureIdx;
                Tile.WallTextureIdx = tile.WallTextureIdx;
                
                Assert.True(Tile.Buffer.SequenceEqual(tile.Buffer));
            }
        }
        
    }
}
