using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UWRandomizerEditor.LEVdotARK;

namespace RandomizerUnitTests;

public class Utils
{
    public const string OriginalLevArkSha256Hash = "87e9a6e5d249df273e1964f48ad910afee6f7e073165c00237dfb9a22ae3a121";

    /// <summary>
    /// Compares two buffers. Returns a tuple containing the number of differences, and the offset positions.
    /// </summary>
    /// <param name="reference">Reference array</param>
    /// <param name="other">Array to compare against reference</param>
    /// <returns>Tuple with number of differences and offset positions</returns>
    public static Tuple<int, List<int>> CompareTwoBuffers(byte[] reference, byte[] other)
    {
        if (reference.Length != other.Length)
        {
            throw new ArgumentException(
                $"Arrays of incompatible lengths: reference {reference.Length} != other {other.Length}");
        }

        int diffCount = 0;
        List<int> differenceOffsets = new List<int>();
        for (int i = 0; i < reference.Length; i++)
        {
            if (reference[i] != other[i])
            {
                differenceOffsets.Add(i);
                diffCount += 1;
            }
        }

        return new Tuple<int, List<int>>(diffCount, differenceOffsets);
    }

    public static bool CheckEqualityOfSha256Hash(byte[] Buffer, string hashToCompare)
    {
        SHA256 mySHA256 = SHA256.Create();
        // Link is converting byte[] into a sequence of hex values, guaranteed to have 2 places (so 7->07)
        var bufferHash = string.Join("", mySHA256.ComputeHash(Buffer).Select(x => x.ToString("x2")));
        return bufferHash == hashToCompare;
    }

    private class OffsetDescriptor
    {
        public int Offset;
        public string BlockName;
        public int BlockNum;

        public OffsetDescriptor(string blockName, int offset, int blockNum)
        {
            Offset = offset;
            BlockName = blockName;
            BlockNum = blockNum;
        }
    }

    public static string DetermineBufferObjectFromAbsoluteOffset_OriginalArk(int offset)
    {
        // These constants are valid for the original lev.ark of UW1. The offsets come from the header
        const int numOfLevels = 9;

        // Header
        const int headerOffset = 0;
        const int headerBlockPositionsOffset = 4;
        const int headerSize = 542;

        // Block sizes
        const int tileMapSize = 31752;
        const int animInfoSize = 384;
        const int textMapSize = 122;

        // Block offsets
        const int tileMapsOffset = headerSize; // 542
        const int animInfosOffset = tileMapsOffset + numOfLevels * tileMapSize; // 286310
        const int textMapsOffset = animInfosOffset + numOfLevels * animInfoSize; // 289766
        const int variableWidthBlocksOffset = textMapsOffset + numOfLevels * textMapSize; // 290864 -- upper bound

        // Populating list of offsets and names
        List<OffsetDescriptor> blockOffsetsAndNames = new List<OffsetDescriptor>();
        blockOffsetsAndNames.Add(new OffsetDescriptor("HeaderBlockCount", headerOffset, 0));
        blockOffsetsAndNames.Add(new OffsetDescriptor("HeaderBlockOffsets", headerBlockPositionsOffset, 0));
        for (int i = 0; i < numOfLevels; i++)
        {
            blockOffsetsAndNames.Add(new OffsetDescriptor($"TileMap", tileMapsOffset + i * tileMapSize, i));
        }

        for (int i = 0; i < numOfLevels; i++)
        {
            blockOffsetsAndNames.Add(new OffsetDescriptor($"animInfo", animInfosOffset + i * animInfoSize, i));
        }

        for (int i = 0; i < numOfLevels; i++)
        {
            blockOffsetsAndNames.Add(new OffsetDescriptor($"textMaps", textMapsOffset + i * textMapSize, i));
        }


        OffsetDescriptor targetBlock = null;
        // Find which block the input offset is referring to
        foreach (var descriptor in blockOffsetsAndNames)
        {
            if ((offset - descriptor.Offset) > 0) // It's always increasing, so when it's <0, we passed the block
            {
                targetBlock = descriptor;
                continue;
            }

            break;
        }

        if (targetBlock is null)
            throw new ArgumentException("Invalid offset");

        int relativeOffset = offset - targetBlock.Offset;

        // TODO: Make these descriptions a bit better, and calculate where, in each subblock, the byte is (e.g. static weapon 100)
        return $"{targetBlock.BlockName}_byte{relativeOffset}";
    }

    public static LevLoader LoadAndAssertOriginalLevArk(string? path = null)
    {
        path ??= Paths.UW_ArkOriginalPath;
        var AL = new LevLoader(path);
        if (!CheckEqualityOfSha256Hash(AL.Buffer, OriginalLevArkSha256Hash))
        {
            throw new Exception("'Original lev.ark was modified. Please replace it with the original file");
        }

        return AL;
    }
}