﻿using System;
using System.Configuration;
using System.IO;
using NUnit.Framework;
using UWRandomizerEditor.LEV;
using UWRandomizerEditor.LEV.Blocks;
using UWRandomizerEditor.LEV.GameObjects;
using UWRandomizerEditor.LEV.GameObjects.Specifics;

namespace RandomizerUnitTests.Editor.LEV;

[TestFixture]
[Category("RequiresSettings")]

class ArkObjectAdditionRemoval
{
    [Test]
    public void TestAddingStaticObjectNearSpawn()
    {
        var AL = new LevLoader(Paths.UW1_ArkCleanedPath);
        var block = AL.MapObjBlocks[0];
        // ushort p = 223;
        Point p = new Point(223);
        
        StaticObject obj = (StaticObject) block.AllGameObjects[620]; 
        var newObj = obj.Clone(); // Meat 176
        Assert.True(newObj.ItemID == 176);
        newObj.Quality = 40;
        newObj.Xpos = 3;
        newObj.Ypos = 3;
        newObj.Zpos = 88;
        
        var originalIdx = block.IdxOfFreeStaticObject;
        Assert.True(originalIdx == 532);

        var tile = block.Tiles2D[p.Row, p.Column];
        Assert.True(tile.FirstObjIdx == 0);
        
        block.AddNewGameObjectToTile(p, newObj);
        Assert.False(obj.Equals(newObj));
        Assert.True(tile.FirstObjIdx == newObj.IdxAtObjectArray);
        
        string savedpath = UWRandomizerEditor.Utils.SaveBuffer(AL, Paths.TestDataOutput, "withMeatCloseToSpawn.bin");
        var AL2 = new LevLoader(savedpath);
        (var didTheFileSaveAndLoadCorrectly, var differencePositions) = Utils.CompareTwoBuffers(AL.Buffer, AL2.Buffer);

        (var didTheChangesOccurInTheCorrectPlaces_Manual, var differencePositions2) = Utils.CompareTwoBuffers(
            File.ReadAllBytes(Paths.UW1_ArkCleanedPath), File.ReadAllBytes(savedpath)
        );
        var AL3 = new LevLoader(Paths.UW1_ArkCleanedPath);
        (var didTheChangesOccurInTheCorrectPlaces_Lev, var differencePositions3) = Utils.CompareTwoBuffers(AL3.Buffer, AL2.Buffer);
        
        Assert.True(AL2.MapObjBlocks[0].Tiles2D[p.Row, p.Column].FirstObjIdx == newObj.IdxAtObjectArray);
        Assert.True(didTheFileSaveAndLoadCorrectly == 0);
        Assert.True(didTheChangesOccurInTheCorrectPlaces_Manual == 8);
        Assert.True(didTheChangesOccurInTheCorrectPlaces_Lev == 8);
    }

    [Test]
    public void TestAddingStaticObjectNearSpawn2()
    {
        var AL = new LevLoader(Paths.UW1_ArkOriginalPath);
        var block = AL.MapObjBlocks[0];
        Point p = new Point(223);
        
        StaticObject obj = (StaticObject) block.AllGameObjects[620]; 
        var newObj = obj.Clone(); // Meat 176
        Assert.True(newObj.ItemID == 176);
        newObj.Quality = 40;
        newObj.Xpos = 3;
        newObj.Ypos = 3;
        newObj.Zpos = 88;
        
        var originalIdx = block.IdxOfFreeStaticObject;
        Assert.True(originalIdx == 532);

        var tile = block.Tiles2D[p.Row, p.Column];
        Assert.True(tile.FirstObjIdx == 0);
        
        block.AddNewGameObjectToTile(p, newObj);
        Assert.False(obj.Equals(newObj));
        Assert.True(tile.FirstObjIdx == newObj.IdxAtObjectArray);
        AL.ReconstructBuffer();
        string savedpath = UWRandomizerEditor.Utils.SaveBuffer(AL, Paths.TestDataOutput, "withMeatCloseToSpawn_Original.bin");
        var AL2 = new LevLoader(savedpath);
        (var didTheFileSaveAndLoadCorrectly, var differencePositions) = Utils.CompareTwoBuffers(AL.Buffer, AL2.Buffer);

        (var didTheChangesOccurWhereExpected_Manual, var differencePositions2) = Utils.CompareTwoBuffers(
            File.ReadAllBytes(Paths.UW1_ArkOriginalPath), File.ReadAllBytes(savedpath)
        );
        var AL3 = new LevLoader(Paths.UW1_ArkOriginalPath);
        (var didTheChangesOccurWhereExpected_Ark, var differencePositions3) = Utils.CompareTwoBuffers(AL3.Buffer, AL2.Buffer);
        
        Assert.True(AL2.MapObjBlocks[0].Tiles2D[p.Row, p.Column].FirstObjIdx == newObj.IdxAtObjectArray);
        Assert.True(didTheFileSaveAndLoadCorrectly == 0);
        Assert.True(didTheChangesOccurWhereExpected_Manual == 4); // Values gotten from manual inspection
        Assert.True(didTheChangesOccurWhereExpected_Ark == 4);
        // TODO: Why 4 here, and not 8, like in the previous test?
    }

    [Test]
    public void TestAddingMobileObjectNearSpawn()
    {
        var AL = new LevLoader(Paths.UW1_ArkCleanedPath);
        var block = AL.MapObjBlocks[0];
        MobileObject obj = block.MobileObjects[219]; // Green goblin
        var newObj = obj.Clone();
        
        var originalIdx = block.IdxOfFreeMobileObject;
        
        block.AddNewGameObjectToTile(new Point(223), newObj);
        
        string savedpath = UWRandomizerEditor.Utils.SaveBuffer(AL, Paths.TestDataOutput, "withGoblinCloseToSpawn_Cleaned.bin");
        var AL2 = new LevLoader(savedpath);
        (var didItSaveCorrectly, var differencePositions) = Utils.CompareTwoBuffers(AL.Buffer, AL2.Buffer);

        (var differenceCount2, var differencePositions2) = Utils.CompareTwoBuffers(
            File.ReadAllBytes(Paths.UW1_ArkOriginalPath), File.ReadAllBytes(savedpath)
        );
        var AL3 = new LevLoader(Paths.UW1_ArkOriginalPath);
        (var differenceCount3, var differencePositions3) = Utils.CompareTwoBuffers(AL3.Buffer, AL2.Buffer);
        
        Assert.True(AL2.MapObjBlocks[0].Tiles[223].FirstObjIdx == newObj.IdxAtObjectArray);
        Assert.True(didItSaveCorrectly == 0);
    }
}