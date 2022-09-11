using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using NUnit.Framework;
using Randomizer;
using Randomizer.LEVDotARK;
using Randomizer.LEVDotARK.GameObjects;
using Randomizer.LEVDotARK.GameObjects.Specifics;

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
        Assert.True(tinfo1.TileBuffer.SequenceEqual(tinfo2.TileBuffer));
        string tinfo1Path = tinfo1.SaveBuffer(basePath: Settings.DefaultBinaryTestsPath, extraInfo: "buffer_tinfo1");
        string tinfo2Path = tinfo2.SaveBuffer(basePath: Settings.DefaultBinaryTestsPath, extraInfo: "buffer_tinfo2");
        string tinfo3Path = tinfo3.SaveBuffer(basePath: Settings.DefaultBinaryTestsPath, extraInfo: "buffer_tinfo3");
        
        // Compare their hashes
        SHA256 mySHA256 = SHA256.Create();
        var tinfo1Hash = mySHA256.ComputeHash(tinfo1.TileBuffer);
        var tinfo2Hash = mySHA256.ComputeHash(tinfo2.TileBuffer);
        var tinfo3Hash = mySHA256.ComputeHash(tinfo3.TileBuffer);
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
        Assert.True(rebuiltTinfo1.TileBuffer.SequenceEqual(rebuiltTinfo2.TileBuffer));
        Assert.True(rebuiltTinfo1.TileBuffer.SequenceEqual(tinfo1.TileBuffer));
        Assert.True(rebuiltTinfo2.TileBuffer.SequenceEqual(tinfo2.TileBuffer));
        Assert.True(rebuiltTinfo1.TileBuffer.SequenceEqual(tinfo2.TileBuffer));
        Assert.True(rebuiltTinfo2.TileBuffer.SequenceEqual(tinfo1.TileBuffer));
        Assert.True(rebuiltTinfo3.TileBuffer.SequenceEqual(tinfo3.TileBuffer));
        
        Assert.False(rebuiltTinfo1.TileBuffer.SequenceEqual(rebuiltTinfo3.TileBuffer));
        Assert.False(rebuiltTinfo2.TileBuffer.SequenceEqual(tinfo3.TileBuffer));
        Assert.False(rebuiltTinfo1.TileBuffer.SequenceEqual(tinfo3.TileBuffer));
        Assert.False(rebuiltTinfo2.TileBuffer.SequenceEqual(tinfo3.TileBuffer));

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

[TestFixture]
public class TestTileInfoWithUWLinkedList
{
    private TileInfo tinfo1;
    private TileInfo tinfo2;
    // private TileInfo tinfo3;
    private List<GameObject> _gameObjects;
    // private IDictionary<string, bool> shouldBeMoved;

    [SetUp]
    public void Setup()
    {
        tinfo1 = new TileInfo(0, 240, 0, 0);
        tinfo2 = new TileInfo(0, 240, 0, 0);
        // tinfo3 = new TileInfo(0, BitConverter.GetBytes(241), 0, 0);
        var fillerBytes = new byte[] {0, 0, 0, 0, 0, 0, 0, 0};
        _gameObjects = new List<GameObject>()
        {
            new StaticObject(fillerBytes, 0) {next = 0}, // end TODO: What is the buffer of object 0?
            new QuantityGameObject(fillerBytes, 1) {next = 2, ShouldBeMoved = true}, // 1.1   movable
            new Door(fillerBytes, 2) {next = 6, ShouldBeMoved = false}, // 1.2         immovable
            new Trap(fillerBytes, 3) {next = 4, ShouldBeMoved = false}, // 2.1         immovable
            new TexturedGameObject(fillerBytes, 4) {next = 5, ShouldBeMoved = false}, // 2.2   immovable
            new EnchantedArmor(fillerBytes, 5) {next = 0, ShouldBeMoved = true}, // 2.3 (last) movable
            new EnchantedWeapon(fillerBytes, 6) {next = 0, ShouldBeMoved = true} // 1.3 (last) movable
        };
    }

    [Test]    
    public void TestChainFunctions() {     
         tinfo1.FirstObjIdx = 1; // 1.1
         tinfo2.FirstObjIdx = 3; // 2.1
         
         
         tinfo1.ObjectChain.PopulateObjectList(_gameObjects.ToArray());
         tinfo2.ObjectChain.PopulateObjectList(_gameObjects.ToArray());
         
         Assert.True(tinfo1.ObjectChain[0] == _gameObjects[1]);
         Assert.True(tinfo1.ObjectChain[^1] == _gameObjects[6]);
         Assert.True(tinfo1.ObjectChain.Count == 3);
         Assert.True(tinfo1.ObjectChain.CheckIntegrity());

         Assert.True(tinfo2.ObjectChain[0] == _gameObjects[3]);
         Assert.True(tinfo2.ObjectChain[^1] == _gameObjects[5]);
         Assert.True(tinfo2.ObjectChain.Count == 3);
         Assert.True(tinfo2.ObjectChain.CheckIntegrity());

         var info1Pop = tinfo1.ObjectChain.Pop();
         Assert.True(info1Pop == _gameObjects[6]);
         Assert.True(tinfo1.ObjectChain.Count == 2);

         var info2Pops = tinfo2.ObjectChain.PopObjectsThatShouldBeMoved();
         Assert.True(info2Pops[0] == _gameObjects[5]);
         Assert.True(tinfo2.ObjectChain.Count == 2);
    }
}