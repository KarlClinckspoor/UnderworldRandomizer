﻿using UWRandomizerEditor.Interfaces;

namespace UWRandomizerEditor;

/// <summary>
/// Class containing useful small functions to set bits in values, save buffers, etc.
/// </summary>
public static class Utils
{
    /// <summary>
    /// Sets specific bits of "newvalue" into "currvalue" obeying a mask and a shift.
    /// Mask acts as the number of bits to be set, shift is its position.
    ///
    /// Taken from https://stackoverflow.com/questions/5925755/how-to-replace-bits-in-a-bitfield-without-affecting-other-bits-using-c
    ///
    /// Note: Can't have this function with type short inputs because +, &lt;&lt;, etc, are all defined for ints only...
    ///
    /// <example>
    ///
    /// <list type="bullet">
    /// <item> <term> currvalue </term> <description> <c>1100_1110</c> What we currently have </description>  </item>
    /// <item> <term> expected: </term> <description> <c>1101_0110</c> Out objective                                                           </description> </item>
    /// <item> <term> newvalue: </term> <description> <c>1001_0000</c> A set of bits we want to set in currvalue. Note the first bit will be ignored</description> </item>
    /// <item> <term> mask:     </term> <description> <c>0011_1000</c> The bits we want to modify are 1, preserve are 0                        </description> </item>
    /// <item> <term> shift:    </term> <description> <c>0        </c> If desired, newvalue and mask are left shifted                          </description> </item>
    /// </list>
    ///
    /// <para> Step 0: Moves the mask to the desired position. No effect in this example </para>
    ///
    /// <para> Step 1: Sets the values to be modified to 0. Preserves the rest </para>
    /// 
    /// <code>
    ///     currvalue &amp; (~(mask &lt;&lt; shift))
    ///     1100_1110 &amp;
    ///     1100_0111
    ///     ---------
    ///     1100_0110 // Set "to change" bits to 0
    /// </code>
    ///
    /// <para> Step 2: Sets the bits in newvalue that will be copied over. </para>
    /// 
    /// <code>
    ///     newvalue &amp; mask:
    ///     1001_0000 &amp;
    ///     0011_1000
    ///     ---------
    ///     0001_0000
    /// </code>
    /// 
    /// <para> Step 3: Moves over values to be copied to their intended position. </para>
    ///
    /// <code>
    ///     (Step 2) &lt;&lt; shift
    ///     0001_0000 &amp; 0
    ///     ---------
    ///     0001_0000
    /// </code>
    ///     
    /// <para> Step 4: ORs the results from Step 1 and Step 3: </para>
    /// <code>
    ///     (Step 1)  | (Step 3)
    ///     1100_0110 |
    ///     0001_0000
    ///     ---------
    ///     1101_0110  // == expected
    /// </code>
    /// 
    /// Check the UnitTests for more examples
    /// </example>
    /// 
    /// </summary>
    /// <param name="currvalue">Current value of the variable where bits will be set</param>
    /// <param name="newvalue">Variable with a set of bits to change </param>
    /// <param name="mask">A bitmask that selects which bits from currvalue and newvalue will be considered</param>
    /// <param name="shift">A number of leftshift operations on the mask. Note this affects the newvalue before applying the mask</param>
    /// <returns>Returns "currvalue" modified by "newvalue" using "mask" with specific left bitshift</returns>
    public static int SetBits(int currvalue, int newvalue, int mask, int shift)
    {
        return (
            (currvalue & (~(mask << shift))) // Sets the bits to be changed to 0, preserving others
            | ((newvalue & mask) <<
               shift) // Moves only the relevant bits of the new value to the relevant position, replaces the target bits
        );
    }
        
    /// <inheritdoc cref="SetBits(int,int,int,int)"/>
    public static uint SetBits(uint currvalue, uint newvalue, int mask, int shift)
    {
        return (uint) SetBits((int) currvalue, (int) newvalue, mask, shift);
    }

