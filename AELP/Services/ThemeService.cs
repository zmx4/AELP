using System;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace AELP.Services;

/// <summary>
/// 主题与字体服务实现，负责应用样式应用与偏好持久化。
/// </summary>
public class ThemeService : IThemeService
{
    private const string ThemePreferenceKey = "AppTheme";
    private const string FontPreferenceKey = "UserFont";
    private readonly IPreferenceStorage _preferenceStorage;
    private AppTheme _currentTheme;
    private string _currentFontFamily;

    /// <inheritdoc />
    public event EventHandler<AppTheme>? ThemeChanged;

    /// <inheritdoc />
    public event EventHandler<string>? FontFamilyChanged;

    /// <inheritdoc />
    public AppTheme CurrentTheme => _currentTheme;

    /// <inheritdoc />
    public string CurrentFontFamily => _currentFontFamily;

    /// <summary>
    /// 初始化 <see cref="ThemeService"/>。
    /// </summary>
    /// <param name="preferenceStorage">偏好设置存储。</param>
    public ThemeService(IPreferenceStorage preferenceStorage)
    {
        _preferenceStorage = preferenceStorage;
        _currentTheme = GetSavedTheme();
        _currentFontFamily = GetSavedFontFamily();
    }

    /// <inheritdoc />
    public void SetTheme(AppTheme theme)
    {
        if (_currentTheme == theme) return;

        _currentTheme = theme;
        _preferenceStorage.Set(ThemePreferenceKey, ((int)theme).ToString());
        ApplyTheme(theme);
        ThemeChanged?.Invoke(this, theme);
    }

    /// <inheritdoc />
    public void SetFontFamily(string fontFamily)
    {
        if (_currentFontFamily == fontFamily) return;

        _currentFontFamily = fontFamily;
        _preferenceStorage.Set(FontPreferenceKey, fontFamily);
        ApplyFont(fontFamily);
        FontFamilyChanged?.Invoke(this, fontFamily);
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public string GetSavedFontFamily()
    {
        return _preferenceStorage.Get(FontPreferenceKey, "Microsoft YaHei, Segoe UI, Arial");
    }

    private void ApplyFont(string fontFamily)
    {
        var app = Application.Current;
        if (app == null) return;
        
        try
        {
            app.Resources["DefaultFontFamily"] = new FontFamily(fontFamily);
        }
        catch
        {
            // Fallback or ignore invalid fonts
        }
    }

    private void ApplyTheme(AppTheme theme)
    {
        var app = Application.Current;
        if (app == null) return;

        // 设置 SemiTheme 的主题变体（Dark/Light/EyeCare）
        var themeVariant = theme switch
        {
            AppTheme.Light => ThemeVariant.Light,
            AppTheme.EyeCare => ThemeVariant.Light, // 护眼模式基于亮色主题
            _ => ThemeVariant.Dark
        };
        app.RequestedThemeVariant = themeVariant;
        
        // 更新LiveCharts的主题
        LiveCharts.Configure(config =>
        {
            switch (theme)
            {
                case AppTheme.Light:
                case AppTheme.EyeCare:
                    config.AddLightTheme();
                    break;
                default:
                    config.AddDarkTheme();
                    break;
            }
        });
        
        // 清除现有的护眼主题样式覆盖
        var existingThemeStyle = app.Styles.FirstOrDefault(s => 
            s is StyleInclude si && 
            si.Source?.ToString().Contains("EyeCareTheme") == true);
        
        if (existingThemeStyle != null)
        {
            app.Styles.Remove(existingThemeStyle);
        }

        // 护眼模式需要额外加载样式覆盖（在亮色主题基础上替换为绿色调）
        if (theme == AppTheme.EyeCare)
        {
            var themeStyle = new StyleInclude(new Uri("avares://AELP"))
            {
                Source = new Uri("avares://AELP/Styles/EyeCareTheme.axaml")
            };
            app.Styles.Add(themeStyle);
        }
    }
}
