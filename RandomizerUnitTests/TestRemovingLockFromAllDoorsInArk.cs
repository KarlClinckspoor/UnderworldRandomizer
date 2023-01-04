using System.Configuration;
using System.IO;
using NUnit.Framework;
using UWRandomizer;
using static UWRandomizerEditor.Utils;
using UWRandomizerEditor.LEVDotARK;
using UWRandomizerEditor.LEVDotARK.Blocks;
using UWRandomizerEditor.LEVDotARK.GameObjects.Specifics;

namespace RandomizerUnitTests;

public class TestRemovingLockFromAllDoorsInArk
{
    [OneTimeSetUp]
    public void SetUp()
    {
    }

    [Test]
    [Category("RequiresArk")]
    public void TestRemovingLockFromDoor_WithArk()
    {
        // var ArkOriginal = new ArkLoader(@"C:\Users\Karl\Desktop\UnderworldStudy\UW\DATA\LEV.ARK");
        // var ArkEditor = new ArkLoader(@"C:\Users\Karl\Desktop\UnderworldStudy\UW - Doors\DATA\LEV.ARK");
        var ArkOriginal = new ArkLoader(Paths.UW_ArkOriginalPath);
        var ArkEditor =
            new ArkLoader(Path.Join(Paths.BasePath, @"UW - Doors\Data\Lev.ark"));

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
        // var path = ArkOriginal.SaveBuffer(@"C:\Users\Karl\Desktop\UnderworldStudy\Buffers - Doors study",
        //     "ark_withdoor1012unlocked.bin");
        var path = StdSaveBuffer(ArkOriginal, Paths.BufferTestsPath, "ark_withdoor1012unlocked.bin");

        var ArkModified = new ArkLoader(path);
        var doorUnlockedHere = (Door) ArkModified.TileMapObjectsBlocks[0].AllGameObjects[1012];
        Assert.False(doorUnlockedHere.HasLock());
        Assert.True(doorUnlockedHere.Equals(doorUnlockedByEditor));
        Assert.True(
            doorUnlockedHere.Equals(new Door(new byte[] {0x41, 0x01, 0x50, 0x6F, 0x28, 0x00, 0x00, 0x00}, 1012)));
    }

    /// <summary>
    /// Returns -1 if buffers are of different length, 0 if they are equal or the index where they differed
    /// </summary>
    /// <param name="buffer1"></param>
    /// <param name="buffer2"></param>
    /// <returns></returns>
    private int CompareBuffers(byte[] buffer1, byte[] buffer2)
    {
        if (buffer1.Length != buffer2.Length)
        {
            return -1;
        }

        for (int i = 0; i < buffer1.Length; i++)
        {
            if (buffer1[i] != buffer2[i])
            {
                return i;
            }
        }

        return 0;
    }

