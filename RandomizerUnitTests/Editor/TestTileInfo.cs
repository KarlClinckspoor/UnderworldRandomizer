using System;
using System.Linq;
using System.Security.Cryptography;
using NUnit.Framework;
using UWRandomizerEditor.LEVDotARK;
using static UWRandomizerEditor.Utils;

namespace RandomizerUnitTests;

[TestFixture]
public class TestTileInfo
{
    // Let's create one random entry from an int.
    // And another from a buffer
    private TileInfo tinfo1;
    private TileInfo tinfo2;
    private TileInfo tinfo3;

    [SetUp]
    public void Setup()
    {
        tinfo1 = new TileInfo(0, 240, 0, 0);
        tinfo2 = new TileInfo(0, BitConverter.GetBytes(240), 0, 0);
        tinfo3 = new TileInfo(0, BitConverter.GetBytes(241), 0, 0);
    }

    [Test]
    public void ComparingConstructors()
    {
        Assert.True(tinfo1.Equals(tinfo2));
        Assert.False(tinfo1.Equals(tinfo3));
        Assert.False(tinfo2.Equals(tinfo3));
    }

    // todo: break up this massive test. Remove hardcoded paths
    [Category("RequiresSettings")]
    [Test]
    public void SavingBufferAndReloading()
    {
        // Compare the buffers as-is
        Assert.True(tinfo1.Buffer.SequenceEqual(tinfo2.Buffer));
        string tinfo1Path = StdSaveBuffer(tinfo1, Paths.BufferTestsPath, filename: "buffer_tinfo1");
        string tinfo2Path = StdSaveBuffer(tinfo2, Paths.BufferTestsPath, filename: "buffer_tinfo2");
        string tinfo3Path = StdSaveBuffer(tinfo3, Paths.BufferTestsPath, filename: "buffer_tinfo3");

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
        var rebuiltTinfo1 = new TileInfo(0, tinfo1RelBuffer, 0, 0);
        var rebuiltTinfo2 = new TileInfo(0, tinfo2RelBuffer, 0, 0);
        var rebuiltTinfo3 = new TileInfo(0, tinfo3RelBuffer, 0, 0);

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

    public byte[] LoadTileData(string path)
    {
        return System.IO.File.ReadAllBytes(path);
    }
}