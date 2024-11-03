using System.Linq;
using NUnit.Framework;
using UWRandomizerEditor.LEV;
using UWRandomizerTools;

namespace RandomizerUnitTests.Tools;

[TestFixture]
public class TestIsTileValid
{

    [Test]
    public void TestTilePositionsLvls1To6([Range(0, 5)] int levelNumber)
    {
        for (uint i = 0; i < 64*64; i++)
        {
            var Tile = new Tile(i, 0, i * UWRandomizerEditor.LEV.Tile.FixedSize, levelNumber) {TileType = (uint) UWRandomizerEditor.LEV.Tile.TileTypes.Open};
            if (((Tile.Row>= 30) & (Tile.Row <= 33) & (Tile.Column >= 30) & (Tile.Column <= 33))) 
                Assert.False(ShuffleItems.IsTileValid(Tile));
            else
                Assert.True(ShuffleItems.IsTileValid(Tile));
        }
    }

    [Test]
    public void TestTilePositionsLvl7()
    {
        int[][] invalidPositions = new[]
        {
            new[] {30, 35},
            new[] {31, 35},
            new[] {32, 35},
            new[] {33, 35},
            new[] {30, 34},
            new[] {31, 34},
            new[] {32, 34},
            new[] {33, 34},
            new[] {30, 33},
            new[] {31, 33},
            new[] {32, 33},
            new[] {33, 33},
            new[] {30, 32},
            new[] {31, 32},
            new[] {32, 32},
            new[] {33, 32},
            new[] {30, 31},
            new[] {31, 31},
            new[] {32, 31},
            new[] {33, 31},
            new[] {30, 30},
            new[] {31, 30},
            new[] {32, 30},
            new[] {33, 30},
            new[] {30, 29},
            new[] {31, 29},
            new[] {32, 29},
            new[] {33, 29},
            new[] {29, 34},
            new[] {29, 33},
            new[] {29, 32},
            new[] {29, 31},
            new[] {29, 30},
            new[] {28, 33},
            new[] {28, 32},
            new[] {28, 31},
            new[] {34, 34},
            new[] {34, 33},
            new[] {34, 32},
            new[] {34, 31},
            new[] {34, 30},
            new[] {35, 33},
            new[] {35, 32},
            new[] {35, 31}
        };
            
        for (uint i = 0; i < 64 * 64; i++)
        {
            var Tile = new Tile(i, 0, i * UWRandomizerEditor.LEV.Tile.FixedSize, 6) {TileType = (uint) UWRandomizerEditor.LEV.Tile.TileTypes.Open};
            if (invalidPositions.Any(x=>x.SequenceEqual(Tile.XYPos.ToArray())))
                Assert.False(ShuffleItems.IsTileValid(Tile));
            else
                Assert.True(ShuffleItems.IsTileValid(Tile));
        }
    }
    
    [Test]
    public void TestTilePositionsLvl8()
    {
        for (uint i = 0; i < 64 * 64; i++)
        {
            var Tile = new Tile(i, 0, i * UWRandomizerEditor.LEV.Tile.FixedSize, 7) {TileType = (uint) UWRandomizerEditor.LEV.Tile.TileTypes.Open};
            if ((Tile.Row >= 29) & (Tile.Row <= 35) & (Tile.Column >= 30) & (Tile.Column <= 35))
                Assert.False(ShuffleItems.IsTileValid(Tile), $"X:{Tile.Row} Y:{Tile.Column}");
            else if (Tile.Row >= 31 & Tile.Row <= 33 & Tile.Column == 29)
                Assert.False(ShuffleItems.IsTileValid(Tile), $"X:{Tile.Row} Y:{Tile.Column}");
            else
                Assert.True(ShuffleItems.IsTileValid(Tile), $"X:{Tile.Row} Y:{Tile.Column}");
        }
    }
}