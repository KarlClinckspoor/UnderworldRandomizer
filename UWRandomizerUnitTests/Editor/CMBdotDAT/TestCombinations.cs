using System.Linq;
using NUnit.Framework;
using UWRandomizerEditor.CMBdotDAT;

namespace RandomizerUnitTests;

[TestFixture]
public class TestCombinations
{
    [Test]
    public void TestCtorItems()
    {
        var comb = new ItemCombination(
            new ItemDescriptor(1, false),
            new ItemDescriptor(2, true),
            new ItemDescriptor(3, false)
        );
        var correctBuffer = new byte[] {1, 0, 2, 0b1000_0000, 3, 0};
        Assert.True(comb.Buffer.SequenceEqual(correctBuffer));
    }

    [Test]
    public void TestCtorBuffer()
    {
        var comb = new ItemCombination( new byte[] {1, 0, 2, 0b1000_0000, 3, 0} );
        Assert.True(comb.FirstItem.ItemID == 1);
        Assert.True(comb.FirstItem.IsDestroyed == false);
        Assert.True(comb.SecondItem.ItemID == 2);
        Assert.True(comb.SecondItem.IsDestroyed == true);
        Assert.True(comb.Product.ItemID == 3);
        Assert.True(comb.Product.IsDestroyed == false);
    }

    [Test]
    public void TestCtorBufferWrongSize()
    {
        Assert.Throws<ItemCombinationException>(() => new ItemCombination( new byte[] {0} ));
        Assert.Throws<ItemCombinationException>(() => new ItemCombination( new byte[] {0, 1, 2, 3, 4, 5, 6} ));
    }
    
    [Test]
    public void TestBufferSetter()
    {
        var correct = new ItemCombination(
            new ItemDescriptor(1, false),
            new ItemDescriptor(2, true),
            new ItemDescriptor(3, false)
        );
        var toSet = new ItemCombination(
            new ItemDescriptor(0, false),
            new ItemDescriptor(0, false),
            new ItemDescriptor(0, false)
        );
        var newBuffer = new byte[] {1, 0, 2, 0b1000_0000, 3, 0};
        toSet.Buffer = newBuffer;
        
        Assert.True(correct.Buffer.SequenceEqual(toSet.Buffer));
    }

    [Test]
    public void TestIsValidItemCombination()
    {
        var invalid1 = new ItemCombination(
            new ItemDescriptor(1, false),
            new ItemDescriptor(2, false),
            new ItemDescriptor(3, false)
        );
        Assert.False(invalid1.IsValidItemCombination());
        var valid1 = new ItemCombination(
            new ItemDescriptor(1, false),
            new ItemDescriptor(2, true),
            new ItemDescriptor(3, false)
        );
        Assert.False(invalid1.IsValidItemCombination());
        Assert.True(valid1.IsValidItemCombination());
    }
}