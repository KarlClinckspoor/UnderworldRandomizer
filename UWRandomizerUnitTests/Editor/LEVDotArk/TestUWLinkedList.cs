using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NUnit.Framework;
using UWRandomizerEditor.LEVdotARK;
using UWRandomizerEditor.LEVdotARK.GameObjects;
using UWRandomizerEditor.LEVdotARK.GameObjects.Specifics;

namespace RandomizerUnitTests.Editor.LEVDotArk;

[TestFixture]
public class TestUWLinkedList
{
#pragma warning disable CS8618
    private List<GameObject> _gameObjects;
    private UWLinkedList LList1;
    private UWLinkedList LList2;
#pragma warning restore CS8618

    [SetUp]
    public void Setup()
    {
        var fillerBytes = new byte[8];
        _gameObjects = new List<GameObject>()
        {
            new StaticObject(fillerBytes, 0) {next = 0}, // end TODO: What is the buffer of object 0?
            new QuantityGameObject(fillerBytes, 1) {next = 2}, // 1.1   movable
            new Door(fillerBytes, 2) {next = 6}, // 1.2         immovable
            new Trap(fillerBytes, 3) {next = 4}, // 2.1         immovable
            new TexturedGameObject(fillerBytes, 4) {next = 5}, // 2.2   immovable
            new EnchantedArmor(fillerBytes, 5) {next = 0}, // 2.3 (last) movable
            new EnchantedWeapon(fillerBytes, 6) {next = 0} // 1.3 (last) movable
        };

        LList1 = new UWLinkedList();
        LList2 = new UWLinkedList();
    }

    [Test]
    public void TestPopulateObjectList()
    {
        LList1.startingIdx = 1;
        LList1.PopulateObjectList(_gameObjects.ToArray());
        Assert.True(CheckObjectsAtPositions(LList1.ToList(), new List<short>() {1, 2, 6}));
        Assert.True(LList1.Count == 3);

        LList1.startingIdx = 3; // This should clear the list
        Assert.True(LList1.Count == 0);
        LList1.PopulateObjectList(_gameObjects.ToArray());

        Assert.True(CheckObjectsAtPositions(LList1.ToList(), new List<short>() {3, 4, 5}));
        Assert.True(LList1.Count == 3);

        LList1.startingIdx = 0;
        Assert.True(LList1.Count == 0);
        LList1.PopulateObjectList(_gameObjects.ToArray());
        Assert.True(LList1.Count == 0);

        // Trying to trigger the ContraintException
        _gameObjects[1].next = 2;
        _gameObjects[2].next = 1;
        LList1.Clear();
        LList1.startingIdx = 1;
        Assert.Throws<ConstraintException>(() => LList1.PopulateObjectList(_gameObjects.ToArray()));
    }

    [Test]
    public void TestPop()
    {
        LList1.startingIdx = 1;
        LList2.startingIdx = 0;
        LList1.PopulateObjectList(_gameObjects);
        LList2.PopulateObjectList(_gameObjects);

        // Assert.Throws<InvalidOperationException>(() => throw new InvalidOperationException());
        Assert.True(LList1.Pop().Equals(_gameObjects[6]));
        Assert.True(LList1.Pop().Equals(_gameObjects[2]));
        Assert.True(LList1.Pop().Equals(_gameObjects[1]));
        Assert.Throws<InvalidOperationException>(() => LList1.Pop());
        Assert.Throws<InvalidOperationException>(() => LList2.Pop());
    }

    [Test]
    public void TestPopulateObjectList_withList()
    {
        LList1.startingIdx = 1;
        LList1.PopulateObjectList(_gameObjects);
        Assert.True(CheckObjectsAtPositions(LList1.ToList(), new List<short>() {1, 2, 6}));
        Assert.True(LList1.Count == 3);
    }


    [Test]
    public void TestRemoveAt_BackToFront()
    {
        LList1.startingIdx = 1;
        // LList1 has items in idx 1,2,6
        LList1.PopulateObjectList(_gameObjects.ToArray());

        Assert.Throws<IndexOutOfRangeException>(() => LList1.RemoveAt(100));
        Assert.Throws<IndexOutOfRangeException>(() => LList1.RemoveAt(-100));

        LList1.RemoveAt(2); // items 1,2
        Assert.True(LList1.Count == 2);
        Assert.True(CheckObjectsAtPositions(LList1.ToList(), new List<short>() {1, 2}));
        Assert.Throws<IndexOutOfRangeException>(() => LList1.RemoveAt(2));
        Assert.True(LList1.CheckIntegrity());

        LList1.RemoveAt(1); // items 1
        Assert.True(LList1.Count == 1);
        Assert.True(CheckObjectsAtPositions(LList1.ToList(), new List<short>() {1}));
        Assert.Throws<IndexOutOfRangeException>(() => LList1.RemoveAt(1));
        Assert.True(LList1.CheckIntegrity());

        LList1.RemoveAt(0); // no items
        Assert.True(LList1.Count == 0);
        Assert.True(CheckObjectsAtPositions(LList1.ToList(), new List<short>() { }));
        Assert.Throws<IndexOutOfRangeException>(() => LList1.RemoveAt(0));
        Assert.True(LList1.startingIdx == 0);
        Assert.True(LList1.CheckIntegrity());
    }

