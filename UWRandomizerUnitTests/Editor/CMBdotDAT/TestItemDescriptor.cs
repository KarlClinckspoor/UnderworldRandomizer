using System.Linq;
using NUnit.Framework;
using UWRandomizerEditor.CMBdotDAT;

namespace RandomizerUnitTests;

[TestFixture]
public class TestItemDescriptor
{
    [Test]
    public void TestDescriptorCtorDefault()
    {
        var itemDescriptor = new ItemDescriptor();
        Assert.True(itemDescriptor.ItemID == 0);
        Assert.True(itemDescriptor.IsDestroyed == false);
        Assert.True(itemDescriptor.Buffer.All(x => x == 0));
        Assert.False(itemDescriptor is FinalCombination);
    }

    [Test]
    public void TestDescriptorCtor2Props()
    {
        var itemDescriptor = new ItemDescriptor(1, false);
        var itemDescriptor2 = new ItemDescriptor(1, true);

        Assert.True(itemDescriptor.ItemID == 1);
        Assert.True(itemDescriptor.IsDestroyed == false);
        Assert.True(itemDescriptor.Buffer[0] == 1);
        Assert.True(itemDescriptor.Buffer[1] == 0);

        Assert.True(itemDescriptor2.ItemID == 1);
        Assert.True(itemDescriptor2.IsDestroyed);
        Assert.True(itemDescriptor2.Buffer[0] == 1);
        Assert.True(itemDescriptor2.Buffer[1] == 0b1000_0000);

        var itemDescriptor3 = new ItemDescriptor(300, false);
        var itemDescriptor4 = new ItemDescriptor(300, true);

        Assert.True(itemDescriptor3.ItemID == 300); // Testing outside the limit of 1 byte
        Assert.True(itemDescriptor3.IsDestroyed == false);
        Assert.True(itemDescriptor3.Buffer[0] == 0b0010_1100);
        Assert.True(itemDescriptor3.Buffer[1] == 0b0000_0001);

        Assert.True(itemDescriptor4.ItemID == 300);
        Assert.True(itemDescriptor4.IsDestroyed == true);
        Assert.True(itemDescriptor4.Buffer[0] == 0b0010_1100);
        Assert.True(itemDescriptor4.Buffer[1] == 0b1000_0001);
    }

    [Test]
    public void TestDescriptorCtorBuffer()
    {
        var itemDescriptor = new ItemDescriptor(new byte[] {1, 0});
        var itemDescriptor2 = new ItemDescriptor(new byte[] {1, 0x80});

        Assert.True(itemDescriptor.ItemID == 1);
        Assert.True(itemDescriptor.IsDestroyed == false);

        Assert.True(itemDescriptor2.ItemID == 1);
        Assert.True(itemDescriptor2.IsDestroyed == true);
    }

    [Test]
    public void TestPropSetters()
    {
        var itemDescriptor = new ItemDescriptor(1, false);
        var itemDescriptor2 = new ItemDescriptor(25, true);

        var manualProperties1 = new ItemDescriptor() {ItemID = 1, IsDestroyed = false};
        var manualProperties2 = new ItemDescriptor() {ItemID = 25, IsDestroyed = true};
        
        Assert.True(itemDescriptor.Buffer.SequenceEqual(manualProperties1.Buffer));
        Assert.True(itemDescriptor2.Buffer.SequenceEqual(manualProperties2.Buffer));
    }

    [Test]
    public void TestLimitsOfItemID()
    {
        var itemDescriptor1 = new ItemDescriptor(0b0011_1111_1111_1111, false);
        var itemDescriptor2 = new ItemDescriptor(0b0000_0001_1111_1111, false);
        Assert.True(itemDescriptor1.Buffer.SequenceEqual(itemDescriptor2.Buffer));
    }

    [Test]
    public void TestDescriptorCtorWrongBufferSize()
    {
        Assert.Throws<ItemCombinationException>(()=>new ItemDescriptor(new byte[] {0, 1, 2}));
        Assert.Throws<ItemCombinationException>(()=>new ItemDescriptor(new byte[] {0}));
    }

    [Test]
    public void TestDescriptorBufferSetter()
    {
        var itemDescriptor = new ItemDescriptor(new byte[] {1, 0x80});
        Assert.Throws<ItemCombinationException>(() => itemDescriptor.Buffer = new byte[] {0});
        Assert.Throws<ItemCombinationException>(() => itemDescriptor.Buffer = new byte[] {0, 1, 2});
        Assert.DoesNotThrow(() => itemDescriptor.Buffer = new byte[] {0, 1});
    }

    [Test]
    public void TestEquality()
    {
        var desc = new ItemDescriptor(1, false);
        var descEq = new ItemDescriptor(1, false);
        var descDiff = new ItemDescriptor(2, false);
        
        Assert.False(desc.Equals(null));
        Assert.True(desc.Equals(desc));
        Assert.True(desc.Equals(descEq));
        Assert.False(desc.Equals(descDiff));
        
        Assert.False(desc.Equals((object) null));
        Assert.True(desc.Equals((object) desc));
        Assert.True(desc.Equals((object) descEq));
        Assert.False(desc.Equals((object) descDiff));
        Assert.False(desc.Equals( new FinalCombination()));
    }
}