    /// <summary>
    /// Gets the bits specified by the mask after shifting "value" to the right by "shift".
    /// 
    /// <example>
    ///
    /// <list type="bullet">
    ///   <item> <term>  value </term> <description> <c>0b1110_0110</c>   </description> </item>
    ///   <item> <term>  mask  </term> <description> <c>0b11       </c>   </description> </item>
    ///   <item> <term>  shift </term> <description> <c>2          </c>   </description> </item>
    ///   <item> <term>  expected </term> <description> <c>0b01    </c>   </description> </item>
    ///</list> 
    /// <para> Step 1: value &gt;&gt; shift </para>
    ///
    /// <code>
    ///     0b1110_0110 &gt;&gt; 2 = 0b0011_1001
    ///       ---- --   &gt;&gt; 2
    ///         -- ----
    /// </code>
    ///
    /// <para> Step 2: AND mask </para>
    ///
    /// <code>
    ///     0b0011_1001
    ///     0b0000_0011
    ///       ---------
    ///     0b000000001 == 0b01
    /// </code>
    /// 
    ///</example>
    /// 
    /// </summary>
    /// <param name="value">Int containing the bits to get</param>
    /// <param name="mask">A bitmask containing the number of bits and the positions that will be extracted</param>
    /// <param name="shift">Number of places the "value" will be shifted to the right</param>
    /// <returns>Returns specific bits in "value" specified by "mask" after shifting "value" to the right by "shift"</returns>
    public static int GetBits(int value, int mask, int shift)
    {
        return (value >> shift) & mask;
    }
        
    /// <inheritdoc cref="GetBits(int,int,int)"/>
    public static uint GetBits(uint value, int mask, int shift)
    {
        return (uint) GetBits((int) value, mask, shift);
    }

    /// <summary>
    /// Saves a specific buffer to a path specified as baseDirectory \ filename.
    /// </summary>
    /// <param name="obj">Object that has a "Buffer" property</param>
    /// <param name="baseDirectory">Base path to the file (e.g. folder structure). If not provided, uses the current working directory </param>
    /// <param name="filename">Extra info to add (e.g. name of file). If not provided, gets a random Guid </param>
    /// <returns>Path to the saved object.</returns>
    public static string SaveBuffer(IBufferObject obj, string? baseDirectory, string? filename)
    {
        return SaveBuffer(obj.Buffer, baseDirectory, filename);
    }

    /// <summary>
    /// Saves a specific buffer to a path specified as baseDirectory \ filename.
    /// </summary>
    /// <param name="buffer">byte array</param>
    /// <param name="baseDirectory">Base path to the file (e.g. folder structure). If not provided, uses the current working directory </param>
    /// <param name="filename">Extra info to add (e.g. name of file). If not provided, gets a random Guid </param>
    /// <returns>Path to the saved object.</returns>
    public static string SaveBuffer(byte[] buffer, string? baseDirectory, string? filename)
    {
        baseDirectory ??= ".";
        filename ??= Guid.NewGuid().ToString();

        var fullPath = Path.Join(baseDirectory, filename);

        File.WriteAllBytes(fullPath, buffer);
        return fullPath;
    }
    

    /// <summary>
    /// Reshapes a flat array into a 2D rectangular array.
    /// </summary>
    /// <param name="flatArray">Array to be reshaped</param>
    /// <param name="width">Number of columns in the output array</param>
    /// <param name="height">Number of rows in the output array</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>A new array of shape width x height</returns>
    /// <exception cref="InvalidDataException">Thrown if width * height != flatArray.Length</exception>
    public static T[,] ReshapeArray<T>(T[] flatArray, int width, int height)
    {
        var totalItems = width * height;
        if (totalItems != flatArray.Length)
        {
            throw new InvalidDataException(
                $"The number of items in the output array {totalItems} is different from dimensions of the input array {flatArray.Length}");
        }

        var outArray = new T[height, width];
        for (var i = 0; i < totalItems; i++)
        {
            var col = i % width;
            var row = i / width;
            outArray[row, col] = flatArray[i];
        }

        return outArray;
    }
    
    /// <summary>
    /// If a 2D array is reshaped to a 1 row, n column array, this drops the extra dimension returning a 1D n-length array.
    /// </summary>
    /// <param name="twoDArray">Array that contains the data. Must have only 1 row</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>1D output array</returns>
    /// <exception cref="ArgumentException">thrown if the array has more than 1 row</exception>
    public static T[] DropDimension<T>(T[,] twoDArray) 
    {
        if (twoDArray.GetLength(0) > 1)
        {
            throw new ArgumentException("This only removes one dimension from the 2D array, it can't have more than 2 rows.");
        }

        return Enumerable.Range(0, twoDArray.GetLength(1)).Select(x => twoDArray[0, x]).ToArray();
    }
    
    
}