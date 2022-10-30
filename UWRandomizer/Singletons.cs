using System;

namespace UWRandomizer;

public static class Singletons
{
    public static Random RandomInstance;

    static Singletons()
    {
        RandomInstance = new Random();
    }

    public static void SeedRandomAndReset(int seed)
    {
        RandomInstance = new Random(seed);
    }
}