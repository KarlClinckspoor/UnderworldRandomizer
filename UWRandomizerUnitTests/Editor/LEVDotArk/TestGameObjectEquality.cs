using NUnit.Framework;
using UWRandomizerEditor.LEV.GameObjects;

namespace RandomizerUnitTests.Editor.LEV;

[TestFixture]
class TestGameObjectEquality
{
    [Test]
    public void TwoEqGameObjects()
    {
        var obj1 = new StaticObject(
            objIdFlags: 0xFFF,
            position: 0xFFF,
            qualityChain: 0xFFF,
            linkSpecial: 0xFFF,
            idxAtObjectArray: 2
        );

        var obj1_copy = new StaticObject(
            objIdFlags: 0xFFF,
            position: 0xFFF,
            qualityChain: 0xFFF,
            linkSpecial: 0xFFF,
            idxAtObjectArray: 2
        );

        Assert.True(obj1.Equals(obj1_copy));
    }

    [Test]
    public void TwoDiffGameObjects()
    {
        var obj1 = new StaticObject(
            objIdFlags: 0xFFF,
            position: 0xFFF,
            qualityChain: 0xFFF,
            linkSpecial: 0xFFF,
            idxAtObjectArray: 1
        );

        var obj2 = new StaticObject(
            objIdFlags: 0xCCC,
            position: 0xCCC,
            qualityChain: 0xCCC,
            linkSpecial: 0xCCC,
            idxAtObjectArray: 1
        );

        Assert.False(obj1.Equals(obj2));
    }

    [Test]
    public void GOAndQtty()
    {
        var obj1 = new StaticObject(
            objIdFlags: 0xFFF,
            position: 0xFFF,
            qualityChain: 0xFFF,
            linkSpecial: 0xFFF,
            idxAtObjectArray: 2
        );

        var obj2 = new QuantityGameObject(
            objIdFlags: 0xFFF,
            position: 0xFFF,
            qualityChain: 0xFFF,
            linkSpecial: 0xFFF,
            idxAtObjectArray: 2
        );

        Assert.False(obj1.Equals(obj2));
    }

    [Test]
    public void StaticAndMobileObjects()
    {
        var staticObj = new StaticObject(
            objIdFlags: 0xFFF,
            position: 0xFFF,
            qualityChain: 0xFFF,
            linkSpecial: 0xFFF,
            idxAtObjectArray: 2
        );

        var mobileObj = new MobileObject(staticObj.Buffer, 1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10,
            11,
            12,
            13,
            14,
            15,
            10);

        Assert.False(staticObj.Equals(mobileObj));
    }
}