    [Test]
    public void TestRemoveAt_FrontToBack()
    {
        // LList1 has items in idx 1,2,6
        LList1.startingIdx = 1;
        LList1.PopulateObjectList(_gameObjects);

        LList1.RemoveAt(0); // items 2,6
        Assert.True(LList1.Count == 2);
        Assert.True(CheckObjectsAtPositions(LList1.ToList(), new List<short>() {2, 6}));
        Assert.Throws<IndexOutOfRangeException>(() => LList1.RemoveAt(2));
        Assert.True(LList1.startingIdx == LList1[0].IdxAtObjectArray);
        Assert.True(LList1.CheckIntegrity());

        LList1.RemoveAt(0); // items 6
        Assert.True(LList1.Count == 1);
        Assert.True(CheckObjectsAtPositions(LList1.ToList(), new List<short>() {6}));
        Assert.Throws<IndexOutOfRangeException>(() => LList1.RemoveAt(1));
        Assert.True(LList1.startingIdx == LList1[0].IdxAtObjectArray);
        Assert.True(LList1.CheckIntegrity());
    }

    [Test]
    public void TestRemoveAt_Middle()
    {
        // LList1 has items in idx 1,2,6
        LList1.startingIdx = 1;
        LList1.PopulateObjectList(_gameObjects);

        LList1.RemoveAt(1); // items 1,6
        Assert.True(LList1.Count == 2);
        Assert.True(CheckObjectsAtPositions(LList1.ToList(), new List<short>() {1, 6}));
        Assert.True(LList1.CheckIntegrity());

        LList1.RemoveAt(1); // items 1
        Assert.True(LList1.Count == 1);
        Assert.True(CheckObjectsAtPositions(LList1.ToList(), new List<short>() {1}));
        Assert.True(LList1.CheckIntegrity());
    }

    [Test]
    public void TestRemove_1Element()
    {
        LList1.startingIdx = 1;
        _gameObjects[1].next = 0;
        LList1.PopulateObjectList(_gameObjects);
        LList1.Remove(_gameObjects[1]);
        Assert.True(LList1.Count == 0);
        Assert.True(LList1.CheckIntegrity());
    }

    [Test]
    public void TestRemoveFromMiddle()
    {
        LList1.startingIdx = 1;
        LList1.PopulateObjectList(_gameObjects);
        LList1.Remove(_gameObjects[2]);
        Assert.True(LList1.Count == 2);
        Assert.True(LList1.CheckIntegrity());
    }

    [Test]
    public void TestRemove_NotFound()
    {
        LList1.startingIdx = 1;
        LList1.PopulateObjectList(_gameObjects);
        Assert.False(LList1.Remove(_gameObjects[^2]));
    }

    [Test]
    public void TestIsReadOnly()
    {
        Assert.False(LList1.IsReadOnly);
    }

    [Test]
    public void TestInsert()
    {
        LList1.startingIdx = 1;
        LList1.PopulateObjectList(_gameObjects);

        LList1.Insert(0, _gameObjects[3]);
        Assert.True(LList1.Count == 4);
        Assert.True(LList1.CheckIntegrity());
        LList1.RemoveAt(0);
        Assert.True(LList1.CheckIntegrity());

        LList1.Insert(3, _gameObjects[3]);
        Assert.True(LList1.Count == 4);
        Assert.True(LList1.CheckIntegrity());
        LList1.RemoveAt(3);
        Assert.True(LList1.CheckIntegrity());

        LList1.Insert(2, _gameObjects[3]);
        Assert.True(LList1.Count == 4);
        Assert.True(LList1.CheckIntegrity());

        Assert.Throws<IndexOutOfRangeException>(() => LList1.Insert(-1, _gameObjects[1]));
        Assert.Throws<IndexOutOfRangeException>(() => LList1.Insert(1000, _gameObjects[1]));
    }

    [Test]
    public void TestGetEnumerator()
    {
        LList1.startingIdx = 1;
        LList1.PopulateObjectList(_gameObjects);
        foreach (var foo in LList1) {}
        // todo: How do I get to IEnumerator IEnumerable.GetEnumerator?
    }


    private bool CheckObjectsAtPositions(List<GameObject> list, List<short> correctIdxs)
    {
        if (list.Count != correctIdxs.Count)
        {
            throw new InvalidOperationException("Can't compare two lists of unequal counts!");
        }

        for (int i = 0; i < list.Count; i++)
        {
            if (!_gameObjects[correctIdxs[i]].Equals(list[i]))
            {
                return false;
            }
        }

        return true;
    }

