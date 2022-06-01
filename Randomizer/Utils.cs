namespace Randomizer
{
    /// <summary>
    /// Class containing useful small functions to set bits in values, save buffers, etc.
    /// </summary>
    public static class Utils
    {
        // TODO: Don't I have to shift the mask also, in the first step? Seems I'm missing the logic here.
        /// <summary>
        /// Sets specific bits of "newvalue" into "currvalue" obeying a mask and a shift.
        /// Mask acts as the number of bits to be set, shift is its position.
        ///
        /// Note: Can't have this function with type short inputs because +, <<, etc, are all defined for ints only...
        ///
        /// Example: Want to modify currvalue (1100 1110) to (1100 0110) by setting the last 4 bits.
        /// 
        ///     currvalue: 1100 1110
        ///     newvalue:  0011 0110
        ///     mask:      0000 1111
        ///     shift:     0
        ///     
        /// 
        ///     
        /// Step 1: Sets the values to be modified to 0. Preserves the rest
        ///     currvalue & (~mask):
        ///     1100 1110 &
        ///     1111 0000
        ///     ---------
        ///     1100 0000
        ///     
        /// Step 2: Sets the bits in newvalue that will be copied over.
        ///     newvalue & mask:
        ///     0011 0110 &
        ///     0000 1111
        ///     ---------
        ///     0000 0110
        /// 
        /// Step 3: Moves over values to be copied to their intended position.
        ///     (newvalue & mask) << shift
        ///     0000 0110 << 0
        ///     --------------
        ///     0000 0110
        ///     
        /// Step 4: ORs the new bits in the current value
        ///     (currvalue & (~mask)) | ((newvalue & mask) << shift)
        ///     1100 0000 |
        ///     0000 0110
        ///     ---------
        ///     1100 0110
        ///     
        /// </summary>
        /// <param name="currvalue"></param>
        /// <param name="newvalue"></param>
        /// <param name="mask"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public static int SetBits(int currvalue, int newvalue, int mask, int shift)
        {
            return (
                      (currvalue & (~mask)) // Sets the bits to be changed to 0, preserving others
                      | ((newvalue & mask) << shift) // Moves only the relevant bits of the new value to the relevant position, replaces the target bits
                   );  
        }

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