using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media;
using AELP.Helper;
using AELP.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AELP.ViewModels;

public partial class SettingsPageViewModel : PageViewModel
{
    private readonly IThemeService _themeService;
    private readonly IKeyboardPreferenceService _keyboardPreferenceService;
    private readonly IPreferenceStorage _preferenceStorage;
    private readonly INotifyService _notifyService;
    private bool _updatingChoiceKeys;

    [ObservableProperty] private ThemeOptionViewModel? _selectedTheme;

    [ObservableProperty] private string _selectedFont;

    [ObservableProperty] private string _choiceKeyMapping;

    [ObservableProperty] private int _notificationDuration;

    public ObservableCollection<ThemeOptionViewModel> ThemeOptions { get; }
    public ObservableCollection<string> AvailableFonts { get; }

    public SettingsPageViewModel(IThemeService themeService, 
        IKeyboardPreferenceService keyboardPreferenceService,
        IPreferenceStorage preferenceStorage,
        INotifyService notifyService)
    {
        PageNames = Data.ApplicationPageNames.Settings;
        _themeService = themeService;
        _keyboardPreferenceService = keyboardPreferenceService;
        _preferenceStorage = preferenceStorage;
        _notifyService = notifyService;
        
        ThemeOptions =
        [
            new ThemeOptionViewModel { Name = "暗色主题", Theme = AppTheme.Dark, Icon = "🌙" },
            new ThemeOptionViewModel { Name = "浅色主题", Theme = AppTheme.Light, Icon = "☀️" },
            new ThemeOptionViewModel { Name = "护眼主题", Theme = AppTheme.EyeCare, Icon = "👁️" }
        ];

        // 设置当前主题
        var currentTheme = _themeService.CurrentTheme;
        _selectedTheme = ThemeOptions.FirstOrDefault(t => t.Theme == currentTheme);

        // 设置字体
        AvailableFonts = new ObservableCollection<string>();
        // Add default font option
        const string defaultFont = "Microsoft YaHei, Segoe UI, Arial";
        AvailableFonts.Add(defaultFont);

        if (FontManager.Current != null)
        {
            var systemFonts = FontManager.Current.SystemFonts.Select(x => x.Name).OrderBy(x => x);
            foreach (var font in systemFonts)
            {
                if (font != defaultFont)
                    AvailableFonts.Add(font);
            }
        }

        _selectedFont = _themeService.CurrentFontFamily;
        if (string.IsNullOrEmpty(_selectedFont))
        {
            _selectedFont = defaultFont;
        }

        _choiceKeyMapping = _keyboardPreferenceService.GetChoiceOptionKeys();
        _notificationDuration = _preferenceStorage.Get("NotificationDuration", 3);
    }

    /// <summary>Executes the logic for when <see cref="P:AELP.ViewModels.SettingsPageViewModel.SelectedTheme">SelectedTheme</see> just changed.</summary>
    /// <param name="value">The new property value that was set.</param>
    /// <remarks>This method is invoked right after the value of <see cref="P:AELP.ViewModels.SettingsPageViewModel.SelectedTheme">SelectedTheme</see> is changed.</remarks>
    partial void OnSelectedThemeChanged(ThemeOptionViewModel? value)
    {
        if (value != null)
        {
            _themeService.SetTheme(value.Theme);
        }
    }
    /// <summary>Executes the logic for when <see cref="P:AELP.ViewModels.SettingsPageViewModel.SelectedFont">SelectedFont</see> just changed.</summary>
    /// <param name="value">The new property value that was set.</param>
    /// <remarks>This method is invoked right after the value of <see cref="P:AELP.ViewModels.SettingsPageViewModel.SelectedFont">SelectedFont</see> is changed.</remarks>
    partial void OnSelectedFontChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _themeService.SetFontFamily(value);
        }
    }
    /// <summary>Executes the logic for when <see cref="P:AELP.ViewModels.SettingsPageViewModel.ChoiceKeyMapping">ChoiceKeyMapping</see> just changed.</summary>
    /// <param name="value">The new property value that was set.</param>
    /// <remarks>This method is invoked right after the value of <see cref="P:AELP.ViewModels.SettingsPageViewModel.ChoiceKeyMapping">ChoiceKeyMapping</see> is changed.</remarks>
    partial void OnChoiceKeyMappingChanged(string value)
    {
        if (_updatingChoiceKeys)
        {
            return;
        }

        _keyboardPreferenceService.SetChoiceOptionKeys(value);
        _updatingChoiceKeys = true;
        ChoiceKeyMapping = _keyboardPreferenceService.GetChoiceOptionKeys();
        _updatingChoiceKeys = false;
    }

    /// <summary>Executes the logic for when <see cref="P:AELP.ViewModels.SettingsPageViewModel.NotificationDuration">NotificationDuration</see> just changed.</summary>
    /// <param name="value">The new property value that was set.</param>
    /// <remarks>This method is invoked right after the value of <see cref="P:AELP.ViewModels.SettingsPageViewModel.NotificationDuration">NotificationDuration</see> is changed.</remarks>
    partial void OnNotificationDurationChanged(int value)
    {
        _preferenceStorage.Set("NotificationDuration", value);
        _notifyService.Notify("通知时长已更新", $"当前通知时长为 {value} 秒");
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
    public AppTheme Theme { get; init; }
}