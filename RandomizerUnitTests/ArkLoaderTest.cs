using System;
using NUnit.Framework;
using Randomizer;
namespace RandomizerUnitTests;

class ArkLoaderTest
{
    [Test]
    public void CompareLoadSerialize() 
    {
        var AL = new ArkLoader(@"D:\Dropbox\UnderworldStudy\UW\DATA\LEV.ARK");
        Assert.True(AL.CompareCurrentArkWithHash());
        AL.arkbuffer = AL.ReconstructBufferFromBlocks();
        AL.SaveBuffer(@"D:\Dropbox\UnderworldStudy\UW\DATA");
        var AL2 = new ArkLoader($@"D:\Dropbox\UnderworldStudy\UW\DATA\NEWLEV.ARK");
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
