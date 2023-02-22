using System;
using System.Configuration;
using System.Linq;
using NUnit.Framework;
using UWRandomizerEditor.Interfaces;
using UWRandomizerEditor.LEVdotARK;
using UWRandomizerEditor.LEVdotARK.GameObjects;

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
    public void ItemID()
    {
        IterAllObjectsAndExecuteAction((GameObject obj) =>
        {
            var cpy = obj.ItemID;
            obj.ItemID = cpy;
        });
    }

    [Test]
    public void Flags()
    {
        IterAllObjectsAndExecuteAction((GameObject obj) =>
        {
            var cpy = obj.Flags;
            obj.Flags = cpy;
        });
    }

    [Test]
    public void EnchantFlag()
    {
        IterAllObjectsAndExecuteAction((GameObject obj) =>
        {
            var cpy = obj.EnchantFlag;
            obj.EnchantFlag = cpy;
        });
    }

    [Test]
    public void DoorDir()
    {
        IterAllObjectsAndExecuteAction((GameObject obj) =>
        {
            var cpy = obj.DoorDir;
            obj.DoorDir = cpy;
        });
    }

    [Test]
    public void Invis()
    {
        IterAllObjectsAndExecuteAction((GameObject obj) =>
        {
            var cpy = obj.Invis;
            obj.Invis = cpy;
        });
    }

    [Test]
    public void IsQuant()
    {
        IterAllObjectsAndExecuteAction((GameObject obj) =>
        {
            var cpy = obj.IsQuant;
            obj.IsQuant = cpy;
        });
    }

    [Test]
    public void Zpos()
    {
        IterAllObjectsAndExecuteAction((GameObject obj) =>
        {
            var cpy = obj.Zpos;
            obj.Zpos = cpy;
        });
    }
    [Test]
    public void Heading()
    {
        IterAllObjectsAndExecuteAction((GameObject obj) =>
        {
            var cpy = obj.Heading;
            obj.Heading = cpy;
        });
    }
    [Test]
    public void Ypos()
    {
        IterAllObjectsAndExecuteAction((GameObject obj) =>
        {
            var cpy = obj.Ypos;
            obj.Ypos = cpy;
        });
    }
    [Test]
    public void Xpos()
    {
        IterAllObjectsAndExecuteAction((GameObject obj) =>
        {
            var cpy = obj.Xpos;
            obj.Xpos = cpy;
        });
    }

    [Test]
    public void Quality()
    {
        IterAllObjectsAndExecuteAction((GameObject obj) =>
        {
            var cpy = obj.Quality;
            obj.Quality = cpy;
        });
    }
    
    [Test]
    public void next()
    {
        IterAllObjectsAndExecuteAction((GameObject obj) =>
        {
            var cpy = obj.next;
            obj.next = cpy;
        });
    }
    [Test]
    public void OwnerOrSpecial()
    {
        IterAllObjectsAndExecuteAction((GameObject obj) =>
        {
            var cpy = obj.OwnerOrSpecial;
            obj.OwnerOrSpecial = cpy;
        });
    }
    [Test]
    public void QuantityOrSpecialLinkOrSpecialProperty()
    {
        IterAllObjectsAndExecuteAction((GameObject obj) =>
        {
            var cpy = obj.QuantityOrSpecialLinkOrSpecialProperty;
            obj.QuantityOrSpecialLinkOrSpecialProperty = cpy;
        });
    }
    [Test]
    public void ItemOwnerStrIdx()
    {
        IterAllObjectsAndExecuteAction((GameObject obj) =>
        {
            var cpy = obj.ItemOwnerStrIdx;
            obj.ItemOwnerStrIdx = cpy;
        });
    }
    
    
}