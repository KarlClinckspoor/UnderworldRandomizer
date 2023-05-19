using System.Linq;
using NUnit.Framework;
using UWRandomizerEditor.CMBdotDAT;

namespace RandomizerUnitTests.Editor.CMBdotDAT;

[TestFixture]
public class TestFinalCombination
{
    [Test]
    public void TestCtor()
    {
        var comb = new FinalCombination();
        Assert.True(comb.Buffer.SequenceEqual(new byte[]{0, 0, 0, 0, 0, 0}));
    }
}