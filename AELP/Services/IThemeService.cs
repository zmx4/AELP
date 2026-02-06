using System;

namespace AELP.Services;

public interface IThemeService
{
    AppTheme CurrentTheme { get; }
    event EventHandler<AppTheme>? ThemeChanged;
    void SetTheme(AppTheme theme);
    AppTheme GetSavedTheme();
}

public enum AppTheme
{
    Dark,
    Light,
    EyeCare
}
