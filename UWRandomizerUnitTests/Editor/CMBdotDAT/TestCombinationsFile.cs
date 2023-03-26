using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UWRandomizerEditor.CMBdotDAT;

namespace RandomizerUnitTests;

[TestFixture]
public class TestCombinationsFile
{
    private CombinationsFile stdFile;

    [SetUp]
    public void SetUp()
    {
        // Implements these combinations from UW1
        // 0: (149:0x95)a_lit_torch+(278:0x116)a_block_of_incense_blocks_of_incense*=(277:0x115)a_block_of_burning_incense_blocks_of_burning_incense
        // 1: (225:0xE1)the_Key_of_Truth*+(226:0xE2)the_Key_of_Love*=(230:0xE6)a_two_part_key
        // 2: (225:0xE1)the_Key_of_Truth*+(227:0xE3)the_Key_of_Courage*=(228:0xE4)a_two_part_key
        // 3: (226:0xE2)the_Key_of_Love*+(227:0xE3)the_Key_of_Courage*=(229:0xE5)a_two_part_key
        // 4: (225:0xE1)the_Key_of_Truth*+(229:0xE5)a_two_part_key*=(231:0xE7)the_Key_of_Infinity
        // 5: (226:0xE2)the_Key_of_Love*+(228:0xE4)a_two_part_key*=(231:0xE7)the_Key_of_Infinity
        // 6: (227:0xE3)the_Key_of_Courage*+(230:0xE6)a_two_part_key*=(231:0xE7)the_Key_of_Infinity
        // 7: (149:0x95)a_lit_torch+(180:0xB4)an_ear_of_corn_ears_of_corn*=(183:0xB7)some_popcorn_bunches_of_popcorn
        // 8: (284:0x11C)some_strong_thread_pieces_of_strong_thread*+(216:0xD8)a_pole*=(299:0x12B)a_fishing_pole
        stdFile = new CombinationsFile(
            new List<ItemCombination>()
            {
                new ItemCombination( // 0
                    new ItemDescriptor(149, false), // lit torch
                    new ItemDescriptor(278, true), // block of incense
                    new ItemDescriptor(277, false) // block of burning incense
                ),
                new ItemCombination( // 1
                    new ItemDescriptor(225, true), // key truth
                    new ItemDescriptor(226, true), // key love
                    new ItemDescriptor(230, false) // two part key
                ),
                new ItemCombination( // 2
                    new ItemDescriptor(225, true), // key truth
                    new ItemDescriptor(227, true), // key courage
                    new ItemDescriptor(228, false) // two part key
                ),
                new ItemCombination( // 3
                    new ItemDescriptor(226, true), // key courage
                    new ItemDescriptor(227, true), // key love
                    new ItemDescriptor(229, false) // two part key
                ),
                new ItemCombination( // 4
                    new ItemDescriptor(225, true), // key truth
                    new ItemDescriptor(229, true), // two part key
                    new ItemDescriptor(231, false) // key infty
                ),
                new ItemCombination( // 5
                    new ItemDescriptor(226, true), // key love
                    new ItemDescriptor(228, true), // two part key
                    new ItemDescriptor(231, false) // key infty
                ),
                new ItemCombination( // 6
                    new ItemDescriptor(227, true), // key courage
                    new ItemDescriptor(230, true), // two part key
                    new ItemDescriptor(231, false) // key infty
                ),
                new ItemCombination( // 7
                    new ItemDescriptor(149, false), // lit torch
                    new ItemDescriptor(180, true), // corn
                    new ItemDescriptor(183, false) // popcorn
                ),
                new ItemCombination( // 8
                    new ItemDescriptor(284, true), // thread
                    new ItemDescriptor(216, true), // pole
                    new ItemDescriptor(299, false) // fishing pole
                ),
                new FinalCombination()
            }
        );
    }
    /// <summary>
    /// Computed with HxD. Date modified: 1993/06/16
    /// </summary>
    private string correctSHA256 = "15A97C476F5E6D72FA75F52B321C5FFAA0131BB9117D3D9B08742C77F8A7B098".ToLower();

    [Test]
    public void TestCtorList()
    {
        Assert.True(Utils.CheckEqualityOfSha256Hash(stdFile.Buffer, correctSHA256));
        Assert.True(stdFile.CheckConsistency());
    }

    [Test]
    [Category("RequiresSettings")]
    public void TestCtorBuffer()
    {
        var path = Paths.UW_CMBPath;
        var combinations = new CombinationsFile(path);
        Assert.True(Utils.CheckEqualityOfSha256Hash(combinations.Buffer, correctSHA256)); // Just to be safe the following are correct
        for (int i = 0; i < stdFile.Combinations.Count; i++)
        {
            Assert.True(stdFile.Combinations[i].Buffer.SequenceEqual(combinations.Combinations[i].Buffer));
            Assert.True(stdFile.Combinations[i].Equals(combinations.Combinations[i]));
        }
        Assert.True(combinations.CheckConsistency());
    }

    [Test]
    public void TestRemovalLastItem()
    {
        Assert.Throws<ItemCombinationException>(() => stdFile.RemoveCombinationAt(stdFile.Combinations.Count - 1));
    }
    
    [Test]
    public void TestRemovalOutOfBoundsItem()
    {
        Assert.Throws<IndexOutOfRangeException>(() => stdFile.RemoveCombinationAt(int.MaxValue));
    }

    [Test]
    public void TestCorrectRemovalOfCombination()
    {
        int previousLength = stdFile.Buffer.Length;
        int expectedLength = previousLength - ItemCombination.FixedBufferSize;
        stdFile.RemoveCombinationAt(0);
        Assert.True(stdFile.CheckConsistency());
        Assert.True(stdFile.Buffer.Length == expectedLength);
    }

    [Test]
    public void TestConsistency()
    {
        var combFileWOEnd = new CombinationsFile(new List<ItemCombination>()
            {
                new ItemCombination(
                    new ItemDescriptor(1, false),
                  new ItemDescriptor(2, true),
                    new ItemDescriptor(3, false))
            }
        );
        var combFileNoneDestroyed = new CombinationsFile(new List<ItemCombination>()
        {
            new ItemCombination(
                new ItemDescriptor(1, false),
                new ItemDescriptor(2, false),
                new ItemDescriptor(3, false)),
            new FinalCombination()
        }
        );
        Assert.True(stdFile.CheckConsistency());
        Assert.False(combFileWOEnd.CheckConsistency());
        Assert.False(combFileNoneDestroyed.CheckConsistency());
    }
    
    
}