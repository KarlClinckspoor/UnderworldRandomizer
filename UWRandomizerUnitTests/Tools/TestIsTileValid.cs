using System.Linq;
using NUnit.Framework;
using UWRandomizerEditor.LEVdotARK;
using UWRandomizerTools;

namespace RandomizerUnitTests;

[TestFixture]
public class TestIsTileValid
{

    [Test]
    public void TestTilePositionsLvls1To6([Range(0, 5)] int levelNumber)
    {
        for (uint i = 0; i < 64*64; i++)
        {
            var Tile = new TileInfo(i, 0, i * TileInfo.FixedSize, levelNumber) {TileType = (uint) TileInfo.TileTypes.Open};
            if (((Tile.XPos>= 30) & (Tile.XPos <= 33) & (Tile.YPos >= 30) & (Tile.YPos <= 33))) 
                Assert.False(ShuffleItems.IsTileValid(Tile));
            else
                Assert.True(ShuffleItems.IsTileValid(Tile));
        }
    }

    [Test]
    public void TestTilePositionsLvl7()
    {
        uint[][] invalidPositions = new[]
        {
            new[] {30u, 35u},
            new[] {31u, 35u},
            new[] {32u, 35u},
            new[] {33u, 35u},
            new[] {30u, 34u},
            new[] {31u, 34u},
            new[] {32u, 34u},
            new[] {33u, 34u},
            new[] {30u, 33u},
            new[] {31u, 33u},
            new[] {32u, 33u},
            new[] {33u, 33u},
            new[] {30u, 32u},
            new[] {31u, 32u},
            new[] {32u, 32u},
            new[] {33u, 32u},
            new[] {30u, 31u},
            new[] {31u, 31u},
            new[] {32u, 31u},
            new[] {33u, 31u},
            new[] {30u, 30u},
            new[] {31u, 30u},
            new[] {32u, 30u},
            new[] {33u, 30u},
            new[] {30u, 29u},
            new[] {31u, 29u},
            new[] {32u, 29u},
            new[] {33u, 29u},
            new[] {29u, 34u},
            new[] {29u, 33u},
            new[] {29u, 32u},
            new[] {29u, 31u},
            new[] {29u, 30u},
            new[] {28u, 33u},
            new[] {28u, 32u},
            new[] {28u, 31u},
            new[] {34u, 34u},
            new[] {34u, 33u},
            new[] {34u, 32u},
            new[] {34u, 31u},
            new[] {34u, 30u},
            new[] {35u, 33u},
            new[] {35u, 32u},
            new[] {35u, 31u}
        };
            
        for (uint i = 0; i < 64 * 64; i++)
        {
            var Tile = new TileInfo(i, 0, i * TileInfo.FixedSize, 6) {TileType = (uint) TileInfo.TileTypes.Open};
            if (invalidPositions.Any(x=>x.SequenceEqual(Tile.XYPos)))
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
            var Tile = new TileInfo(i, 0, i * TileInfo.FixedSize, 7) {TileType = (uint) TileInfo.TileTypes.Open};
            if ((Tile.XPos >= 29) & (Tile.XPos <= 35) & (Tile.YPos >= 30) & (Tile.YPos <= 35))
                Assert.False(ShuffleItems.IsTileValid(Tile), $"X:{Tile.XPos} Y:{Tile.YPos}");
            else if (Tile.XPos >= 31 & Tile.XPos <= 33 & Tile.YPos == 29)
                Assert.False(ShuffleItems.IsTileValid(Tile), $"X:{Tile.XPos} Y:{Tile.YPos}");
            else
                Assert.True(ShuffleItems.IsTileValid(Tile), $"X:{Tile.XPos} Y:{Tile.YPos}");
        }
    }
}