using System;
using System.Collections.Generic;
using System.Linq;

namespace AELP.Services;

public class KeyboardPreferenceService : IKeyboardPreferenceService
{
    private const string ChoiceKeysPreferenceKey = "ChoiceOptionKeys";
    private const string DefaultChoiceKeys = "qwer";
    private readonly IPreferenceStorage _preferenceStorage;

    public KeyboardPreferenceService(IPreferenceStorage preferenceStorage)
    {
        _preferenceStorage = preferenceStorage;
    }

    public string GetChoiceOptionKeys()
    {
        var stored = _preferenceStorage.Get(ChoiceKeysPreferenceKey, DefaultChoiceKeys);
        return NormalizeKeys(stored) ?? DefaultChoiceKeys;
    }

    public void SetChoiceOptionKeys(string keys)
    {
        var normalized = NormalizeKeys(keys) ?? DefaultChoiceKeys;
        _preferenceStorage.Set(ChoiceKeysPreferenceKey, normalized);
    }

    private static string? NormalizeKeys(string keys)
    {
        if (string.IsNullOrWhiteSpace(keys))
        {
            return null;
        }

        var letters = new List<char>();
        foreach (var ch in keys.Trim())
        {
            if (char.IsLetter(ch))
            {
                letters.Add(char.ToLowerInvariant(ch));
            }
        }

        if (letters.Count != 4)
        {
            return null;
        }

        if (letters.Distinct().Count() != 4)
        {
            return null;
        }

        return new string(letters.ToArray());
    }
}