    // This test isn't working ATM because somehow an object isn't updating its buffer. But checking with UE and UWE, it's fine
    [Test]
    [Category("RequiresArk")]
    public void TestRemovingAllLocks_WithArk()
    {
        // I'm testing here both the original and the "cleaned" version from UltimateEditor
        // var ArkOriginal = new ArkLoader(@"C:\Users\Karl\Desktop\UnderworldStudy\UW\DATA\LEV.ARK");
        // var ArkOriginalToModify = new ArkLoader(@"C:\Users\Karl\Desktop\UnderworldStudy\UW\DATA\LEV.ARK");
        var ArkOriginal = new ArkLoader(Paths.UW_ArkOriginalPath);
        var ArkOriginalToModify = new ArkLoader(Paths.UW_ArkOriginalPath);

        var ArkCleaned = new ArkLoader(Paths.UW_ArkCleanedPath);
        var ArkCleanedToModify = new ArkLoader(Paths.UW_ArkCleanedPath);

        RandoTools.RemoveAllDoorReferencesToLocks(ArkOriginalToModify);
        RandoTools.RemoveAllDoorReferencesToLocks(ArkCleanedToModify);

        var pathOriginal = StdSaveBuffer(ArkOriginalToModify, Path.Join(Paths.BufferTestsPath, "RemovingDoors"),
            "ark_nodoors.bin");
        var pathCleaned = StdSaveBuffer(ArkCleanedToModify, Path.Join(Paths.BufferTestsPath, "RemovingDoors"),
            "ark_cleaned_nodoors.bin");

        var ArkOriginalModified = new ArkLoader(pathOriginal);
        var ArkCleanedModified = new ArkLoader(pathCleaned);

        // Seeing if the buffers were saved correctly and if the buffers were altered in any way
        Assert.AreEqual(CompareBuffers(ArkOriginalModified.Buffer, ArkOriginalToModify.Buffer), 0);
        Assert.AreEqual(CompareBuffers(ArkCleanedModified.Buffer, ArkCleanedToModify.Buffer), 0);

        // Check if every byte of every block outside the StaticObject arrays is the same
        for (int blocknum = 0; blocknum < ArkLoader.NumOfLevels; blocknum++)
        {
            // TileMap
            Assert.AreEqual(CompareBuffers(
                ArkOriginal.TileMapObjectsBlocks[blocknum].TileMapBuffer,
                ArkOriginalModified.TileMapObjectsBlocks[blocknum].TileMapBuffer
            ), 0);

            Assert.AreEqual(CompareBuffers(
                ArkOriginal.TileMapObjectsBlocks[blocknum].MobileObjectInfoBuffer,
                ArkOriginalModified.TileMapObjectsBlocks[blocknum].MobileObjectInfoBuffer
            ), 0);

            Assert.AreEqual(CompareBuffers(
                ArkOriginal.TileMapObjectsBlocks[blocknum].FreeListMobileObjectBuffer,
                ArkOriginalModified.TileMapObjectsBlocks[blocknum].FreeListMobileObjectBuffer
            ), 0);

            Assert.AreEqual(CompareBuffers(
                ArkOriginal.TileMapObjectsBlocks[blocknum].FreeListStaticObjectBuffer,
                ArkOriginalModified.TileMapObjectsBlocks[blocknum].FreeListStaticObjectBuffer
            ), 0);

            // Should have modified only the StaticObjectBuffer (Doors)
            Assert.AreNotEqual(CompareBuffers(
                ArkOriginal.TileMapObjectsBlocks[blocknum].StaticObjectInfoBuffer,
                ArkOriginalModified.TileMapObjectsBlocks[blocknum].StaticObjectInfoBuffer
            ), 0);

            Assert.AreEqual(CompareBuffers(
                ArkCleaned.TileMapObjectsBlocks[blocknum].TileMapBuffer,
                ArkCleanedModified.TileMapObjectsBlocks[blocknum].TileMapBuffer
            ), 0);

            Assert.AreEqual(CompareBuffers(
                ArkCleaned.TileMapObjectsBlocks[blocknum].MobileObjectInfoBuffer,
                ArkCleanedModified.TileMapObjectsBlocks[blocknum].MobileObjectInfoBuffer
            ), 0);

            Assert.AreEqual(CompareBuffers(
                ArkCleaned.TileMapObjectsBlocks[blocknum].FreeListMobileObjectBuffer,
                ArkCleanedModified.TileMapObjectsBlocks[blocknum].FreeListMobileObjectBuffer
            ), 0);

            Assert.AreEqual(CompareBuffers(
                ArkCleaned.TileMapObjectsBlocks[blocknum].FreeListStaticObjectBuffer,
                ArkCleanedModified.TileMapObjectsBlocks[blocknum].FreeListStaticObjectBuffer
            ), 0);

            // Should have modified only the StaticObjectBuffer (Doors)
            Assert.AreNotEqual(CompareBuffers(
                ArkCleaned.TileMapObjectsBlocks[blocknum].StaticObjectInfoBuffer,
                ArkCleanedModified.TileMapObjectsBlocks[blocknum].StaticObjectInfoBuffer
            ), 0);

            // AutomapBlocks
            Assert.AreEqual(CompareBuffers(
                ArkOriginal.AutomapBlocks[blocknum].Buffer,
                ArkOriginalModified.AutomapBlocks[blocknum].Buffer
            ), 0);

            Assert.AreEqual(CompareBuffers(
                ArkCleaned.AutomapBlocks[blocknum].Buffer,
                ArkCleanedModified.AutomapBlocks[blocknum].Buffer
            ), 0);

            // Map Notes
            Assert.AreEqual(CompareBuffers(
                ArkOriginal.MapNotesBlocks[blocknum].Buffer,
                ArkOriginalModified.MapNotesBlocks[blocknum].Buffer
            ), 0);

            Assert.AreEqual(CompareBuffers(
                ArkCleaned.MapNotesBlocks[blocknum].Buffer,
                ArkCleanedModified.MapNotesBlocks[blocknum].Buffer
            ), 0);

            // ObjectAnimOverlay
            Assert.AreEqual(CompareBuffers(
                ArkOriginal.ObjAnimBlocks[blocknum].Buffer,
                ArkOriginalModified.ObjAnimBlocks[blocknum].Buffer
            ), 0);

            Assert.AreEqual(CompareBuffers(
                ArkCleaned.ObjAnimBlocks[blocknum].Buffer,
                ArkCleanedModified.ObjAnimBlocks[blocknum].Buffer
            ), 0);

            // TextureMapping
            Assert.AreEqual(CompareBuffers(
                ArkOriginal.TextMapBlocks[blocknum].Buffer,
                ArkOriginalModified.TextMapBlocks[blocknum].Buffer
            ), 0);

            Assert.AreEqual(CompareBuffers(
                ArkCleaned.TextMapBlocks[blocknum].Buffer,
                ArkCleanedModified.TextMapBlocks[blocknum].Buffer
            ), 0);


            for (int objnum = 0; objnum < TileMapMasterObjectListBlock.StaticObjectNum; objnum++)
            {
                var objOriginal = ArkOriginal.TileMapObjectsBlocks[blocknum].StaticObjects[objnum];
                var objModified = ArkOriginalModified.TileMapObjectsBlocks[blocknum].StaticObjects[objnum];

                var objCleaned = ArkCleaned.TileMapObjectsBlocks[blocknum].StaticObjects[objnum];
                var objCleanedModified = ArkCleanedModified.TileMapObjectsBlocks[blocknum].StaticObjects[objnum];

                if (objOriginal is Door doorOriginal) // Ignore doors
                {
                    if (doorOriginal.HasLock()) // If they have a lock, they should be different
                    {
                        Assert.True(CompareBuffers(doorOriginal.Buffer, ((Door) objModified).Buffer) > 0);
                        Assert.True(CompareBuffers(objOriginal.Buffer, objModified.Buffer) >
                                    0); // Will this be different?
                    }
                    else // Otherwise should be the same
                    {
                        Assert.True(CompareBuffers(doorOriginal.Buffer, ((Door) objModified).Buffer) == 0);
                        Assert.True(CompareBuffers(objOriginal.Buffer, objModified.Buffer) == 0);
                    }

                    continue;
                }

                if (objCleaned is Door doorCleaned) // Ignore doors
                {
                    if (doorCleaned.HasLock()) // If they have a lock, they should be different
                    {
                        Assert.True(CompareBuffers(doorCleaned.Buffer, ((Door) objCleanedModified).Buffer) > 0);
                        Assert.True(CompareBuffers(objCleaned.Buffer, objCleanedModified.Buffer) >
                                    0); // Will this be different?
                    }
                    else // Otherwise should be the same
                    {
                        Assert.True(CompareBuffers(doorCleaned.Buffer, ((Door) objCleanedModified).Buffer) == 0);
                        Assert.True(CompareBuffers(objCleaned.Buffer, objCleanedModified.Buffer) == 0);
                    }

                    continue;
                }

                Assert.AreEqual(CompareBuffers(objOriginal.Buffer, objModified.Buffer), 0);
                Assert.AreEqual(CompareBuffers(objCleaned.Buffer, objCleanedModified.Buffer), 0);
            }
        }
    }
}