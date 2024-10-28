using System;
using System.Linq;
using NUnit.Framework;
using UWRandomizerEditor.LEV;
using UWRandomizerEditor.LEV.GameObjects;

namespace RandomizerUnitTests;

[TestFixture]
[Category("RequiresSpecificFiles")]
public class TestBugsFromGOG
{
    [Test]
    public void TestBugInGOG()
    {
        var path = @"C:\Users\karl9\OneDrive\UnderworldStudy\UW\SAVE1\LEV.ARK";
        var AL = new LevLoader(path);

        var problematicObject = AL.TileMapObjectsBlocks[4].AllGameObjects[0x01a9];
        var shouldBeFreeObject = AL.TileMapObjectsBlocks[4].AllGameObjects[0x01a7];
        var coinID = 160;
        var temp1 = AL.TileMapObjectsBlocks[4].FreeMobileObjIndexes.ToList();
        var temp2 = AL.TileMapObjectsBlocks[4].FreeStaticObjIndexes.ToList();
        var temp3 = temp1.Concat(temp2);
        int counter = 1;
        Console.Write("[0000] ");
        foreach (var x in temp3)
        {
            Console.Write($"[{x:x4}] ");
            if ((counter + 1) % 16 == 0)
            {
                Console.WriteLine();
            }

            counter++;
        }
        Assert.False(AL.TileMapObjectsBlocks[4].isObjectInFreeSlot(problematicObject));
        Assert.True(AL.TileMapObjectsBlocks[4].isObjectInFreeSlot(shouldBeFreeObject));

        var potentialBuffer = new StaticObject(0x80a0, 0x5080, 0x0028, 0x01c0, 0x01a9);
        
        Console.WriteLine(problematicObject);
        Console.WriteLine(potentialBuffer);
    }
    
    [Test]
    public void TestBugInGOG2()
    {
        var path = @"C:\Users\karl9\OneDrive\UnderworldStudy\UW\SAVE1\LEV.ARK";
        var AL = new LevLoader(path);

        foreach (var block in AL.TileMapObjectsBlocks)
        {
            foreach (var obj in block.AllGameObjects)
            {
                if ((obj.ReferenceCount == 0) & (!block.isObjectInFreeSlot(obj)))
                {
                    Console.WriteLine($"lvl {block.LevelNumber}: Potentially problematic object {{obj}} at idx {obj.IdxAtObjectArray}");
                }
            }
        }
    }
    
}