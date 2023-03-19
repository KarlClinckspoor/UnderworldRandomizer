using System;
using System.IO;
using NUnit.Framework;
using UWRandomizerWPF;
using UWRandomizerEditor.LEVdotARK;
using UWRandomizerEditor.LEVdotARK.Blocks;
using UWRandomizerTools;

namespace RandomizerUnitTests;

public class TestWhatUWEditorFixes
{
    [Test]
    [Category("RequiresSettings")]
    [Category("FishyTests")]
    public void DetectDifferencesAfterLoadingInUltimateEditor()
    {
        var ArkCleaned = new ArkLoader(Paths.UW_ArkCleanedPath);
        var ArkCleanedNoDoors = new ArkLoader(Paths.UW_ArkCleanedPath);
        var countLocksRemoved =
            RandoTools.RemoveAllDoorReferencesToLocks(ArkCleanedNoDoors); // Assumes this is working correctly

        var cleanedNoDoorsPath =
            UWRandomizerEditor.Utils.StdSaveBuffer(ArkCleanedNoDoors.Buffer, Paths.BufferTestsPath,
                "cleaned_nodoors.ark");

        // Every time I change the function to remove all locks, this has to be re-generated
        while (!File.Exists(Paths.UWArkCleanedNoDoorsPath_Fixed))
        {
            Console.WriteLine(
                "cleaned_nodoors_fixed.ark in the buffer test folder not found. Please create it then press enter");
            Console.ReadLine();
        }

        var ArkCleanedNoDoorsFixed = new ArkLoader(Paths.UWArkCleanedNoDoorsPath_Fixed);

        var (countDiffs, positionDiffs) =
            Utils.CompareTwoBuffers(ArkCleanedNoDoors.Buffer, ArkCleanedNoDoorsFixed.Buffer);
        Console.WriteLine(string.Join("\n", positionDiffs));
        Assert.True(countDiffs == 0);
    }
}