using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class FileManager
{
    public static string[] GetAllFiles(string folder, string file)
    {
        var d = new DirectoryInfo(folder);
        var fileInfo = d.GetFiles(file);
        var files = new string[fileInfo.Length];

        for (var i = 0; i < fileInfo.Length; i++)
        {
            files[i] = fileInfo[i].Name;
        }

        return files;
    }

    public static void SetIntPlayerPref(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    public static int GetIntPlayerPref(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public static void DeletePlayerPref(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }

    public static void SaveJsonFile(string path, string fileName, object saveData)
    {
        string fullPath = Path.Combine(path, $"{fileName}.json");

        if (!IsDirectoryExist(path))
        {
            Directory.CreateDirectory(path);
        }

        string jsonData = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        File.WriteAllText(fullPath, jsonData);

        Log.Logging($"Save file: {fullPath}");
    }

    public static T LoadJsonFile<T>(string path)
    {
        if (!IsFileExists(path))
        {
            return default(T);
        }

        string jsonData = File.ReadAllText(path);
        var loadData = JsonConvert.DeserializeObject<T>(jsonData);
        return loadData;
    }

    public static string GetText(string filePath)
    {
        if (!IsFileExists(filePath))
        {
            return null;
        }

        string file = File.ReadAllText(filePath);
        return file;
    }

    public static string[] GetAllFileInDirectory(string path)
    {
        if (!IsDirectoryExist(path))
        {
            Directory.CreateDirectory(path);
        }

        string[] files = Directory.GetFiles(path);
        return files;
    }

    public static void RemoveFile(string path)
    {
        if (!IsFileExists(path))
        {
            return;
        }

        File.Delete(path);
    }

    public static bool IsFileExists(string path)
    {
        if (File.Exists(path))
        {
            return true;
        }

        return false;
    }

    public static bool IsDirectoryExist(string path)
    {
        if (Directory.Exists(path))
        {
            return true;
        }

        return false;
    }
}