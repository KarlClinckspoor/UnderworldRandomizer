using System;
using NUnit.Framework;
using UWRandomizerEditor.CMB;

namespace RandomizerUnitTests.Editor.CMB;

[TestFixture]
public class TestExceptions
{
    [Test]
    public void TestExceptionThrows()
    {
        Assert.Throws<ItemCombinationException>(() => throw new ItemCombinationException());
        Assert.Throws<ItemCombinationException>(() => throw new ItemCombinationException("Message"));
        Assert.Throws<ItemCombinationException>(() => throw new ItemCombinationException("Other message", new Exception()));
    }
}