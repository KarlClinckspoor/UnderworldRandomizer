using System;
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
        string savedpath = StdSaveBuffer(AL, Paths.BufferTestsPath, "reconstructedOriginalArk.bin");
        var AL2 = new ArkLoader(savedpath);
        Assert.True(AL2.CompareCurrentArkWithHash());

        for (int i = 0; i < AL.Buffer.Length; i++)
        {
            if (AL.Buffer[i] != AL2.Buffer[i])
            {
                Console.WriteLine($"Failed LEV.ARK comparison at byte {i}");
            }
        }
    }

    [Test]
    [Category("RequiresArk")]
    public void CompareLoadSerializeCleaned()
    {
        var AL = new ArkLoader(Paths.UW_ArkCleanedPath);
        AL.ReconstructBuffer();
        string savedpath = StdSaveBuffer(AL, Paths.BufferTestsPath, "reconstructedCleanedArk.bin");
        var AL2 = new ArkLoader(savedpath);

        for (int i = 0; i < AL.Buffer.Length; i++)
        {
            if (AL.Buffer[i] != AL2.Buffer[i])
            {
                Console.WriteLine($"Failed LEV.ARK comparison at byte {i}");
            }
        }
    }
}