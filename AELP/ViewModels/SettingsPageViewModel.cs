using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media;
using AELP.Helper;
using AELP.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AELP.ViewModels;

/// <summary>
/// 设置页面视图模型，负责主题、字体、按键与通知配置。
/// </summary>
public partial class SettingsPageViewModel : PageViewModel
{
    private readonly IThemeService _themeService;
    private readonly IKeyboardPreferenceService _keyboardPreferenceService;
    private readonly INotifyService _notifyService;
    private bool _updatingChoiceKeys;

    [ObservableProperty] private ThemeOptionViewModel? _selectedTheme;

    [ObservableProperty] private string _selectedFont;

    [ObservableProperty] private string _choiceKeyMapping;

    [ObservableProperty] private int _notificationDuration;

    public ObservableCollection<ThemeOptionViewModel> ThemeOptions { get; }
    public ObservableCollection<string> AvailableFonts { get; }

    /// <summary>
    /// 初始化 <see cref="SettingsPageViewModel"/>。
    /// </summary>
    /// <param name="themeService">主题服务。</param>
    /// <param name="keyboardPreferenceService">按键偏好服务。</param>
    /// <param name="preferenceStorage">偏好存储服务。</param>
    /// <param name="notifyService">通知服务。</param>
    public SettingsPageViewModel(IThemeService themeService, 
        IKeyboardPreferenceService keyboardPreferenceService,
        IPreferenceStorage preferenceStorage,
        INotifyService notifyService)
    {
        PageNames = Data.ApplicationPageNames.Settings;
        _themeService = themeService;
        _keyboardPreferenceService = keyboardPreferenceService;
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
        _notificationDuration = preferenceStorage.Get("NotificationDuration", 3);
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
        _notifyService.SetNotificationDuration(value);
        _notifyService.Notify("通知时长已更新", $"当前通知时长为 {value} 秒");
    }

    /// <summary>
    /// 打开用户数据目录。
    /// </summary>
    [RelayCommand]
    private static void OpenUserDataFolder()
    {
        var userDataPath = PathHelper.GetLocalFilePath("");
        System.Diagnostics.Process.Start("explorer.exe", userDataPath);
    }

    /// <summary>
    /// 打开应用目录。
    /// </summary>
    [RelayCommand]
    private static void OpenAppFolder()
    {
        var appPath = PathHelper.GetAppFilePath("");
        System.Diagnostics.Process.Start("explorer.exe", appPath);
    }

    /// <summary>
    /// 删除用户数据文件。
    /// </summary>
    /// <returns>表示删除流程完成的异步任务。</returns>
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

/// <summary>
/// 主题选项展示模型。
/// </summary>
public class ThemeOptionViewModel
{
    /// <summary>
    /// 主题显示名称。
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 主题图标。
    /// </summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// 对应主题值。
    /// </summary>
    public AppTheme Theme { get; init; }
}