using UWRandomizerEditor.Interfaces;

namespace UWRandomizerEditor
{
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
        
        public static uint GetBits(uint value, int mask, int shift)
        {
            return (uint) GetBits((int) value, mask, shift);
        }

        /// <summary>
        /// Saves a specific buffer to a path specified as basepath \ filename.
        /// </summary>
        /// <param name="obj">Object that has a "Buffer" property</param>
        /// <param name="basepath">Base path to the file (e.g. folder structure). If not provided, uses the current working directory </param>
        /// <param name="filename">Extra info to add (e.g. name of file). If not provided, gets a random Guid </param>
        /// <returns>Path to the saved object. If couldn't be saved, returns null</returns>
        public static string StdSaveBuffer(IBufferObject obj, string? basepath, string? filename)
        {
            return StdSaveBuffer(obj.Buffer, basepath, filename);
        }

        /// <summary>
        /// Saves a specific buffer to a path specified as basepath \ filename.
        /// </summary>
        /// <param name="buffer">byte array</param>
        /// <param name="basepath">Base path to the file (e.g. folder structure). If not provided, uses the current working directory </param>
        /// <param name="filename">Extra info to add (e.g. name of file). If not provided, gets a random Guid </param>
        /// <returns>Path to the saved object. If couldn't be saved, returns null</returns>
        public static string StdSaveBuffer(byte[] buffer, string? basepath, string? filename)
        {
            basepath ??= ".";
            filename ??= Guid.NewGuid().ToString();

            string fullpath = Path.Join(basepath, filename);

            File.WriteAllBytes(fullpath, buffer);
            return fullpath;
        }
    }
}