using System;
using NUnit.Framework;
using Randomizer;
using Randomizer.LEVDotARK.GameObjects;

namespace RandomizerUnitTests;

class TestGameObjectEquality
{
    [Test]
    public void TwoEqGameObjects()
    {
        var obj1 = new GameObject(
            objid_flagsField: 0xFFF,
            positionField: 0xFFF,
            quality_chainField: 0xFFF,
            link_specialField: 0xFFF
        );

        var obj1_copy = new GameObject(
            objid_flagsField: 0xFFF,
            positionField: 0xFFF,
            quality_chainField: 0xFFF,
            link_specialField: 0xFFF
        );

        Assert.True(obj1.Equals(obj1_copy));
    }

    [Test]
    public void TwoDiffGameObjects()
    {
        var obj1 = new GameObject(
            objid_flagsField: 0xFFF,
            positionField: 0xFFF,
            quality_chainField: 0xFFF,
            link_specialField: 0xFFF
        );

        var obj2 = new GameObject(
            objid_flagsField: 0xCCC,
            positionField: 0xCCC,
            quality_chainField: 0xCCC,
            link_specialField: 0xCCC
        );
        
        Assert.False(obj1.Equals(obj2));
    }

    [Test]
    public void GOAndQtty()
    {
        var obj1 = new GameObject(
            objid_flagsField: 0xFFF,
            positionField: 0xFFF,
            quality_chainField: 0xFFF,
            link_specialField: 0xFFF
        );
        
        var obj2 = new QuantityGameObject(
            objid_flagsField: 0xFFF,
            positionField: 0xFFF,
            quality_chainField: 0xFFF,
            link_specialField: 0xFFF
        );

        Assert.False(obj1.Equals(obj2));
    }

    [Test]
    public void StaticAndMobileObjects()
    {
        var staticObj = new GameObject(
            objid_flagsField: 0xFFF,
            positionField: 0xFFF,
            quality_chainField: 0xFFF,
            link_specialField: 0xFFF
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
