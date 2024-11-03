using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UWRandomizerEditor.LEV;
using UWRandomizerEditor.LEV.GameObjects;
using UWRandomizerTools;

namespace RandomizerUnitTests.Tools;

[TestFixture]
public class TestMovingItemsInLevel
{

    private Tile Move10ItemsNearSpawn(LevLoader arkFile, ItemRandomizationSettings settings)
    {
        var lvl = arkFile.TileMapObjectsBlocks[0];
        var leftTile = lvl.Tiles[159]; // X 31 Y 2
        // Assert.True(leftTile.FirstObjIdx == 981);
        
        Stack<GameObject> objectsInLevel = new Stack<GameObject>();
        int count = 0;
        foreach (var tile in lvl.Tiles)
        {
            // if (tile.XYPos.x == 31 & tile.XYPos.y == 2) continue;
            if (count >= 10)
            {
                break;
            }
            foreach (var obj in ItemTools.ExtractMovableItems(tile, settings))
            {
                Console.WriteLine($"Extracted obj {obj.IdxAtObjectArray} from Tile num {tile.EntryNum} XY {tile.XYPos}");
                objectsInLevel.Push(obj);
                count++;
            }
        
            tile.ReconstructBuffer();
        }
        while (objectsInLevel.Count > 0)
        {
            leftTile.ObjectChain.Add(objectsInLevel.Pop());
        }
        
        leftTile.MoveObjectsToSameZLevel();
        
        leftTile.ReconstructBuffer();
        return leftTile;
    }
    private Tile MoveSpecificItemsNearSpawn(LevLoader arkFile, ItemRandomizationSettings settings, bool firstPass)
    {
        var lvl = arkFile.TileMapObjectsBlocks[0];
        var leftTile = lvl.Tiles[159]; // X 31 Y 2
        Assert.True(leftTile.FirstObjIdx == 981);
        
        // Let's get some objects from specific tiles first
        const int indexTile1 = (0xE86 - 542) / 4; // X26, Y12, bowl (142), axe(0), torch(145)
        const int indexTile2 = (0x68E - 542) / 4; // X28, Y4, Jux Rune(241)
        const int indexTile3 = (0x29B6 - 542) / 4; // X38, Y39, Plant(206), Bridge(356) -- shouldn't be moved!

        var Tile1 = lvl.Tiles[indexTile1];
        var Tile2 = lvl.Tiles[indexTile2];
        var Tile3 = lvl.Tiles[indexTile3];

        Assert.True(Tile1.XYPos.Row == 26 && Tile1.XYPos.Column == 12);
        Assert.True(Tile2.XYPos.Row == 28 && Tile2.XYPos.Column == 4);
        Assert.True(Tile3.XYPos.Row == 38 && Tile3.XYPos.Column == 39);
        
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
    
    [Category("RequiresSettings")]
    [Test]
    public void TestMovingItemsNearSpawn()
    {
        var arkFile = Utils.LoadAndAssertOriginalLevArk();
        var settings = new ItemRandomizationSettings();

        var leftTile = MoveSpecificItemsNearSpawn(arkFile, settings, true);
        arkFile.ReconstructBuffer();
        var path = UWRandomizerEditor.Utils.SaveBuffer(arkFile, Path.GetDirectoryName(arkFile.Path), "mod.ark");
        var newArkFile = new LevLoader(path);
        
        Assert.True(newArkFile.TileMapObjectsBlocks[0].Tiles[159].FirstObjIdx == leftTile.FirstObjIdx);
        Assert.True(newArkFile.TileMapObjectsBlocks[0].Tiles[159].ObjectChain.Count == leftTile.ObjectChain.Count);
        Assert.True(
            (from obj in newArkFile.TileMapObjectsBlocks[0].Tiles[159].ObjectChain select obj.IdxAtObjectArray)
            .SequenceEqual
            (from obj in arkFile.TileMapObjectsBlocks[0].Tiles[159].ObjectChain select obj.IdxAtObjectArray)
            );

        var leftTile2 = MoveSpecificItemsNearSpawn(newArkFile, settings, false);
        
        Assert.True(newArkFile.TileMapObjectsBlocks[0].Tiles[159].FirstObjIdx == leftTile.FirstObjIdx);
        Assert.True(newArkFile.TileMapObjectsBlocks[0].Tiles[159].ObjectChain.Count == leftTile.ObjectChain.Count);
        Assert.True(
            (from obj in newArkFile.TileMapObjectsBlocks[0].Tiles[159].ObjectChain select obj.IdxAtObjectArray)
            .SequenceEqual
            (from obj in arkFile.TileMapObjectsBlocks[0].Tiles[159].ObjectChain select obj.IdxAtObjectArray)
            );
        
    }

    [Category("RequiresSettings")]
    [Test]
    public void TestMoving10ItemsNearSpawn()
    {
        var arkFile = Utils.LoadAndAssertOriginalLevArk();
        var settings = new ItemRandomizationSettings();

        var leftTile = Move10ItemsNearSpawn(arkFile, settings);
        arkFile.ReconstructBuffer();
        var path = UWRandomizerEditor.Utils.SaveBuffer(arkFile, Path.GetDirectoryName(arkFile.Path), "mod.ark");
        var newArkFile = new LevLoader(path);
        
        Assert.True(newArkFile.TileMapObjectsBlocks[0].Tiles[159].FirstObjIdx == leftTile.FirstObjIdx);
        Assert.True(newArkFile.TileMapObjectsBlocks[0].Tiles[159].ObjectChain.Count == leftTile.ObjectChain.Count);
        Assert.True(
            (from obj in newArkFile.TileMapObjectsBlocks[0].Tiles[159].ObjectChain select obj.IdxAtObjectArray)
            .SequenceEqual
            (from obj in arkFile.TileMapObjectsBlocks[0].Tiles[159].ObjectChain select obj.IdxAtObjectArray)
            );

        var leftTile2 = Move10ItemsNearSpawn(newArkFile, settings);
        
        // TODO: When moving, this is finding the same tile and inverting the item sequence.
        // TODO: also, the blood stain is at the wrong z height. Check
        Assert.True(
            (from obj in newArkFile.TileMapObjectsBlocks[0].Tiles[159].ObjectChain select obj.IdxAtObjectArray).OrderBy(x=>x)
            .SequenceEqual
            ((from obj in arkFile.TileMapObjectsBlocks[0].Tiles[159].ObjectChain select obj.IdxAtObjectArray).OrderBy(x=>x))
            );
    }
    
}