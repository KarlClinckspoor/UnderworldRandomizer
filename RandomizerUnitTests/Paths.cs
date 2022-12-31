using System.Configuration;

namespace RandomizerUnitTests;

public static class Paths
{
    public static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
    public static string BufferTestsPath = config.AppSettings.Settings["BufferTestsPath"].Value;
    public static string BasePath = config.AppSettings.Settings["BasePath"].Value;
    public static string UW_OriginalPath = config.AppSettings.Settings["UWOriginalPath"].Value;
    public static string UW_CleanedPath = config.AppSettings.Settings["UWCleanedPath"].Value;
    public static string UW_ArkOriginalPath = config.AppSettings.Settings["UWArkOriginalPath"].Value;
    public static string UW_ArkCleanedPath = config.AppSettings.Settings["UWArkCleanedPath"].Value;
    public static string RUT_TestDataPath = config.AppSettings.Settings["RUTTestDataPath"].Value;
}