using System;
using System.IO;
using NUnit.Framework;
using Randomizer;
namespace RandomizerUnitTests;

class ArkLoaderTest
{
    [Test]
    public void CompareLoadSerialize() 
    {
        var AL = new ArkLoader(Settings.DefaultArkPath);
        Assert.True(AL.CompareCurrentArkWithHash());
        AL.arkbuffer = AL.ReconstructBufferFromBlocks();
        string savedpath = AL.SaveBuffer(Path.GetDirectoryName(Settings.DefaultArkPath));
        var AL2 = new ArkLoader(savedpath);
        Assert.True(AL2.CompareCurrentArkWithHash());

        for (int i = 0; i < AL.arkbuffer.Length; i++)
        {
            if (AL.arkbuffer[i] != AL2.arkbuffer[i])
            {
                Console.WriteLine($"Failed LEV.ARK comparison at byte {i}");
            }
        }
    }
    // TODO: Make one here to test the buffer lengths
}
