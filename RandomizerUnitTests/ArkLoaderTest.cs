using System;
using System.Security.Cryptography;
using NUnit.Framework;
using Randomizer;
namespace RandomizerUnitTests;

class ArkLoaderTest
{
    // TODO: for this test to pass, need to put the correct length in the map info buffer.
    [Test]
    public void CompareLoadSerialize() 
    {
        var AL = new ArkLoader(@"C:\Users\Karl\Dropbox\UnderworldStudy\UW\DATA\LEV.ARK");
        Assert.True(AL.CompareCurrentArkWithHash());
        AL.arkbuffer = AL.ReconstructBufferFromBlocks(); // This was important
        AL.SaveBuffer(@"C:\Users\Karl\Dropbox\UnderworldStudy\UW\DATA");
        var AL2 = new ArkLoader($@"C:\Users\Karl\Dropbox\UnderworldStudy\UW\DATA\NEWLEV.ARK");
        Assert.True(AL2.CompareCurrentArkWithHash());

        for (int i = 0; i < AL.arkbuffer.Length; i++)
        {
            if (AL.arkbuffer[i] != AL2.arkbuffer[i])
            {
                Console.WriteLine($"Failed LEV.ARK comparison at byte {i}");
            }
        }
    }
}
