using System;
using System.IO;
using NUnit.Framework;
using UWRandomizerEditor.LEV;
using UWRandomizerEditor.LEV.GameObjects.Specifics;

namespace RandomizerUnitTests.Tools;

[TestFixture]
public class TestLotusTurboEsprit
{
    [Test]
    [Category("RequiresSettings")]
    public void TestLotus()
    {
        var AL = new LevLoader(Paths.UW1_ArkOriginalPath);
        // var obj = AL.MapObjBlocks[0].StaticObjects[539] as Container;
        if (AL.MapObjBlocks[0].AllGameObjects[539] is Container cont)
        {
            cont.Contents.Add(AL.MapObjBlocks[0].AllGameObjects[532]); // Sack -> lotus
            Console.WriteLine("Here!");
            AL.MapObjBlocks[0].AllGameObjects[539].next = 0; // Sack -> end
            AL.MapObjBlocks[0].Tiles2D[31, 5].FirstObjIdx = 539;
            AL.MapObjBlocks[0].AllGameObjects[532].Invis = 0;
            AL.MapObjBlocks[0].ReconstructBuffer();
            File.WriteAllBytes(AL.Path+"lotus_esprit", AL.Buffer);
        }

    }
}