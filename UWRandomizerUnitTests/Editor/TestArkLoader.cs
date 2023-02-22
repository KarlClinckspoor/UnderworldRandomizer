using System;
using System.IO;
using NUnit.Framework;
using UWRandomizerEditor.LEVDotARK;
using UWRandomizerEditor.LEVDotARK.GameObjects.Specifics;

namespace RandomizerUnitTests;

class ArkLoaderTest
{
    /// <summary>
    /// Tests if the original lev.ark is correctly reconstructed and serialized.
    /// </summary>
    [Test]
    [Category("RequiresArk")]
    public void CompareLoadSerializeOriginal()
    {
        var myArkLoader = new ArkLoader(Paths.UW_ArkOriginalPath);
        Assert.True(Utils.CheckEqualityOfSha256Hash(myArkLoader.Buffer, Utils.OriginalLevArkSha256Hash));
        myArkLoader.ReconstructBuffer();
        Assert.True(Utils.CheckEqualityOfSha256Hash(myArkLoader.Buffer, Utils.OriginalLevArkSha256Hash));
        string savedPath =
            UWRandomizerEditor.Utils.StdSaveBuffer(myArkLoader, Paths.BufferTestsPath, "reconstructedOriginalArk.bin");

        // Reloading and checking if everything was saved correctly.
        var myArkLoader2 = new ArkLoader(savedPath);
        myArkLoader2.ReconstructBuffer();
        var (differenceCount, differencePositions) = Utils.CompareTwoBuffers(myArkLoader.Buffer, myArkLoader2.Buffer);
        Assert.True(differenceCount == 0, "Differences at positions:" + String.Join(",", differencePositions));
        Assert.True(Utils.CheckEqualityOfSha256Hash(myArkLoader2.Buffer, Utils.OriginalLevArkSha256Hash));

        File.Delete(savedPath);
    }

    /// <summary>
    /// Tests if the "cleaned" lev.ark (opened and closed with UWEditor) is correctly reconstructed and serialized.
    /// </summary>
    [Test]
    [Category("RequiresArk")]
    public void TestReconstructBufferCleaned()
    {
        var AL = new ArkLoader(Paths.UW_ArkCleanedPath);
        var AL_Unreconstructed = new ArkLoader(Paths.UW_ArkCleanedPath);
        AL.ReconstructBuffer();
        var (diffs, positions) = Utils.CompareTwoBuffers(AL.Buffer, AL_Unreconstructed.Buffer);
        Assert.True(diffs == 0);
        string savedpath =
            UWRandomizerEditor.Utils.StdSaveBuffer(AL, Paths.BufferTestsPath, "reconstructedCleanedArk.bin");

        var AL2 = new ArkLoader(savedpath);
        AL2.ReconstructBuffer();

        var (differenceCount, differencePositions) = Utils.CompareTwoBuffers(AL.Buffer, AL2.Buffer);
        Assert.True(differenceCount == 0, "Differences at positions:" + String.Join(",", differencePositions));
    }

    /// <summary>
    /// This will test some items to see if they're in the appropriate positions, heights, correct classes, etc
    /// </summary>
    [Test]
    [Category("RequiresArk")]
    public void TestSpecificObjectProperties()
    {
        var AL = new ArkLoader(Paths.UW_ArkOriginalPath);
        // Testing Lvl1 starting bag
        var lvl1 = AL.TileMapObjectsBlocks[0];
        // X=33, Y=3
        // var tile = lvl1.TileInfos[33 * 64 + 3];
        // Console.WriteLine($"{tile.XYPos[0]}, {tile.XYPos[1]}");
        // Assert.True(tile.FirstObjIdx == 942);
        
        Assert.True(lvl1.AllGameObjects[942] is Container);
        Assert.False(lvl1.AllGameObjects[942].InContainer);
        Assert.True(lvl1.AllGameObjects[940].InContainer);
        Assert.True(lvl1.AllGameObjects[936].InContainer);
        Assert.True(lvl1.AllGameObjects[941].InContainer);
        Assert.True(lvl1.AllGameObjects[935].InContainer);
        Assert.True(lvl1.AllGameObjects[934].InContainer);
        Assert.True(lvl1.AllGameObjects[959].InContainer);

        Container cont = (Container) lvl1.AllGameObjects[942];
        Assert.True(cont.Contents.Count == 6);
        Assert.True(cont.Contents.RepresentingContainer);
        Assert.True(cont.Contents[^1].next == 0);
    }
}