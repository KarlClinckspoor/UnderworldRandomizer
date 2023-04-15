﻿using System;
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
        LList1.StartingIdx = 1;
        LList1.PopulateObjectList(_gameObjects.ToArray());
        Assert.True(CheckObjectsAtPositions(LList1.ToList(), new List<short>() {1, 2, 6}));
        Assert.True(LList1.Count == 3);

        LList1.StartingIdx = 3; // This should clear the list
        Assert.True(LList1.Count == 0);
        LList1.PopulateObjectList(_gameObjects.ToArray());

        Assert.True(CheckObjectsAtPositions(LList1.ToList(), new List<short>() {3, 4, 5}));
        Assert.True(LList1.Count == 3);

        LList1.StartingIdx = 0;
        Assert.True(LList1.Count == 0);
        LList1.PopulateObjectList(_gameObjects.ToArray());
        Assert.True(LList1.Count == 0);

        // Trying to trigger the ContraintException
        _gameObjects[1].next = 2;
        _gameObjects[2].next = 1;
        LList1.Clear();
        LList1.StartingIdx = 1;
        Assert.Throws<ConstraintException>(() => LList1.PopulateObjectList(_gameObjects.ToArray()));
    }

    [Test]
    public void TestPop()
    {
        LList1.StartingIdx = 1;
        LList2.StartingIdx = 0;
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
        LList1.StartingIdx = 1;
        LList1.PopulateObjectList(_gameObjects);
        Assert.True(CheckObjectsAtPositions(LList1.ToList(), new List<short>() {1, 2, 6}));
        Assert.True(LList1.Count == 3);
    }


    [Test]
    public void TestRemoveAt_BackToFront()
    {
        LList1.StartingIdx = 1;
        // LList1 has items in idx 1,2,6
        LList1.PopulateObjectList(_gameObjects.ToArray());

        Assert.Throws<ArgumentOutOfRangeException>(() => LList1.RemoveAt(100));
        Assert.Throws<ArgumentOutOfRangeException>(() => LList1.RemoveAt(-100));

        LList1.RemoveAt(2); // items 1,2
        Assert.True(LList1.Count == 2);
        Assert.True(CheckObjectsAtPositions(LList1.ToList(), new List<short>() {1, 2}));
        Assert.Throws<ArgumentOutOfRangeException>(() => LList1.RemoveAt(2));
        Assert.True(LList1.CheckIntegrity());

        LList1.RemoveAt(1); // items 1
        Assert.True(LList1.Count == 1);
        Assert.True(CheckObjectsAtPositions(LList1.ToList(), new List<short>() {1}));
        Assert.Throws<ArgumentOutOfRangeException>(() => LList1.RemoveAt(1));
        Assert.True(LList1.CheckIntegrity());

        LList1.RemoveAt(0); // no items
        Assert.True(LList1.Count == 0);
        Assert.True(CheckObjectsAtPositions(LList1.ToList(), new List<short>() { }));
        Assert.Throws<ArgumentOutOfRangeException>(() => LList1.RemoveAt(0));
        Assert.True(LList1.CheckIntegrity());
        Assert.True(LList1.StartingIdx == 0);
    }

    [Test]
    public void TestRemoveAt_FrontToBack()
    {
        // LList1 has items in idx 1,2,6
        LList1.StartingIdx = 1;
        LList1.PopulateObjectList(_gameObjects);

        LList1.RemoveAt(0); // items 2,6
        Assert.True(LList1.Count == 2);
        Assert.True(CheckObjectsAtPositions(LList1.ToList(), new List<short>() {2, 6}));
        Assert.Throws<ArgumentOutOfRangeException>(() => LList1.RemoveAt(2));
        Assert.True(LList1.CheckIntegrity());
        Assert.True(LList1.StartingIdx == LList1[0].IdxAtObjectArray);

        LList1.RemoveAt(0); // items 6
        Assert.True(LList1.Count == 1);
        Assert.True(CheckObjectsAtPositions(LList1.ToList(), new List<short>() {6}));
        Assert.Throws<ArgumentOutOfRangeException>(() => LList1.RemoveAt(1));
        Assert.True(LList1.CheckIntegrity());
        Assert.True(LList1.StartingIdx == LList1[0].IdxAtObjectArray);
    }

    [Test]
    public void TestRemoveAt_Middle()
    {
        // LList1 has items in idx 1,2,6
        LList1.StartingIdx = 1;
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
        LList1.StartingIdx = 1;
        _gameObjects[1].next = 0;
        LList1.PopulateObjectList(_gameObjects);
        LList1.Remove(_gameObjects[1]);
        Assert.True(LList1.Count == 0);
        Assert.True(LList1.CheckIntegrity());
    }

    [Test]
    public void TestRemoveFromMiddle()
    {
        LList1.StartingIdx = 1;
        LList1.PopulateObjectList(_gameObjects);
        LList1.Remove(_gameObjects[2]);
        Assert.True(LList1.Count == 2);
        Assert.True(LList1.CheckIntegrity());
    }

    [Test]
    public void TestRemove_NotFound()
    {
        LList1.StartingIdx = 1;
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
        LList1.StartingIdx = 1;
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

        Assert.Throws<ArgumentOutOfRangeException>(() => LList1.Insert(-1, _gameObjects[1]));
        Assert.Throws<ArgumentOutOfRangeException>(() => LList1.Insert(1000, _gameObjects[1]));
    }

    [Test]
    public void TestGetEnumerator()
    {
        LList1.StartingIdx = 1;
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
        LList1.StartingIdx = 1;
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
        LList1.StartingIdx = 1;
        Assert.False(LList1.CheckIntegrity());
    }

    [Test]
    public void TestContains()
    {
        LList1.StartingIdx = 1;
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
        LList1.StartingIdx = 1;
        LList1.PopulateObjectList(_gameObjects);
        Assert.True(LList1.IndexOf(_gameObjects[1]) == 0);
        Assert.True(LList1.IndexOf(_gameObjects[2]) == 1);
        Assert.True(LList1.IndexOf(_gameObjects[6]) == 2);
        Assert.True(LList1.IndexOf(_gameObjects[3]) == -1);
        Assert.True(LList1.IndexOf(_gameObjects[4]) == -1);
        Assert.True(LList1.IndexOf(_gameObjects[5]) == -1);
    }


    [Test]
    public void TestSet()
    {
        LList1.StartingIdx = 1;
        LList1.PopulateObjectList(_gameObjects);

        LList1[0] = _gameObjects[3];
        Assert.True(LList1.CheckIntegrity());
        LList1[2] = _gameObjects[4];
        Assert.True(LList1.CheckIntegrity());
        LList1[1] = _gameObjects[5];
        Assert.True(LList1.CheckIntegrity());

        Assert.Throws<ArgumentOutOfRangeException>(() => LList1[-1] = _gameObjects[2]);
        Assert.Throws<ArgumentOutOfRangeException>(() => LList1[100] = _gameObjects[2]);

        LList1.Clear();
        LList1.Add(_gameObjects[1]);
        LList1[0] = _gameObjects[2];
        Assert.True(LList1.CheckIntegrity());
    }

    [Test]
    public void TestGet()
    {
        LList1.StartingIdx = 1;
        LList1.PopulateObjectList(_gameObjects);

        Assert.True(LList1[0].Equals(_gameObjects[1]));
        Assert.True(LList1[1].Equals(_gameObjects[2]));
        Assert.True(LList1[2].Equals(_gameObjects[6]));

        Assert.Throws<ArgumentOutOfRangeException>(() => LList1[-1].ToString());
        Assert.Throws<ArgumentOutOfRangeException>(() => LList1[100].ToString());
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

    [Category("RequiresSettings")]
    [Test]
    public void TestReferenceCounts()
    {
        var AL = new ArkLoader(Paths.UW_ArkOriginalPath);
        foreach (var block in AL.TileMapObjectsBlocks)
        {
            var counter = 0;
            var lowestObjectWithReference = 10000;
            
            foreach (var mobileObject in block.MobileObjects)
            {
                Assert.True(mobileObject.ReferenceCount <= 1);
                if (mobileObject.ReferenceCount == 1)
                {
                    counter += 1;
                }

                if ((mobileObject.IdxAtObjectArray < lowestObjectWithReference)&(mobileObject.ReferenceCount>=1))
                {
                    lowestObjectWithReference = mobileObject.IdxAtObjectArray;
                }
            }
            Console.WriteLine($"lvl: {block.LevelNumber} references {counter} MobileObjects with the lowest one being " +
                              $"{lowestObjectWithReference}. First Obj idx is {block.FirstFreeMobileObjectIdx}; " +
                              $"slot is {block.FirstFreeSlotInMobileList}");
            
            counter = 0;
            lowestObjectWithReference = 10000;
            
            foreach (var staticObject in block.StaticObjects)
            {
                Assert.True(staticObject.ReferenceCount <= 1);
                if (staticObject.ReferenceCount == 1)
                {
                    counter += 1;
                }

                if ((staticObject.IdxAtObjectArray < lowestObjectWithReference) & (staticObject.ReferenceCount>=1))
                {
                    lowestObjectWithReference = staticObject.IdxAtObjectArray;
                }
            }
            Console.WriteLine($"lvl: {block.LevelNumber} references {counter} StaticObjects with the lowest one being " +
                              $"{lowestObjectWithReference}. First Obj idx is {block.FirstFreeStaticObjectIdx}; " +
                              $"slot is {block.FirstFreeSlotInStaticList+254}");
            
            var objectsThatAreInvalidAndAreUsed = block.AllGameObjects
                .Select(x => x)
                .Where(x => (x.ReferenceCount == 1) & (x.Invalid)).ToList();
            
            if (objectsThatAreInvalidAndAreUsed.Count > 0)
                Console.WriteLine($"\tlvl: {block.LevelNumber} - The following objects are considered 'invalid' but are referenced by tiles or containers in the level");
            else
                Console.WriteLine($"\tlvl: {block.LevelNumber} - Has no objects that are invalid and referenced");
            
            foreach (var obj in objectsThatAreInvalidAndAreUsed)
            {
                Console.WriteLine($"\t{obj}, index {obj.IdxAtObjectArray}, in container? {obj.InContainer}");
            }
            
        }
    }

    [Category("RequiresSettings")]
    [Test]
    public void TestItemsBeforeFirstFreeSlot()
    {
        var AL = new ArkLoader(Paths.UW_ArkOriginalPath);
        foreach (var block in AL.TileMapObjectsBlocks)
        {
            var originalValidity = new bool[block.AllGameObjects.Length];
            foreach (var obj in block.AllGameObjects)
            {
                originalValidity[obj.IdxAtObjectArray] = obj.Invalid;
                obj.Invalid = false;
            }

            block.AllGameObjects[0].Invalid = true;
            block.AllGameObjects[1].Invalid = true;

            for (int i = 2; i <= block.FirstFreeSlotInMobileList; i++)
            {
                var idx = block.FreeListMobileObjects[i].IdxAtArray;
                block.MobileObjects[idx].Invalid = true;
            }

            for (int i = 0; i <= block.FirstFreeSlotInStaticList; i++)
            {
                var idx = block.FreeListStaticObjects[i].IdxAtArray;
                block.AllGameObjects[idx].Invalid = true;
            }

            var objsThatAreBeforeTheFirstFreeSlot = block.AllGameObjects
                .Select(x => x)
                .Where(x => x.Invalid).ToList();

            var idxsToExplore = new List<int>();
            Console.WriteLine($"Block {block.LevelNumber}");
            for (int i = 0; i < block.AllGameObjects.Length; i++)
            {
                if (block.AllGameObjects[i].Invalid != originalValidity[i])
                {
                    Console.WriteLine($"\t{i}: {block.AllGameObjects[i]} has different validity. Before: valid? {!originalValidity[i]} after: valid?{!block.AllGameObjects[i].Invalid}");
                    idxsToExplore.Add(i);
                }
            }
            Console.WriteLine("\t Explore idxs: " + string.Join(",", idxsToExplore));
            
            Func<TileInfo, List<ushort>> getIdxs = tileInfo => tileInfo.ObjectChain.Select(gameObject => gameObject.IdxAtObjectArray).ToList();
            
            var TilesToConsider = block.TileInfos
                .Where(tileInfo => getIdxs(tileInfo).Any(idx => idxsToExplore.Contains(idx)))
                .Select(x => $"({x.XPos},{x.YPos})");
            Console.WriteLine("\tCheck tiles " + String.Join(" ", TilesToConsider));
        }
    }
}

