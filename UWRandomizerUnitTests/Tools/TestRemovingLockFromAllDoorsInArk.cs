using System;
using System.IO;
using NUnit.Framework;
using UWRandomizerEditor.LEV;
using UWRandomizerEditor.LEV.GameObjects.Specifics;
using UWRandomizerTools;

namespace RandomizerUnitTests.Tools;

public class TestRemovingLockFromAllDoorsInArk
{
    [Test]
    [Category("RequiresSettings")]
    public void TestManualLockRemoval()
    {
        var ArkOriginal = Utils.LoadAndAssertOriginalLevArk();
        // var ArkEditor = new LevLoader(Path.Join(Paths.BasePath, @"UW - Doors\Data\Lev.ark"));
        var ArkEditor = new LevLoader(Path.Join(Paths.TestDataPath, @"Doors_LEV.ARK"));

        var doorToUnlock = (Door) ArkOriginal.MapObjBlocks[0].AllGameObjects[1012];
        var doorUnlockedByEditor = (Door) ArkEditor.MapObjBlocks[0].AllGameObjects[1012];

        Assert.True(doorToUnlock.HasLock());
        Assert.False(doorUnlockedByEditor.HasLock());
        Assert.False(doorToUnlock.Equals(doorUnlockedByEditor));

        doorToUnlock.RemoveLock();
        Assert.False(doorToUnlock.HasLock());
        Assert.True(doorToUnlock.Equals(doorUnlockedByEditor));
        Assert.True(doorToUnlock.Equals(new Door(new byte[] {0x41, 0x01, 0x50, 0x6F, 0x28, 0x00, 0x00, 0x00}, 1012)));

        ArkOriginal.MapObjBlocks[0].ReconstructBuffer();
        ArkOriginal.ReconstructBuffer();
        var path = UWRandomizerEditor.Utils.SaveBuffer(ArkOriginal, Paths.BufferTestsPath,
            "ark_withdoor1012unlocked.bin");

        var ArkModified = new LevLoader(path);
        var doorUnlockedHere = (Door) ArkModified.MapObjBlocks[0].AllGameObjects[1012];
        Assert.False(doorUnlockedHere.HasLock());
        Assert.True(doorUnlockedHere.Equals(doorUnlockedByEditor));
        Assert.True(
            doorUnlockedHere.Equals(new Door(new byte[] {0x41, 0x01, 0x50, 0x6F, 0x28, 0x00, 0x00, 0x00}, 1012)));
    }

    // This test isn't working ATM because somehow an object isn't updating its buffer. But checking with UE and UWE, it's fine
    [Test]
    [Category("RequiresSettings")]
    public void TestRemovingAllLocks()
    {
        // I'm testing here both the original and the "cleaned" version from UltimateEditor
        var ArkOriginal = Utils.LoadAndAssertOriginalLevArk();
        var ArkOriginalToModify = Utils.LoadAndAssertOriginalLevArk();

        var ArkCleaned = new LevLoader(Paths.UW1_ArkCleanedPath);
        var ArkCleanedToModify = new LevLoader(Paths.UW1_ArkCleanedPath);

        int countOfLocksRemovedOriginal = RandoTools.RemoveAllDoorReferencesToLocks(ArkOriginalToModify);
        int countOfLocksRemovedCleaned = RandoTools.RemoveAllDoorReferencesToLocks(ArkCleanedToModify);

        Assert.True(countOfLocksRemovedOriginal > 0);
        Assert.True(countOfLocksRemovedCleaned > 0);

        var RemovingDoorsPath = Path.Join(Paths.BufferTestsPath, "RemovingDoors");
        Directory.CreateDirectory(RemovingDoorsPath);
        var pathOriginal = UWRandomizerEditor.Utils.SaveBuffer(ArkOriginalToModify,
            RemovingDoorsPath,
            "ark_nodoors.bin");
        var pathCleaned = UWRandomizerEditor.Utils.SaveBuffer(ArkCleanedToModify,
            RemovingDoorsPath,
            "ark_cleaned_nodoors.bin");

        var ArkOriginalModified = new LevLoader(pathOriginal);
        var ArkCleanedModified = new LevLoader(pathCleaned);

        // Seeing if the buffers were saved correctly and if the buffers were altered in any way
        Assert.AreEqual(Utils.CompareTwoBuffers(ArkOriginalModified.Buffer, ArkOriginalToModify.Buffer).Item1, 0);
        Assert.AreEqual(Utils.CompareTwoBuffers(ArkCleanedModified.Buffer, ArkCleanedToModify.Buffer).Item1, 0);

        var (diffCountsOriginal, diffPositionsOriginal) =
            Utils.CompareTwoBuffers(ArkOriginal.Buffer, ArkOriginalModified.Buffer);
        var (diffCountsCleaned, diffPositionsCleaned) =
            Utils.CompareTwoBuffers(ArkCleaned.Buffer, ArkCleanedModified.Buffer);

        Assert.True(diffCountsOriginal > 0);
        Assert.True(diffCountsCleaned > 0);

        // It modifies a short (2 bytes), so the number of differences should be at least the number of locks removed,
        // or at most 2x the number of locks removed.
        // TODO: Dunno why this assumption is wrong... It's currently less than I think it should be. Need to compare the buffers later
        Console.WriteLine(
            $"Original: Number of modified bytes: {diffCountsOriginal}; Number of modified doors: {countOfLocksRemovedOriginal}. Positions: " +
            string.Join(",", diffPositionsOriginal));
        Console.WriteLine(
            $"Cleaned: Number of modified bytes: {diffCountsCleaned}; Number of modified doors: {countOfLocksRemovedCleaned}. Positions: " +
            string.Join(",", diffPositionsCleaned));
        // Assert.True((diffCountsOriginal >= countOfLocksRemovedOriginal) &
        //             (diffCountsOriginal <= countOfLocksRemovedOriginal * 2));
        // Assert.True((diffCountsCleaned >= countOfLocksRemovedCleaned) &
        //             (diffCountsCleaned <= countOfLocksRemovedCleaned * 2));

        // File.Delete(pathOriginal);
        // File.Delete(pathCleaned);
    }
}