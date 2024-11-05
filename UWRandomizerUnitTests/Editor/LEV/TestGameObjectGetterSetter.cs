using System;
using System.Linq;
using NUnit.Framework;
using UWRandomizerEditor.Interfaces;
using UWRandomizerEditor.LEV;
using UWRandomizerEditor.LEV.GameObjects;

namespace RandomizerUnitTests.Editor.LEV;

[TestFixture]
[Category("RequiresSettings")]
public class TestObjectGetterSetter
{
#pragma warning disable CS8618
    private LevLoader ark;
#pragma warning restore CS8618
    
    [OneTimeSetUp]
    public void SetUp()
    {
        ark = new LevLoader(Paths.UW1_ArkOriginalPath);
    }

    private byte[] CreateBufferCopy(IBufferObject obj)
    {
        byte[] bufferCopy = new byte[obj.Buffer.Length];
        obj.Buffer.CopyTo(bufferCopy, 0);
        return bufferCopy;
    }

    private void IterAllObjectsAndExecuteAction(Action<GameObject> action)
    {
        foreach (var tilemap in ark.MapObjBlocks)
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

    // Mobile object stuff
    [Test]
    public void HP()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            if (obj is MobileObject mobj)
            {
                var cpy = mobj.HP;
                mobj.HP = cpy;
            }
        });
    }
    [Test]
    public void byte2_unk()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            if (obj is MobileObject mobj)
            {
                var cpy = mobj.byte2_unk;
                mobj.byte2_unk = cpy;
            }
        });
    }
    
    [Test]
    public void byte3_unk()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            if (obj is MobileObject mobj)
            {
                var cpy = mobj.byte3_unk;
                mobj.byte3_unk = cpy;
            }
        });
    }
    
    [Test]
    public void NPCGoalGtarg()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            if (obj is MobileObject mobj)
            {
                var cpy = mobj.NPCGoalGtarg;
                mobj.NPCGoalGtarg = cpy;
            }
        });
    }
    
    
    [Test]
    public void NPCLevelTalkedAttitude()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            if (obj is MobileObject mobj)
            {
                var cpy = mobj.NPCLevelTalkedAttitude;
                mobj.NPCLevelTalkedAttitude = cpy;
            }
        });
    }
    
    [Test]
    public void NPCheightQM()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            if (obj is MobileObject mobj)
            {
                var cpy = mobj.NPCheightQM;
                mobj.NPCheightQM = cpy;
            }
        });
    }
    
    [Test]
    public void byte4_unk()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            if (obj is MobileObject mobj)
            {
                var cpy = mobj.byte4_unk;
                mobj.byte4_unk = cpy;
            }
        });
    }
    
    [Test]
    public void byte5_unk()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            if (obj is MobileObject mobj)
            {
                var cpy = mobj.byte5_unk;
                mobj.byte5_unk = cpy;
            }
        });
    }
    
    [Test]
    public void byte6_unk()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            if (obj is MobileObject mobj)
            {
                var cpy = mobj.byte6_unk;
                mobj.byte6_unk = cpy;
            }
        });
    }
    
    [Test]
    public void byte7_unk()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            if (obj is MobileObject mobj)
            {
                var cpy = mobj.byte7_unk;
                mobj.byte7_unk = cpy;
            }
        });
    }
    
    [Test]
    public void byte8_unk()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            if (obj is MobileObject mobj)
            {
                var cpy = mobj.byte8_unk;
                mobj.byte8_unk = cpy;
            }
        });
    }
    
    [Test]
    public void NPChome()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            if (obj is MobileObject mobj)
            {
                var cpy = mobj.NPChome;
                mobj.NPChome = cpy;
            }
        });
    }
    
    [Test]
    public void NPCheading()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            if (obj is MobileObject mobj)
            {
                var cpy = mobj.NPCHeading;
                mobj.NPCHeading = cpy;
            }
        });
    }
    
    [Test]
    public void NPChunger()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            if (obj is MobileObject mobj)
            {
                var cpy = mobj.byte_NPCHunger;
                mobj.byte_NPCHunger = cpy;
            }
        });
    }
    
    [Test]
    public void whoami()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            if (obj is MobileObject mobj)
            {
                var cpy = mobj.NPCwhoami;
                mobj.NPCwhoami = cpy;
            }
        });
    }
    
    [Test]
    public void Goal()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            if (obj is MobileObject mobj)
            {
                var cpy = mobj.Goal;
                mobj.Goal = cpy;
            }
        });
    }
    [Test]
    public void Gtarg()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            if (obj is MobileObject mobj)
            {
                var cpy = mobj.Gtarg;
                mobj.Gtarg = cpy;
            }
        });
    }
    [Test]
    public void Level()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            if (obj is MobileObject mobj)
            {
                var cpy = mobj.Level;
                mobj.Level = cpy;
            }
        });
    }
    [Test]
    public void TalkedTo()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            if (obj is MobileObject mobj)
            {
                var cpy = mobj.TalkedTo;
                mobj.TalkedTo = cpy;
            }
        });
    }
    [Test]
    public void Attitude()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            if (obj is MobileObject mobj)
            {
                var cpy = mobj.Attitude;
                mobj.Attitude = cpy;
            }
        });
    }
    [Test]
    public void Height()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            if (obj is MobileObject mobj)
            {
                var cpy = mobj.Height;
                mobj.Height = cpy;
            }
        });
    }
    [Test]
    public void YHome()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            if (obj is MobileObject mobj)
            {
                var cpy = mobj.YHome;
                mobj.YHome = cpy;
            }
        });
    }
    [Test]
    public void XHome()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            if (obj is MobileObject mobj)
            {
                var cpy = mobj.XHome;
                mobj.XHome = cpy;
            }
        });
    }
    [Test]
    public void Hunger()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            if (obj is MobileObject mobj)
            {
                var cpy = mobj.Hunger;
                mobj.Hunger = cpy;
            }
        });
    }
    [Test]
    public void whoami2()
    {
        IterAllObjectsAndExecuteAction(obj =>
        {
            if (obj is MobileObject mobj)
            {
                var cpy = mobj.whoami;
                mobj.whoami = cpy;
            }
        });
    }
    
}