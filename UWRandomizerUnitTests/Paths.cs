using System.Configuration;

namespace RandomizerUnitTests;

public static class Paths
{
    // The current working folder of these test suites UWRandomizerUnitTests/bin/Debug(OrRelease/net7.0/)
    // This config object can be used to conveniently find the expected name of the Unit test dll config
    // (e.g. testhost.dll.config or ReSharperTestRunner.dll.config)
    // But considering the way I've structured this file right now, those config files are unnecessary
    public static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
    public static string BufferTestsPath = "./";
    // In this folder you should place the files that will be used for testing
    // (Lev.ark, CMB.DAT, etc.)
    // Don't forget to change their properties to always copy
    public static string TestDataPath = "./testdata/";
    public static string UW1_ArkOriginalPath = TestDataPath + "UW1Original_LEV.ARK";
    public static string UW1_ArkCleanedPath = TestDataPath + "UW1Cleaned_LEV.ARK";
    public static string UW1_CMBPath = TestDataPath + "UW1Original_CMB.DAT";
    public static string UW1_ArkDifficultPath = TestDataPath + "UW1Difficult_LEV.ARK";
    public static string TestDataOutput = "./testdataoutput/";
}