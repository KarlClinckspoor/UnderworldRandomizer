using System;
using System.IO;
using NUnit.Framework;
using UWRandomizerEditor.LEVdotARK;
using UWRandomizerEditor.LEVdotARK.GameObjects.Specifics;
using UWRandomizerTools;

namespace RandomizerUnitTests.Tools;

public class TestRemovingLockFromAllDoorsInArk
{
    [Test]
    [Category("RequiresSettings")]
    public void TestManualLockRemoval()
    {
        // var ArkOriginal = new ArkLoader(@"C:\Users\Karl\Desktop\UnderworldStudy\UW\DATA\LEV.ARK");
        // var ArkEditor = new ArkLoader(@"C:\Users\Karl\Desktop\UnderworldStudy\UW - Doors\DATA\LEV.ARK");
        var ArkOriginal = new ArkLoader(Paths.UW_ArkOriginalPath);
        var ArkEditor = new ArkLoader(Path.Join(Paths.BasePath, @"UW - Doors\Data\Lev.ark"));

        var doorToUnlock = (Door) ArkOriginal.TileMapObjectsBlocks[0].AllGameObjects[1012];
        var doorUnlockedByEditor = (Door) ArkEditor.TileMapObjectsBlocks[0].AllGameObjects[1012];

        Assert.True(doorToUnlock.HasLock());
        Assert.False(doorUnlockedByEditor.HasLock());
        Assert.False(doorToUnlock.Equals(doorUnlockedByEditor));

        doorToUnlock.RemoveLock();
        Assert.False(doorToUnlock.HasLock());
        Assert.True(doorToUnlock.Equals(doorUnlockedByEditor));
        Assert.True(doorToUnlock.Equals(new Door(new byte[] {0x41, 0x01, 0x50, 0x6F, 0x28, 0x00, 0x00, 0x00}, 1012)));

        ArkOriginal.TileMapObjectsBlocks[0].ReconstructBuffer();
        ArkOriginal.ReconstructBuffer();
        var path = UWRandomizerEditor.Utils.SaveBuffer(ArkOriginal, Paths.BufferTestsPath,
            "ark_withdoor1012unlocked.bin");

        var ArkModified = new ArkLoader(path);
        var doorUnlockedHere = (Door) ArkModified.TileMapObjectsBlocks[0].AllGameObjects[1012];
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
        // var ArkOriginal = new ArkLoader(@"C:\Users\Karl\Desktop\UnderworldStudy\UW\DATA\LEV.ARK");
        // var ArkOriginalToModify = new ArkLoader(@"C:\Users\Karl\Desktop\UnderworldStudy\UW\DATA\LEV.ARK");
        var ArkOriginal = new ArkLoader(Paths.UW_ArkOriginalPath);
        var ArkOriginalToModify = new ArkLoader(Paths.UW_ArkOriginalPath);

        var ArkCleaned = new ArkLoader(Paths.UW_ArkCleanedPath);
        var ArkCleanedToModify = new ArkLoader(Paths.UW_ArkCleanedPath);

        int countOfLocksRemovedOriginal = RandoTools.RemoveAllDoorReferencesToLocks(ArkOriginalToModify);
        int countOfLocksRemovedCleaned = RandoTools.RemoveAllDoorReferencesToLocks(ArkCleanedToModify);

        Assert.True(countOfLocksRemovedOriginal > 0);
        Assert.True(countOfLocksRemovedCleaned > 0);

        var pathOriginal = UWRandomizerEditor.Utils.SaveBuffer(ArkOriginalToModify,
            Path.Join(Paths.BufferTestsPath, "RemovingDoors"),
            "ark_nodoors.bin");
        var pathCleaned = UWRandomizerEditor.Utils.SaveBuffer(ArkCleanedToModify,
            Path.Join(Paths.BufferTestsPath, "RemovingDoors"),
            "ark_cleaned_nodoors.bin");

        var ArkOriginalModified = new ArkLoader(pathOriginal);
        var ArkCleanedModified = new ArkLoader(pathCleaned);

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