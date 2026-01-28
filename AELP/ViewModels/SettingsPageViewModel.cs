using System.Threading.Tasks;
using AELP.Helper;
using CommunityToolkit.Mvvm.Input;

namespace AELP.ViewModels;

public partial class SettingsPageViewModel : PageViewModel
{
    public SettingsPageViewModel()
    {
        PageNames = Data.ApplicationPageNames.Settings;
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