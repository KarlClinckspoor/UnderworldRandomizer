using System;
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
class ArkLoaderTest
{
    /// <summary>
    /// Tests if the original lev.ark is correctly reconstructed and serialized.
    /// </summary>
    [Test]
    public void CompareLoadSerializeOriginal()
    {
        var myArkLoader = new LevLoader(Paths.UW1_ArkOriginalPath);
        Assert.True(Utils.CheckEqualityOfSha256Hash(myArkLoader.Buffer, Utils.OriginalLevArkSha256Hash));
        myArkLoader.ReconstructBuffer();
        Assert.True(Utils.CheckEqualityOfSha256Hash(myArkLoader.Buffer, Utils.OriginalLevArkSha256Hash));
        string savedPath =
            UWRandomizerEditor.Utils.SaveBuffer(myArkLoader, Paths.TestDataOutput, "reconstructedOriginalArk.bin");

        // Reloading and checking if everything was saved correctly.
        var myArkLoader2 = new LevLoader(savedPath);
        myArkLoader2.ReconstructBuffer();
        var (differenceCount, differencePositions) = Utils.CompareTwoBuffers(myArkLoader.Buffer, myArkLoader2.Buffer);
        Assert.True(differenceCount == 0, "Differences at positions:" + String.Join(",", differencePositions));
        Assert.True(Utils.CheckEqualityOfSha256Hash(myArkLoader2.Buffer, Utils.OriginalLevArkSha256Hash));

        File.Delete(savedPath);
    }

    [Test]
    public void CompareLoadSerializeDifficult()
    {
        var myArkLoader = new LevLoader(Paths.UW1_ArkDifficultPath);
        Assert.True(Utils.CheckEqualityOfSha256Hash(myArkLoader.Buffer, Utils.OriginalLevArkSha256Hash));
        myArkLoader.ReconstructBuffer();
        Assert.True(Utils.CheckEqualityOfSha256Hash(myArkLoader.Buffer, Utils.OriginalLevArkSha256Hash));
        string savedPath =
            UWRandomizerEditor.Utils.SaveBuffer(myArkLoader, Paths.BufferTestsPath, "reconstructedOriginalArk.bin");

        // Reloading and checking if everything was saved correctly.
        var myArkLoader2 = new LevLoader(savedPath);
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
    public void TestReconstructBufferCleaned()
    {
        var AL = new LevLoader(Paths.UW1_ArkCleanedPath);
        var AL_Unreconstructed = new LevLoader(Paths.UW1_ArkCleanedPath);
        AL.ReconstructBuffer();
        var (diffs, positions) = Utils.CompareTwoBuffers(AL.Buffer, AL_Unreconstructed.Buffer);
        Assert.True(diffs == 0);
        string savedpath =
            UWRandomizerEditor.Utils.SaveBuffer(AL, Paths.BufferTestsPath, "reconstructedCleanedArk.bin");

        var AL2 = new LevLoader(savedpath);
        AL2.ReconstructBuffer();

        var (differenceCount, differencePositions) = Utils.CompareTwoBuffers(AL.Buffer, AL2.Buffer);
        Assert.True(differenceCount == 0, "Differences at positions:" + String.Join(",", differencePositions));
    }

    /// <summary>
    /// This will test some items to see if they're in the appropriate positions, heights, correct classes, etc
    /// </summary>
    [Test]
    public void TestSpecificObjectProperties()
    {
        var AL = new LevLoader(Paths.UW1_ArkOriginalPath);
        // Testing Lvl1 starting bag
        var lvl1 = AL.MapObjBlocks[0];

        Assert.True(lvl1.AllGameObjects[942] is Container);
        Assert.False(lvl1.AllGameObjects[942].InContainer);
        Assert.True(lvl1.AllGameObjects[940].InContainer);
        Assert.True(lvl1.AllGameObjects[936].InContainer);
        Assert.True(lvl1.AllGameObjects[941].InContainer);
        Assert.True(lvl1.AllGameObjects[935].InContainer);
        Assert.True(lvl1.AllGameObjects[934].InContainer);
        Assert.True(lvl1.AllGameObjects[959].InContainer);

        Container cont = (Container)lvl1.AllGameObjects[942];
        Assert.True(cont.Contents.Count == 6);
        Assert.True(cont.Contents.RepresentingContainer);
        Assert.True(cont.Contents[^1].next == 0);
    }



}