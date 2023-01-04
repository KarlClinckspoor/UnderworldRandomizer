using System;
using System.Collections.Generic;

namespace RandomizerUnitTests;

public class Utils
{
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
}