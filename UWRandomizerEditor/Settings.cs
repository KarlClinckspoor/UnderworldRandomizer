﻿using System;
using System.IO;
using System.Text.Json;

namespace UWRandomizerEditor;


public static class Settings
{
    public static readonly string DefaultArkPath = @"C:\Users\Karl\Desktop\UnderworldStudy\UW\DATA\LEV.ARK";
    public static readonly string DefaultBinaryTestsPath = @"C:\Users\Karl\Desktop\UnderworldStudy\Buffers";

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
            #if !DEBUG
                Console.WriteLine("UWRandomizerSettings.json file not found!");
                throw;
            #else
                Console.WriteLine("UWRandomizerSettings.json file not found! If you're testing, ignore.");
            #endif
        }


    }

}

class Stt
{
    public string DefaultArkPath { get; set; }
    public string DefaultBinaryTestsPath { get; set; }
}