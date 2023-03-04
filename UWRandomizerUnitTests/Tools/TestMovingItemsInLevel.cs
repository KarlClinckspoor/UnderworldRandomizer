using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UWRandomizerEditor.LEVdotARK;
using UWRandomizerEditor.LEVdotARK.GameObjects;
using UWRandomizerTools;

namespace RandomizerUnitTests;

[TestFixture]
public class TestMovingItemsInLevel
{
    private TileInfo DoStuff(ArkLoader arkFile, ItemRandomizationSettings settings)
    {
        var lvl = arkFile.TileMapObjectsBlocks[0];
        var leftTile = lvl.TileInfos[159]; // X 31 Y 2
        var targetObjHeight = lvl.AllGameObjects[981].Zpos;
        Assert.True(leftTile.FirstObjIdx == 981);
        
        Stack<GameObject> objectsInLevel = new Stack<GameObject>();
        int count = 0;
        foreach (var tile in lvl.TileInfos)
        {
            // TODO: If the same tile is encountered, using this function on it shouldn't affect anything
            if (tile.XYPos[0] == 31 & tile.XYPos[1] == 2) continue;
            if (count >= 10)
            {
                break;
            }
            foreach (var obj in ItemTools.ExtractMovableItems(tile, settings))
            {
                Console.WriteLine($"Extracted obj {obj.IdxAtObjectArray} from Tile {tile.EntryNum} XY {tile.XYPos[0]}:{tile.XYPos[1]}");
                objectsInLevel.Push(obj);
                count++;
            }
        }
        while (objectsInLevel.Count > 0)
        {
            leftTile.ObjectChain.Add(objectsInLevel.Pop());
        }

        leftTile.MoveObjectsToSameZLevel();
        return leftTile;
    }
    /// This function is only to test moving files to the start of the game
    [Test]
    public void MoveStuffCloseToSpawn()
    {
        var arkFile = new ArkLoader(Paths.UW_ArkOriginalPath);
        var settings = new ItemRandomizationSettings();

        var leftTile = DoStuff(arkFile, settings);
        arkFile.ReconstructBuffer();
        var path = UWRandomizerEditor.Utils.StdSaveBuffer(arkFile, Path.GetDirectoryName(arkFile.Path), "mod.ark");
        var newArkFile = new ArkLoader(path);
        
        Assert.True(newArkFile.TileMapObjectsBlocks[0].TileInfos[159].FirstObjIdx == leftTile.FirstObjIdx);
        Assert.True(newArkFile.TileMapObjectsBlocks[0].TileInfos[159].ObjectChain.Count == leftTile.ObjectChain.Count);
        Assert.True((from obj in newArkFile.TileMapObjectsBlocks[0].TileInfos[159].ObjectChain select obj.IdxAtObjectArray).SequenceEqual(
            from obj in arkFile.TileMapObjectsBlocks[0].TileInfos[159].ObjectChain select obj.IdxAtObjectArray));

        var leftTile2 = DoStuff(newArkFile, settings);
        
        // TODO: This is failing, and the item ids are repeating. This means the item references aren't being removed from 
        // the correct positions. The Object chain is completely mangled.
        Assert.True(newArkFile.TileMapObjectsBlocks[0].TileInfos[159].FirstObjIdx == leftTile.FirstObjIdx);
        Assert.True(newArkFile.TileMapObjectsBlocks[0].TileInfos[159].ObjectChain.Count == leftTile.ObjectChain.Count);
        Assert.True((from obj in newArkFile.TileMapObjectsBlocks[0].TileInfos[159].ObjectChain select obj.IdxAtObjectArray).SequenceEqual(
            from obj in arkFile.TileMapObjectsBlocks[0].TileInfos[159].ObjectChain select obj.IdxAtObjectArray));
        
    }
    
}