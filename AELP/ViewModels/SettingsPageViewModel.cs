using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;
using AELP.Helper;
using AELP.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AELP.ViewModels;

public partial class SettingsPageViewModel : PageViewModel
{
    private readonly IThemeService _themeService;
    
    [ObservableProperty]
    private ThemeOptionViewModel? _selectedTheme;
    
    public ObservableCollection<ThemeOptionViewModel> ThemeOptions { get; }

    public SettingsPageViewModel(IThemeService themeService)
    {
        PageNames = Data.ApplicationPageNames.Settings;
        _themeService = themeService;
        
        ThemeOptions = new ObservableCollection<ThemeOptionViewModel>
        {
            new ThemeOptionViewModel { Name = "暗色主题", Theme = AppTheme.Dark, Icon = "🌙" },
            new ThemeOptionViewModel { Name = "浅色主题", Theme = AppTheme.Light, Icon = "☀️" },
            new ThemeOptionViewModel { Name = "护眼主题", Theme = AppTheme.EyeCare, Icon = "👁️" }
        };
        
        // 设置当前主题
        var currentTheme = _themeService.CurrentTheme;
        _selectedTheme = ThemeOptions.FirstOrDefault(t => t.Theme == currentTheme);
    }
    
    partial void OnSelectedThemeChanged(ThemeOptionViewModel? value)
    {
        if (value != null)
        {
            _themeService.SetTheme(value.Theme);
        }
    }
    
    [RelayCommand]
    private static void OpenUserDataFolder()
    {
        var userDataPath = PathHelper.GetLocalFilePath("");
        System.Diagnostics.Process.Start("explorer.exe", userDataPath);
    }

    [RelayCommand]
    private static void OpenAppFolder()
    {
        var appPath = PathHelper.GetAppFilePath("");
        System.Diagnostics.Process.Start("explorer.exe", appPath);
    }
    [RelayCommand]
    private static async Task DeleteUserDataAsync()
    {
        var userDataPath = PathHelper.GetLocalFilePath("userdata.sqlite");
        if (System.IO.File.Exists(userDataPath))
        {
            System.IO.File.Delete(userDataPath);
        }
        
        await Task.CompletedTask;
    }
}
public class ThemeOptionViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public AppTheme Theme { get; set; }
}
