namespace Randomizer
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
        /// Note: Can't have this function with type short inputs because +, &lt&lt, etc, are all defined for ints only...
        ///
        /// Example:
        /// 
        ///     currvalue: 1100_1110  // What we current have
        ///     expected:  1101_0110  // Out objective
        ///     newvalue:  1001_0000  // A set of bits we want to set in currvalue. Note the first bit is ignored
        ///     mask:      0011_1000  // The bits we want to modify are 1, preserve are 0
        ///     shift:     0          // If desired, newvalue and mask are left shifted
        ///     
        /// Step 0: Moves the mask to the desired position. No effect in this example
        /// 
        /// Step 1: Sets the values to be modified to 0. Preserves the rest
        ///     currvalue & (~(mask &lt&lt shift))
        ///     1100_1110 &
        ///     1100_0111
        ///     ---------
        ///     1100_0110 // Set "to change" bits to 0
        ///     
        /// Step 2: Sets the bits in newvalue that will be copied over.
        ///     newvalue & mask:
        ///     1001_0000 &
        ///     0011_1000
        ///     ---------
        ///     0001_0000
        /// 
        /// Step 3: Moves over values to be copied to their intended position.
        ///     
        /// Step 4: ORs the results from Step 1 and Step 2:
        ///     (Step 1) | (Step 2)
        ///     1100_0110 |
        ///     0001_0000
        ///     ---------
        ///     1101_0110  // == expected
        ///     
        /// Check the UnitTests for more examples
        /// </summary>
        /// <param name="currvalue">Current value of the variable where bits will be set</param>
        /// <param name="newvalue">Variable with a set of bits to change </param>
        /// <param name="mask">A bitmask that selects which bits from currvalue and newvalue will be considered</param>
        /// <param name="shift">A number of leftshift operations on the mask. Note this affects the newvalue before applying the mask</param>
        /// <returns></returns>
        public static int SetBits(int currvalue, int newvalue, int mask, int shift)
        {
            return (
                      (currvalue & (~(mask << shift))) // Sets the bits to be changed to 0, preserving others
                      | ((newvalue & mask) << shift) // Moves only the relevant bits of the new value to the relevant position, replaces the target bits
                   );  
        }

        /// <summary>
        /// Gets the bits specified by the mask after shifting "value" to the right by "shift".
        /// 
        /// Example:
        /// 
        ///     value = 0b1110_0110
        ///     mask  = 0b11
        ///     shift = 2
        ///     expected = 0b01
        /// 
        /// Step 1: value &gt&gt shift
        ///     
        ///     0b1110_0110 &gt&gt 2 = 0b0011_1001
        ///       ---- --
        ///         -- ----
        ///
        /// Step 2: AND mask
        /// 
        ///     0b0011_1001
        ///     0b0000_0011
        ///       ---------
        ///     0b000000001 == 0b01
        /// 
        /// </summary>
        /// <param name="value">Int containing the bits to get</param>
        /// <param name="mask">A bitmask containing the number of bits and the positions that will be extracted</param>
        /// <param name="shift">Number of places the "value" will be shifted to the right</param>
        /// <returns></returns>
        public static int GetBits(int value, int mask, int shift)
        {
            return (value >> shift) & mask;
        }

        public static bool StdSaveBuffer(byte[] buffer, string basepath, string extrainfo)
        {

        string fullpath = @$"{basepath}\{extrainfo}";
        
        try
        {
            File.WriteAllBytes(fullpath, buffer);
        }
        catch (Exception e) // TODO: Specify exception.
        {
            Console.WriteLine($"Unable to write. Error: {e}");
            return false;
        }
        return true;

        }

    }
}