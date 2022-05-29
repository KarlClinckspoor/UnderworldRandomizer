namespace Randomizer
{
    public static class Utils
    {
        // Note: Can't have these as short because +, <<, etc, are all defined for ints only...
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