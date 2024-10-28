using System;
using System.IO;
using NUnit.Framework;
using UWRandomizerEditor.LEVdotARK;
using UWRandomizerTools;

namespace RandomizerUnitTests.Tools;

public class TestWhatUWEditorFixes
{
    [Test]
    [Category("RequiresSettings")]
    [Category("FishyTests")]
    public void DetectDifferencesAfterLoadingInUltimateEditor()
    {
        var ArkCleaned = new LevLoader(Paths.UW1_ArkCleanedPath);
        var ArkCleanedNoDoors = new LevLoader(Paths.UW1_ArkCleanedPath);
        var countLocksRemoved =
            RandoTools.RemoveAllDoorReferencesToLocks(ArkCleanedNoDoors); // Assumes this is working correctly

        var cleanedNoDoorsPath =
            UWRandomizerEditor.Utils.SaveBuffer(ArkCleanedNoDoors.Buffer, Paths.BufferTestsPath,
                "cleaned_nodoors.ark");

        // Every time I change the function to remove all locks, this has to be re-generated
        // while (!File.Exists(Paths.UWArkCleanedNoDoorsPath_Fixed))
        while (!File.Exists("./cleaned_nodoors_fixed.ark"))
        {
            Console.WriteLine(
                "cleaned_nodoors_fixed.ark in the buffer test folder not found. Please create it then press enter");
            Console.ReadLine();
        }

        // var ArkCleanedNoDoorsFixed = new LevLoader(Paths.UWArkCleanedNoDoorsPath_Fixed);
        var ArkCleanedNoDoorsFixed = new LevLoader("./cleaned_nodoors_fixed.ark");

        var (countDiffs, positionDiffs) =
            Utils.CompareTwoBuffers(ArkCleanedNoDoors.Buffer, ArkCleanedNoDoorsFixed.Buffer);
        Console.WriteLine(string.Join("\n", positionDiffs));
        Assert.True(countDiffs == 0);
    }
}