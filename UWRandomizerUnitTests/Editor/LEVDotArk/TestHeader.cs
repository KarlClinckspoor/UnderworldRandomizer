using NUnit.Framework;
using UWRandomizerEditor.LEVdotARK;

namespace RandomizerUnitTests.Editor.LEVDotArk;

[Category("RequiresSettings")]
// ReSharper disable once InconsistentNaming
public class HeaderTestUW1
{
    private LevLoader _myArkLoader = null!;
    private Header _header = null!;
    private const int ValidEntriesWithInfo = 9 * 3;
    private const int ValidEntries = 45;
    private const int EmptyEntries = 90;
    private const int TotalEntries = ValidEntries + EmptyEntries;

    [SetUp]
    public void SetUp()
    {
        _myArkLoader = new LevLoader(Paths.UW1_ArkOriginalPath);
        _header = _myArkLoader.header;
    }

    // 542
    [Test]
    public void TestHeaderSize()
    {
        Assert.True(_header.Size == _header.Buffer.Length);
    }

    [Test]
    public void TestGetOffsetForBlock()
    {
        for (int i = 0; i < TotalEntries; i++)
        {
            int offset = _header.GetOffsetForBlock(i);
            if (i < ValidEntriesWithInfo)
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
            // Texture Mapping
            289766, 289888, 290010, 290132, 290254, 290376, 290498, 290620, 290742,
            // Automap info
            0, 0, 0, 0, 0, 0, 0, 0, 0,
            // map notes
            0, 0, 0, 0, 0, 0, 0, 0, 0
        };

        for (int i = 0; i < ValidEntries; i++)
        {
            Assert.True(_header.BlockOffsets[i] == validEntryOffsets[i]);
        }

        for (int i = ValidEntries; i < TotalEntries; i++)
        {
            Assert.True(_header.BlockOffsets[i] == 0);
        }
    }
}