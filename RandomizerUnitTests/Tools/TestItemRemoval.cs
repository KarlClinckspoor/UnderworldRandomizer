using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UWRandomizerEditor.LEVDotARK;
using UWRandomizerEditor.LEVDotARK.GameObjects;
using UWRandomizerEditor.LEVDotARK.GameObjects.Specifics;
using UWRandomizerTools;

namespace RandomizerUnitTests;

public partial class TestUWLinkedList
{

    [Test]
    public void TestObjectsThatShouldBeMoved()
    {
        LList1.startingIdx = 1;
        LList2.startingIdx = 3;

        LList1.PopulateObjectList(_gameObjects.ToArray());
        LList2.PopulateObjectList(_gameObjects.ToArray());

        // LList1 has items in idx 1*,2,6*
        // LList2 has items in idx 3,4,5*
        // * means it should be moved
        var objs1 = ItemTools.ExtractMovableItems(LList1, settings);
        var objs2 = ItemTools.ExtractMovableItems(LList2, settings);

        // Now LList1 should be 2, LList2 should be 3,4
        // objs1 should be 1,6
        // objs2 should be 5
        Assert.True(LList1.Count + objs1.Count == 3); // Should be complementary
        Assert.True(objs1[0].Equals(_gameObjects[1]));
        Assert.True(LList1[0].Equals(_gameObjects[2]));
        Assert.True(objs1[1].Equals(_gameObjects[6]));

        Assert.True(LList2.Count + objs2.Count == 3);
        Assert.True(LList2[0].Equals(_gameObjects[3]));
        Assert.True(LList2[1].Equals(_gameObjects[4]));
        Assert.True(objs2[0].Equals(_gameObjects[5]));

        var empty1 = ItemTools.ExtractMovableItems(LList1, settings);
        var empty2 = ItemTools.ExtractMovableItems(LList2, settings);
        Assert.True(empty1.Count == 0);
        Assert.True(empty2.Count == 0);
        Assert.True(LList1.Count == 1);
        Assert.True(LList2.Count == 2);
    }

    [Test]
    public void TestAppendItems()
    {
        LList1.startingIdx = 1;
        LList2.startingIdx = 3;

        // LList1 has items in idx 1*,2,6*
        // LList2 has items in idx 3,4,5*
        // * means it should be moved
        LList1.PopulateObjectList(_gameObjects.ToArray());
        LList2.PopulateObjectList(_gameObjects.ToArray());

        // Now LList1 should be 2, LList2 should be 3,4
        // objs1 should be 1,6
        // objs2 should be 5
        var objs1 = ItemTools.ExtractMovableItems(LList1, settings);
        var objs2 = ItemTools.ExtractMovableItems(LList2, settings);

        // LList1 should have 2,5
        // LList2 should have 3,4,1,6
        LList1.AppendItems(objs2.ToArray()); // Changing to ToArray just to cover the test
        LList2.AppendItems(objs1.ToArray());

        Assert.True(CheckObjectsAtPositions(LList1.ToList(), new List<short>() {2, 5}));
        Assert.True(CheckObjectsAtPositions(LList2.ToList(), new List<short>() {3, 4, 1, 6}));
        Assert.True(LList1.CheckIntegrity());
        Assert.True(LList2.CheckIntegrity());
    }

    [Test]
    public void TestAppendItems_OnEmpty()
    {
        LList1.startingIdx = 1;
        LList2.startingIdx = 0;

        // LList1 has items in idx 1*,2,6*
        // LList2 has no items
        // * means it should be moved
        LList1.PopulateObjectList(_gameObjects.ToArray());
        LList2.PopulateObjectList(_gameObjects.ToArray());

        // Now LList1 should be 2, LList2 should be empty
        // objs1 should be 1,6
        // objs2 should be empty
        var objs1 = ItemTools.ExtractMovableItems(LList1, settings);
        var objs2 = ItemTools.ExtractMovableItems(LList2, settings);

        // LList1 should have 2
        // LList2 should have 1,6
        LList1.AppendItems(objs2);
        LList2.AppendItems(objs1);

        Assert.True(CheckObjectsAtPositions(LList1.ToList(), new List<short>() {2}));
        Assert.True(CheckObjectsAtPositions(LList2.ToList(), new List<short>() {1, 6}));
        Assert.True(LList1.CheckIntegrity());
        Assert.True(LList2.CheckIntegrity());
    }

    [Test]
    public void TestPrependItems()
    {
        LList1.startingIdx = 1;
        LList2.startingIdx = 3;

        // LList1 has items in idx 1*,2,6*
        // LList2 has items in idx 3,4,5*
        // * means it should be moved
        LList1.PopulateObjectList(_gameObjects.ToArray());
        LList2.PopulateObjectList(_gameObjects.ToArray());

        // Now LList1 should be 2, LList2 should be 3,4
        // objs1 should be 1,6
        // objs2 should be 5
        var objs1 = ItemTools.ExtractMovableItems(LList1, settings);
        var objs2 = ItemTools.ExtractMovableItems(LList2, settings);

        // LList1 should have 5,2
        // LList2 should have 1,6,3,4
        LList1.PrependItems(objs2);
        LList2.PrependItems(objs1);

        Assert.True(CheckObjectsAtPositions(LList1.ToList(), new List<short>() {5, 2}));
        Assert.True(CheckObjectsAtPositions(LList2.ToList(), new List<short>() {1, 6, 3, 4}));
        Assert.False(CheckObjectsAtPositions(LList1.ToList(), new List<short>() {2, 5}));
        Assert.False(CheckObjectsAtPositions(LList2.ToList(), new List<short>() {3, 4, 1, 6}));
        Assert.True(LList1.CheckIntegrity());
        Assert.True(LList2.CheckIntegrity());
    }

    [Test]
    public void TestPrependItems_OnEmptyLList()
    {
        LList1.startingIdx = 1;
        LList2.startingIdx = 0;

        // LList1 has items in idx 1*,2,6*
        // LList2 has no items
        // * means it should be moved
        LList1.PopulateObjectList(_gameObjects.ToArray());
        LList2.PopulateObjectList(_gameObjects.ToArray());

        // Now LList1 should be 2, LList2 should be empty
        // objs1 should be 1,6
        // objs2 should be empty
        var objs1 = ItemTools.ExtractMovableItems(LList1, settings);
        var objs2 = ItemTools.ExtractMovableItems(LList2, settings);

        // LList1 should have 2
        // LList2 should have 1,6
        LList1.PrependItems(objs2);
        LList2.PrependItems(objs1);

        Assert.True(CheckObjectsAtPositions(LList1.ToList(), new List<short>() {2}));
        Assert.True(CheckObjectsAtPositions(LList2.ToList(), new List<short>() {1, 6}));
        Assert.True(LList1.CheckIntegrity());
        Assert.True(LList2.CheckIntegrity());
    }
    
}