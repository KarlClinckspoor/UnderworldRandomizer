using System;
using System.Linq;
using NUnit.Framework;
using UWRandomizerEditor.Interfaces;
using UWRandomizerEditor.LEVdotARK;
using UWRandomizerEditor.LEVdotARK.GameObjects;

namespace RandomizerUnitTests.Editor.LEVDotArk;

[TestFixture]
[Category("RequiresSettings")]
public class TestObjectGetterSetter
{
#pragma warning disable CS8618
    private ArkLoader ark;
#pragma warning restore CS8618
    
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
        IterAllObjectsAndExecuteAction(obj =>
        {
            var cpy = obj.ItemID;
            obj.ItemID = cpy;
        });
    }

    [Test]
    public void Flags()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            var cpy = obj.Flags;
            obj.Flags = cpy;
        });
    }

    [Test]
    public void EnchantFlag()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            var cpy = obj.EnchantFlag;
            obj.EnchantFlag = cpy;
        });
    }

    [Test]
    public void DoorDir()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            var cpy = obj.DoorDir;
            obj.DoorDir = cpy;
        });
    }

    [Test]
    public void Invis()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            var cpy = obj.Invis;
            obj.Invis = cpy;
        });
    }

    [Test]
    public void IsQuant()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            var cpy = obj.IsQuant;
            obj.IsQuant = cpy;
        });
    }

    [Test]
    public void Zpos()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            var cpy = obj.Zpos;
            obj.Zpos = cpy;
        });
    }
    [Test]
    public void Heading()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            var cpy = obj.Heading;
            obj.Heading = cpy;
        });
    }
    [Test]
    public void Ypos()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            var cpy = obj.Ypos;
            obj.Ypos = cpy;
        });
    }
    [Test]
    public void Xpos()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            var cpy = obj.Xpos;
            obj.Xpos = cpy;
        });
    }

    [Test]
    public void Quality()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            var cpy = obj.Quality;
            obj.Quality = cpy;
        });
    }
    
    [Test]
    public void next()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            var cpy = obj.next;
            obj.next = cpy;
        });
    }
    [Test]
    public void OwnerOrSpecial()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            var cpy = obj.OwnerOrSpecial;
            obj.OwnerOrSpecial = cpy;
        });
    }
    [Test]
    public void QuantityOrSpecialLinkOrSpecialProperty()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            var cpy = obj.QuantityOrSpecialLinkOrSpecialProperty;
            obj.QuantityOrSpecialLinkOrSpecialProperty = cpy;
        });
    }
    [Test]
    public void ItemOwnerStrIdx()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            var cpy = obj.ItemOwnerStrIdx;
            obj.ItemOwnerStrIdx = cpy;
        });
    }
    
    
}