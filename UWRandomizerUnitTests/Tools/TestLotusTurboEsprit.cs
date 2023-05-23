using System;
using System.IO;
using NUnit.Framework;
using UWRandomizerEditor.LEVdotARK;
using UWRandomizerEditor.LEVdotARK.GameObjects.Specifics;

namespace RandomizerUnitTests.Tools;

[TestFixture]
public class TestLotusTurboEsprit
{
    [Test]
    public void TestLotus()
    {
        var AL = new ArkLoader(Paths.UW_ArkOriginalPath);
        // var obj = AL.TileMapObjectsBlocks[0].StaticObjects[539] as Container;
        if (AL.TileMapObjectsBlocks[0].AllGameObjects[539] is Container cont)
        {
            cont.Contents.Add(AL.TileMapObjectsBlocks[0].AllGameObjects[532]); // Sack -> lotus
            Console.WriteLine("Here!");
            AL.TileMapObjectsBlocks[0].AllGameObjects[539].next = 0; // Sack -> end
            AL.TileMapObjectsBlocks[0].Tiles2D[31, 5].FirstObjIdx = 539;
            AL.TileMapObjectsBlocks[0].AllGameObjects[532].Invis = 0;
            AL.TileMapObjectsBlocks[0].ReconstructBuffer();
            // File.WriteAllBytes(AL.Path+"2", AL.Buffer);
        }

    }
}