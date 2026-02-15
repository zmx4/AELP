using System.Collections.Generic;
using System.Linq;

namespace AELP.Services;

/// <summary>
/// 提供选择题按键映射的读取与规范化保存功能。
/// </summary>
public class KeyboardPreferenceService(IPreferenceStorage preferenceStorage) : IKeyboardPreferenceService
{
    private const string ChoiceKeysPreferenceKey = "ChoiceOptionKeys";
    private const string DefaultChoiceKeys = "qwer";

    /// <summary>
    /// 获取当前按键映射。
    /// </summary>
    /// <returns>长度为 4 的按键字符串。</returns>
    public string GetChoiceOptionKeys()
    {
        var stored = preferenceStorage.Get(ChoiceKeysPreferenceKey, DefaultChoiceKeys);
        return NormalizeKeys(stored) ?? DefaultChoiceKeys;
    }

    /// <summary>
    /// 保存按键映射。
    /// </summary>
    /// <param name="keys">按键映射字符串。</param>
    public void SetChoiceOptionKeys(string keys)
    {
        var normalized = NormalizeKeys(keys) ?? DefaultChoiceKeys;
        preferenceStorage.Set(ChoiceKeysPreferenceKey, normalized);
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
