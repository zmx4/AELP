using System;
using System.Linq;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;

namespace AELP.Services;

public class ThemeService : IThemeService
{
    private const string ThemePreferenceKey = "AppTheme";
    private readonly IPreferenceStorage _preferenceStorage;
    private AppTheme _currentTheme;

    public event EventHandler<AppTheme>? ThemeChanged;

    public AppTheme CurrentTheme => _currentTheme;

    public ThemeService(IPreferenceStorage preferenceStorage)
    {
        _preferenceStorage = preferenceStorage;
        _currentTheme = GetSavedTheme();
    }

    public void SetTheme(AppTheme theme)
    {
        if (_currentTheme == theme) return;

        _currentTheme = theme;
        _preferenceStorage.Set(ThemePreferenceKey, ((int)theme).ToString());
        ApplyTheme(theme);
        ThemeChanged?.Invoke(this, theme);
    }

    public AppTheme GetSavedTheme()
    {
        var themeValue = _preferenceStorage.Get(ThemePreferenceKey, "0");
        if (int.TryParse(themeValue, out var themeInt) && 
            Enum.IsDefined(typeof(AppTheme), themeInt))
        {
            return (AppTheme)themeInt;
        }
        return AppTheme.Dark;
    }

    private void ApplyTheme(AppTheme theme)
    {
        var app = Application.Current;
        if (app == null) return;

        // 更新 FluentTheme 的主题变体
        var themeVariant = theme switch
        {
            AppTheme.Light => ThemeVariant.Light,
            _ => ThemeVariant.Dark
        };
        app.RequestedThemeVariant = themeVariant;

        // 清除现有的主题样式
        var existingThemeStyle = app.Styles.FirstOrDefault(s => 
            s is StyleInclude si && 
            (si.Source?.ToString().Contains("EyeCareTheme") == true ||
             si.Source?.ToString().Contains("DarkTheme") == true ||
             si.Source?.ToString().Contains("LightTheme") == true));
        
        if (existingThemeStyle != null)
        {
            app.Styles.Remove(existingThemeStyle);
        }

        // 根据主题添加对应的样式
        string? themeStylePath = theme switch
        {
            AppTheme.EyeCare => "avares://AELP/Styles/EyeCareTheme.axaml",
            AppTheme.Light => "avares://AELP/Styles/LightTheme.axaml",
            _ => null // Dark theme is the default
        };

        if (themeStylePath != null)
        {
            var themeStyle = new StyleInclude(new Uri("avares://AELP"))
            {
                Source = new Uri(themeStylePath)
            };
            app.Styles.Add(themeStyle);
        }
    }
}
