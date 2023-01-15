using System;
using System.Configuration;
using System.Linq;
using NUnit.Framework;
using UWRandomizerEditor.Interfaces;
using UWRandomizerEditor.LEVDotARK;
using UWRandomizerEditor.LEVDotARK.GameObjects;

namespace RandomizerUnitTests;

[TestFixture]
public class TestObjectGetterSetter
{
    private ArkLoader ark;
    [OneTimeSetUp]
    public void SetUp()
    {
        ark = new ArkLoader(Paths.UW_ArkOriginalPath);
    }

    private byte[] CreateBufferCopy(IBufferObject obj)
    {
        byte[] bufferCopy = new byte[obj.Buffer.Length];
        obj.Buffer.CopyTo(bufferCopy, 0);
        return bufferCopy;
    }

    private void IterAllObjectsAndExecuteAction(Action<GameObject> action)
    {
        foreach (var tilemap in ark.TileMapObjectsBlocks)
        {
            foreach (var obj in tilemap.AllGameObjects)
            {
                var bufferCopy = CreateBufferCopy(obj);
                action(obj);
                Assert.True(obj.Buffer.SequenceEqual(bufferCopy), 
                   $"Buffers differ:\n" +
                   $"Original {string.Join(',', obj.Buffer)}\n" +
                   $"Copy     {string.Join(',', bufferCopy)}");
            }
        }
    }
    
    [Test]
    public void TestItemID()
    {
        IterAllObjectsAndExecuteAction((GameObject obj) =>
        {
            var cpy = obj.ItemID;
            obj.ItemID = cpy;
        });
    }

    [Test]
    public void TestFlags()
    {
        IterAllObjectsAndExecuteAction((GameObject obj) =>
        {
            var cpy = obj.Flags;
            obj.Flags = cpy;
        });
    }
    
    
}