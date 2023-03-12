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
    private TileInfo DoStuff(ArkLoader arkFile, ItemRandomizationSettings settings, bool firstPass)
    {
        var lvl = arkFile.TileMapObjectsBlocks[0];
        var leftTile = lvl.TileInfos[159]; // X 31 Y 2
        var targetObjHeight = lvl.AllGameObjects[981].Zpos;
        Assert.True(leftTile.FirstObjIdx == 981);
        
        // Stack<GameObject> objectsInLevel = new Stack<GameObject>();
        // int count = 0;
        // foreach (var tile in lvl.TileInfos)
        // {
        //     // TODO: If the same tile is encountered, using this function on it shouldn't affect anything
        //     if (tile.XYPos[0] == 31 & tile.XYPos[1] == 2) continue;
        //     if (count >= 10)
        //     {
        //         break;
        //     }
        //     foreach (var obj in ItemTools.ExtractMovableItems(tile, settings))
        //     {
        //         Console.WriteLine($"Extracted obj {obj.IdxAtObjectArray} from Tile {tile.EntryNum} XY {tile.XYPos[0]}:{tile.XYPos[1]}");
        //         objectsInLevel.Push(obj);
        //         count++;
        //     }
        //
        //     tile.ReconstructBuffer();
        // }
        // while (objectsInLevel.Count > 0)
        // {
        //     leftTile.ObjectChain.Add(objectsInLevel.Pop());
        // }
        
        // Let's get some objects from specific tiles first
        const int indexTile1 = (0xE86 - 542) / 4; // X26, Y12, bowl (142), axe(0), torch(145)
        const int indexTile2 = (0x68E - 542) / 4; // X28, Y4, Jux Rune(241)
        const int indexTile3 = (0x29B6 - 542) / 4; // X38, Y39, Plant(206), Bridge(356) -- shouldn't be moved!

        var Tile1 = lvl.TileInfos[indexTile1];
        var Tile2 = lvl.TileInfos[indexTile2];
        var Tile3 = lvl.TileInfos[indexTile3];

        Assert.True(Tile1.XYPos[0] == 26 && Tile1.XYPos[1] == 12);
        Assert.True(Tile2.XYPos[0] == 28 && Tile2.XYPos[1] == 4);
        Assert.True(Tile3.XYPos[0] == 38 && Tile3.XYPos[1] == 39);
        
        if (firstPass)
            Assert.True(Tile2.ObjectChain[0].ItemID == 241);

        var objs1 = ItemTools.ExtractMovableItems(Tile1, settings); // Len 3
        var objs2 = ItemTools.ExtractMovableItems(Tile2, settings); // Len 1
        var objs3 = ItemTools.ExtractMovableItems(Tile3, settings); // Len 1

        if (firstPass)
        {
            Assert.True(objs1.Count == 3);
            Assert.True(objs2.Count == 1);
            Assert.True(objs3.Count == 1);
        }

        Assert.True(Tile3.ObjectChain.Count == 1); // Bridge
        Assert.True(Tile3.ObjectChain[0].next == 0);
        Assert.True(Tile3.ObjectChain[0].ItemID == 356);

        List<GameObject> objs = new List<GameObject>();
        objs.AddRange(objs1);
        objs.AddRange(objs2);
        objs.AddRange(objs3);

        leftTile.ObjectChain.AppendItems(objs);
        leftTile.MoveObjectsToSameZLevel();
        
        // Needed?
        Tile1.ReconstructBuffer();
        Tile2.ReconstructBuffer();
        Tile3.ReconstructBuffer();
        leftTile.ReconstructBuffer();
        
        return leftTile;
    }
    /// This function is only to test moving files to the start of the game
    [Category("RequiresArk")]
    [Test]
    public void MoveStuffCloseToSpawn()
    {
        var arkFile = new ArkLoader(Paths.UW_ArkOriginalPath);
        var settings = new ItemRandomizationSettings();

        var leftTile = DoStuff(arkFile, settings, true);
        arkFile.ReconstructBuffer();
        var path = UWRandomizerEditor.Utils.StdSaveBuffer(arkFile, Path.GetDirectoryName(arkFile.Path), "mod.ark");
        var newArkFile = new ArkLoader(path);
        
        Assert.True(newArkFile.TileMapObjectsBlocks[0].TileInfos[159].FirstObjIdx == leftTile.FirstObjIdx);
        Assert.True(newArkFile.TileMapObjectsBlocks[0].TileInfos[159].ObjectChain.Count == leftTile.ObjectChain.Count);
        Assert.True(
            (from obj in newArkFile.TileMapObjectsBlocks[0].TileInfos[159].ObjectChain select obj.IdxAtObjectArray)
            .SequenceEqual
            (from obj in arkFile.TileMapObjectsBlocks[0].TileInfos[159].ObjectChain select obj.IdxAtObjectArray)
            );

        var leftTile2 = DoStuff(newArkFile, settings, false);
        
        // TODO: This is failing, and the item ids are repeating. This means the item references aren't being removed from 
        // the correct positions. The Object chain is completely mangled.
        Assert.True(newArkFile.TileMapObjectsBlocks[0].TileInfos[159].FirstObjIdx == leftTile.FirstObjIdx);
        Assert.True(newArkFile.TileMapObjectsBlocks[0].TileInfos[159].ObjectChain.Count == leftTile.ObjectChain.Count);
        Assert.True(
            (from obj in newArkFile.TileMapObjectsBlocks[0].TileInfos[159].ObjectChain select obj.IdxAtObjectArray)
            .SequenceEqual
            (from obj in arkFile.TileMapObjectsBlocks[0].TileInfos[159].ObjectChain select obj.IdxAtObjectArray)
            );
        
    }
    
}