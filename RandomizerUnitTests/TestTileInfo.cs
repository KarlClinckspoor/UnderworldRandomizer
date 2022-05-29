using System;
using System.Security.Cryptography;
using NUnit.Framework;
using Randomizer;
namespace RandomizerUnitTests;

public class TestTileInfo
{
    // Let's create one random entry from an int.
    // And another from a buffer
    private TileInfo tinfo1 = new TileInfo(0, 240, 0, 0);
    private TileInfo tinfo2 = new TileInfo(0, BitConverter.GetBytes(240), 0, 0);
    
    [SetUp]
    public void Setup()
    {
    }

    public bool CompareTwoTileInfos(TileInfo first, TileInfo second)
    {
        // Let's see all properties
        if (first.Entry == second.Entry &
            CompareArrays(first.TileBuffer, second.TileBuffer) &
            first.EntryNum == second.EntryNum &
            first.LevelNum == second.LevelNum &
            first.TileType == second.TileType &
            first.TileHeight == second.TileHeight &
            first.Light == second.Light &
            first.FloorTextureIdx == second.FloorTextureIdx &
            first.NoMagic == second.NoMagic &
            first.DoorBit == second.DoorBit &
            first.WallTextureIdx == second.WallTextureIdx &
            first.FirstObjIdx == second.FirstObjIdx)
            return true;
        return false;
    }

    [Test]
    public void ComparingConstructors()
    {
        Assert.True(CompareTwoTileInfos(tinfo1, tinfo2));   
    }

    // todo: break up this massive test. Remove hardcoded paths
    [Test]
    public void SavingBufferAndReloading()
    {
        // Compare the buffers as-is
        Assert.True(CompareArrays(tinfo1.TileBuffer, tinfo2.TileBuffer));
        tinfo1.SaveBuffer(basepath:@"D:\Dropbox\UnderworldStudy\studies\tests", extrainfo:"buffer_tinfo1");
        tinfo2.SaveBuffer(basepath:@"D:\Dropbox\UnderworldStudy\studies\tests", extrainfo:"buffer_tinfo2");
        
        // Compare their hashes
        SHA256 mySHA256 = SHA256.Create();
        var tinfo1Hash = mySHA256.ComputeHash(tinfo1.TileBuffer);
        var tinfo2Hash = mySHA256.ComputeHash(tinfo2.TileBuffer);
        Assert.True(CompareArrays(tinfo1Hash, tinfo2Hash));

        // Reload the buffers
        var tinfo1RelBuffer = LoadTileData(@"D:\Dropbox\UnderworldStudy\studies\tests\buffer_tinfo1.bin");
        var tinfo2RelBuffer = LoadTileData(@"D:\Dropbox\UnderworldStudy\studies\tests\buffer_tinfo2.bin");
        
        // Compare the hashes again
        var rectinfo1Hash = mySHA256.ComputeHash(tinfo1RelBuffer);
        var rectinfo2Hash = mySHA256.ComputeHash(tinfo2RelBuffer);
        Assert.True(CompareArrays(rectinfo1Hash, rectinfo2Hash));
        
        // Compare the buffers
        Assert.True(CompareArrays(tinfo1RelBuffer, tinfo2RelBuffer));
        
        // Rebuild the objects
        var rebuiltTinfo1 = new TileInfo(0, tinfo1RelBuffer, 0, 0);
        var rebuiltTinfo2 = new TileInfo(0, tinfo2RelBuffer, 0, 0);
        
        // Compare their buffers
        Assert.True(CompareArrays(rebuiltTinfo1.TileBuffer, rebuiltTinfo2.TileBuffer));
        Assert.True(CompareArrays(rebuiltTinfo1.TileBuffer, tinfo1.TileBuffer));
        Assert.True(CompareArrays(rebuiltTinfo2.TileBuffer, tinfo2.TileBuffer));
        Assert.True(CompareArrays(rebuiltTinfo1.TileBuffer, tinfo2.TileBuffer));
        Assert.True(CompareArrays(rebuiltTinfo2.TileBuffer, tinfo1.TileBuffer));
        
        // Compare instances
        Assert.True(CompareTwoTileInfos(tinfo1, tinfo1));
        Assert.True(CompareTwoTileInfos(tinfo1, tinfo2));
        Assert.True(CompareTwoTileInfos(tinfo2, tinfo2));
        Assert.True(CompareTwoTileInfos(tinfo2, tinfo2));
        
        Assert.True(CompareTwoTileInfos(rebuiltTinfo1, tinfo1));
        Assert.True(CompareTwoTileInfos(rebuiltTinfo1, tinfo2));
        Assert.True(CompareTwoTileInfos(rebuiltTinfo1, rebuiltTinfo2));
        Assert.True(CompareTwoTileInfos(rebuiltTinfo2, rebuiltTinfo1));
    }

    // todo: move this elsewhere
    public bool CompareArrays(byte[] arr1, byte[] arr2)
    {
        if (arr1.Length != arr2.Length)
            return false;
        for (int i = 0; i < arr1.Length; i++)
        {
            if (arr1[i] != arr2[i])
            {
                return false;
            }
        }

        return true;
    }

    public byte[] LoadTileData(string path)
    {
        return System.IO.File.ReadAllBytes(path);
    }
    
}