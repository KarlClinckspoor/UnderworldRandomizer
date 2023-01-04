using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

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
}