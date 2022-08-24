using System.Text.Json;
using System;

namespace Randomizer;


public static class Settings
{
    public static readonly string DefaultArkPath = "D:\\Dropbox\\UnderworldStudy\\studies\\LEV.ARK";
    public static readonly string DefaultBinaryTestsPath = "D:\\Dropbox\\UnderworldStudy\\studies\\tests";

    static Settings()
    {
        string homepath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string jsonpath = Path.Join(homepath, "UWRandomizerSettings.json");
        try
        {
            string json = File.ReadAllText(jsonpath);
            Stt? settings = JsonSerializer.Deserialize<Stt>(json);
            if (settings is null)
            {
                // DefaultArkPath = "D:\\Dropbox\\UnderworldStudy\\studies\\LEV.ARK"; 
                // DefaultBinaryTestsPath = "D:\\Dropbox\\UnderworldStudy\\studies\\tests";
            }
            else
            {
                DefaultArkPath = settings.DefaultArkPath;
                DefaultBinaryTestsPath = settings.DefaultBinaryTestsPath;
            }
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine("UWRandomizerSettings.json file not found!");
            throw;
        }


    }

}

class Stt
{
    public string DefaultArkPath { get; set; }
    public string DefaultBinaryTestsPath { get; set; }
}