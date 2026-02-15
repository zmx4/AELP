using System;

namespace AELP.Services;

/// <summary>
/// 提供主题和字体设置的管理能力。
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// 获取当前主题。
    /// </summary>
    AppTheme CurrentTheme { get; }

    /// <summary>
    /// 获取当前字体族。
    /// </summary>
    string CurrentFontFamily { get; }

    /// <summary>
    /// 当主题发生变化时触发。
    /// </summary>
    event EventHandler<AppTheme>? ThemeChanged;

    /// <summary>
    /// 当字体族发生变化时触发。
    /// </summary>
    event EventHandler<string>? FontFamilyChanged;

    /// <summary>
    /// 设置应用主题。
    /// </summary>
    /// <param name="theme">目标主题。</param>
    void SetTheme(AppTheme theme);

    /// <summary>
    /// 设置应用字体族。
    /// </summary>
    /// <param name="fontFamily">目标字体族。</param>
    void SetFontFamily(string fontFamily);

    /// <summary>
    /// 获取已保存的主题设置。
    /// </summary>
    /// <returns>持久化保存的主题值。</returns>
    AppTheme GetSavedTheme();

    /// <summary>
    /// 获取已保存的字体族设置。
    /// </summary>
    /// <returns>持久化保存的字体族值。</returns>
    string GetSavedFontFamily();
}

/// <summary>
/// 应用主题枚举。
/// </summary>
public enum AppTheme
{
    /// <summary>
    /// 暗色主题。
    /// </summary>
    Dark,

    /// <summary>
    /// 浅色主题。
    /// </summary>
    Light,

    /// <summary>
    /// 护眼主题。
    /// </summary>
    EyeCare
}
