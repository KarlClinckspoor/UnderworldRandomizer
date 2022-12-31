using System.Configuration;
using NUnit.Framework;
using UWRandomizerEditor.LEVDotARK;

namespace RandomizerUnitTests;

[Category("RequiresArk")]
public class HeaderTestUW1
{
    private ArkLoader AL;
    private Header header;
    static int validEntriesWithInfo = 9 * 3;
    static int validEntries = 45;
    static int emptyEntries = 90;
    static int totalEntries = validEntries + emptyEntries;

    [SetUp]
    public void SetUp()
    {
        AL = new ArkLoader(Paths.UW_ArkOriginalPath);
        Assert.True(AL.CompareCurrentArkWithHash()); // This is supposed to work with pristine UW1 lev.ark
        header = AL.header;
    }

    // 542
    [Test]
    public void TestHeaderSize()
    {
        Assert.True(header.Size == header.Buffer.Length);
    }

    [Test]
    public void TestGetOffsetForBlock()
    {
        for (int i = 0; i < totalEntries; i++)
        {
            int offset = header.GetOffsetForBlock(i);
            if (i < validEntriesWithInfo)
            {
                Assert.True(offset > 0);
            }
            else
            {
                Assert.True(offset == 0);
            }
        }
    }

    [Test]
    public void TestReadOffsets()
    {
        int[] validEntryOffsets = new[]
        {
            // Level tilemap...
            542, 32294, 64046, 95798, 127550, 159302, 191054, 222806, 254558,
            // Animation overlay
            286310, 286694, 287078, 287462, 287846, 288230, 288614, 288998, 289382,
            // Texturemapping
            289766, 289888, 290010, 290132, 290254, 290376, 290498, 290620, 290742,
            // automap info
            0, 0, 0, 0, 0, 0, 0, 0, 0,
            // map notes
            0, 0, 0, 0, 0, 0, 0, 0, 0
        };

        for (int i = 0; i < validEntries; i++)
        {
            Assert.True(header.BlockOffsets[i] == validEntryOffsets[i]);
        }

        for (int i = validEntries; i < totalEntries; i++)
        {
            Assert.True(header.BlockOffsets[i] == 0);
        }
    }
}