using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using NUnit.Framework;
using UWRandomizerEditor;
using UWRandomizerEditor.LEVDotARK;
using static UWRandomizerEditor.Utils;

namespace RandomizerUnitTests;

class ArkLoaderTest
{
    [Test]
    [Category("RequiresArk")]
    public void CompareLoadSerializeOriginal()
    {
        var AL = new ArkLoader(Paths.UW_ArkOriginalPath);
        Assert.True(AL.CompareCurrentArkWithHash());
        AL.ReconstructBuffer();
        Assert.True(AL.CompareCurrentArkWithHash());
        string savedpath = StdSaveBuffer(AL, Paths.BufferTestsPath, "reconstructedOriginalArk.bin");
        var AL2 = new ArkLoader(savedpath);
        AL2.ReconstructBuffer();
        var (differenceCount, differencePositions) = Utils.CompareTwoBuffers(AL.Buffer, AL2.Buffer);
        Assert.True(differenceCount == 0, "Differences at positions:" + String.Join(",", differencePositions));
        Assert.True(AL2.CompareCurrentArkWithHash());
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