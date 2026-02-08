using System;

namespace AELP.Services;

public interface IThemeService
{
    AppTheme CurrentTheme { get; }
    string CurrentFontFamily { get; }
    event EventHandler<AppTheme>? ThemeChanged;
    event EventHandler<string>? FontFamilyChanged;
    void SetTheme(AppTheme theme);
    void SetFontFamily(string fontFamily);
    AppTheme GetSavedTheme();
    string GetSavedFontFamily();
}

public enum AppTheme
{
    Dark,
    Light,
    EyeCare
}
