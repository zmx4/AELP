using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading;
using AELP.Helper;

namespace AELP.Services;

public class JsonPreferenceStorage : IPreferenceStorage
{
    private const string PreferencesFileName = "preferences.json";
    private static readonly Lock FileLock = new();

    public string Get(string key, string defaultValue)
    {
        lock (FileLock)
        {
            var preferences = LoadPreferences();
            return preferences.TryGetValue(key, out var value) && value is not null
                ? value
                : defaultValue;
        }
    }

    public void Set(string key, DateTime value)
    {
        Set(key, value.ToString("O", CultureInfo.InvariantCulture));
    }
    
    public void Set(string key, int value)
    {
        Set(key, value.ToString(CultureInfo.InvariantCulture));
    }

    public int Get(string key, int defaultValue)
    {
        lock (FileLock)
        {
            var preferences = LoadPreferences();
            return preferences.TryGetValue(key, out var value)
                   && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
                ? parsed
                : defaultValue;
        }
    }

    public void Set(string key, string value)
    {
        lock (FileLock)
        {
            var preferences = LoadPreferences();
            preferences[key] = value;
            SavePreferences(preferences);
        }
    }


    public DateTime Get(string key, DateTime defaultValue)
    {
        lock (FileLock)
        {
            var preferences = LoadPreferences();
            if (preferences.TryGetValue(key, out var value)
                && DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed))
            {
                return parsed;
            }

            return defaultValue;
        }
    }

    private static string PreferenceFilePath => PathHelper.GetLocalFilePath(PreferencesFileName);

    private static Dictionary<string, string> LoadPreferences()
    {
        if (!File.Exists(PreferenceFilePath))
        {
            return new Dictionary<string, string>(StringComparer.Ordinal);
        }

        try
        {
            var json = File.ReadAllText(PreferenceFilePath);
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            return data ?? new Dictionary<string, string>(StringComparer.Ordinal);
        }
        catch
        {
            return new Dictionary<string, string>(StringComparer.Ordinal);
        }
    }

    private static void SavePreferences(Dictionary<string, string> preferences)
    {
        var json = JsonSerializer.Serialize(preferences, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(PreferenceFilePath, json);
    }
}