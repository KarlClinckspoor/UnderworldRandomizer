using System;
using System.IO;
using NUnit.Framework;
using UWRandomizerEditor.LEVDotARK;
using static UWRandomizerEditor.Utils;

namespace RandomizerUnitTests;

class ArkLoaderTest
{
    /// <summary>
    /// Loads an original LEV.ARK, checks if it's loaded correctly by its checksum, then reconstructs the buffers
    /// </summary>
    [Test]
    [Category("RequiresArk")]
    public void CompareLoadSerializeOriginal()
    {
        var myArkLoader = new ArkLoader(Paths.UW_ArkOriginalPath);
        Assert.True(Utils.CheckEqualityOfSha256Hash(myArkLoader.Buffer, Utils.OriginalLevArkSha256Hash));
        myArkLoader.ReconstructBuffer();
        Assert.True(Utils.CheckEqualityOfSha256Hash(myArkLoader.Buffer, Utils.OriginalLevArkSha256Hash));
        string savedPath = StdSaveBuffer(myArkLoader, Paths.BufferTestsPath, "reconstructedOriginalArk.bin");

        // Reloading and checking if everything was saved correctly.
        var myArkLoader2 = new ArkLoader(savedPath);
        myArkLoader2.ReconstructBuffer();
        var (differenceCount, differencePositions) = Utils.CompareTwoBuffers(myArkLoader.Buffer, myArkLoader2.Buffer);
        Assert.True(differenceCount == 0, "Differences at positions:" + String.Join(",", differencePositions));
        Assert.True(Utils.CheckEqualityOfSha256Hash(myArkLoader2.Buffer, Utils.OriginalLevArkSha256Hash));

        File.Delete(savedPath);
    }

    [Test]
    [Category("RequiresArk")]
    public void CompareLoadSerializeCleaned()
    {
        var AL = new ArkLoader(Paths.UW_ArkCleanedPath);
        var AL_Unreconstructed = new ArkLoader(Paths.UW_ArkCleanedPath);
        AL.ReconstructBuffer();
        var (diffs, positions) = Utils.CompareTwoBuffers(AL.Buffer, AL_Unreconstructed.Buffer);
        Assert.True(diffs == 0);
        string savedpath = StdSaveBuffer(AL, Paths.BufferTestsPath, "reconstructedCleanedArk.bin");

        var AL2 = new ArkLoader(savedpath);
        AL2.ReconstructBuffer();

        var (differenceCount, differencePositions) = Utils.CompareTwoBuffers(AL.Buffer, AL2.Buffer);
        Assert.True(differenceCount == 0, "Differences at positions:" + String.Join(",", differencePositions));
    }
}