    [Test]
    public void TestListCheckIntegrity()
    {
        // This is what should be run normally 
        LList1.startingIdx = 1;
        LList1.PopulateObjectList(_gameObjects);
        Assert.True(LList1.CheckIntegrity());

        // Changing the index of the first object should invalidate the list because it wouldn't be pointing correctly
        LList1[0].IdxAtObjectArray = 100; // Should be 1
        Assert.False(LList1.CheckIntegrity());
        LList1[0].IdxAtObjectArray = 1; // Returning to normal
        Assert.True(LList1.CheckIntegrity());

        // Changing the index of an object in the middle, or its next attribute, should invalidate the LList
        LList1[1].IdxAtObjectArray = 100; // Should be 2
        Assert.False(LList1.CheckIntegrity());
        LList1[1].IdxAtObjectArray = 2; // Returning
        Assert.True(LList1.CheckIntegrity());

        LList1[1].next = 100; // Should be 6
        Assert.False(LList1.CheckIntegrity());
        LList1[1].next = 6; // Returning
        Assert.True(LList1.CheckIntegrity());

        // 
        _gameObjects[^1].next = 100;
        Assert.False(LList1.CheckIntegrity());

        // Having a starting index but no objects is invalid
        LList1.Clear();
        LList1.startingIdx = 1;
        Assert.False(LList1.CheckIntegrity());
    }

    [Test]
    public void TestContains()
    {
        LList1.startingIdx = 1;
        LList1.PopulateObjectList(_gameObjects);
        Assert.True(LList1.Contains(_gameObjects[1]));
        Assert.True(LList1.Contains(_gameObjects[2]));
        Assert.True(LList1.Contains(_gameObjects[6]));
        Assert.False(LList1.Contains(_gameObjects[3]));
        Assert.False(LList1.Contains(_gameObjects[4]));
        Assert.False(LList1.Contains(_gameObjects[5]));
    }

    [Test]
    public void TestIndexOf()
    {
        LList1.startingIdx = 1;
        LList1.PopulateObjectList(_gameObjects);
        Assert.True(LList1.IndexOf(_gameObjects[1]) == 0);
        Assert.True(LList1.IndexOf(_gameObjects[2]) == 1);
        Assert.True(LList1.IndexOf(_gameObjects[6]) == 2);
        Assert.True(LList1.IndexOf(_gameObjects[3]) == -1);
        Assert.True(LList1.IndexOf(_gameObjects[4]) == -1);
        Assert.True(LList1.IndexOf(_gameObjects[5]) == -1);
    }

    [Test]
    public void TestListCtor()
    {
        LList1.startingIdx = 1;
        LList1.PopulateObjectList(_gameObjects);

        var LList3 = new UWLinkedList(LList1.ToList(), (ushort) LList1.startingIdx);
        var LList4 = new UWLinkedList(LList1.ToArray(), (ushort) LList1.startingIdx);
        Assert.True(LList3.CheckIntegrity());
        Assert.True(LList4.CheckIntegrity());
    }

    [Test]
    public void TestSet()
    {
        LList1.startingIdx = 1;
        LList1.PopulateObjectList(_gameObjects);

        LList1[0] = _gameObjects[3];
        Assert.True(LList1.CheckIntegrity());
        LList1[2] = _gameObjects[4];
        Assert.True(LList1.CheckIntegrity());
        LList1[1] = _gameObjects[5];
        Assert.True(LList1.CheckIntegrity());

        Assert.Throws<IndexOutOfRangeException>(() => LList1[-1] = _gameObjects[2]);
        Assert.Throws<IndexOutOfRangeException>(() => LList1[100] = _gameObjects[2]);

        LList1.Clear();
        LList1.Add(_gameObjects[1]);
        LList1[0] = _gameObjects[2];
        Assert.True(LList1.CheckIntegrity());
    }

    [Test]
    public void TestGet()
    {
        LList1.startingIdx = 1;
        LList1.PopulateObjectList(_gameObjects);

        Assert.True(LList1[0].Equals(_gameObjects[1]));
        Assert.True(LList1[1].Equals(_gameObjects[2]));
        Assert.True(LList1[2].Equals(_gameObjects[6]));

        Assert.Throws<IndexOutOfRangeException>(() => LList1[-1].ToString());
        Assert.Throws<IndexOutOfRangeException>(() => LList1[100].ToString());
    }

    [Test]
    public void TestAddingTwoOfTheSameItem()
    {
        LList1.Add(_gameObjects[1]);
        LList1.Add(_gameObjects[1]);
        Assert.False(LList1.CheckIntegrity());
    }

    [Test]
    public void TestHavingItemOfIdx0()
    {
        LList1.Add(_gameObjects[0]);
        Assert.False(LList1.CheckIntegrity());
    }
    
    
}