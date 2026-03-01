using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using System.Threading.Tasks;
using AELP.Data;
using AELP.Helper;
using Avalonia.Markup.Xaml;
using AELP.ViewModels;
using AELP.Views;
using Microsoft.Extensions.DependencyInjection;
using AELP.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace AELP;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        LiveCharts.Configure(config =>
            config
                .AddSkiaSharp()
                .AddDefaultMappers()
                .AddDarkTheme()
        );
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        collection.AddAelpDbContexts();
        collection.AddAelpServices();
        collection.AddAelpViewModels();
        collection.AddAelpFactories();

        var serviceProvider = collection.BuildServiceProvider();

        // 初始化主题
        var themeService = serviceProvider.GetRequiredService<IThemeService>();
        var savedTheme = themeService.GetSavedTheme();
        themeService.SetTheme(savedTheme);

        // 初始化字体
        var savedFont = themeService.GetSavedFontFamily();
        themeService.SetFontFamily(savedFont);

        var preferenceStorage = serviceProvider.GetRequiredService<IPreferenceStorage>();
        preferenceStorage.Set("app_location", PathHelper.GetAppFilePath("AELP.Desktop.exe"));


        // 预加载数据，避免首次打开相关页面时的卡顿
        Task.Run(async () =>
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
                await db.Database.EnsureCreatedAsync();
                using var favoritesFromStorage = scope.ServiceProvider
                    .GetRequiredKeyedService<IFavoritesDataStorageService>(null)
                    .LoadFavorites();
                using var mistakesFromStorage = scope.ServiceProvider
                    .GetRequiredKeyedService<IMistakeDataStorageService>(null)
                    .LoadMistakeData();
                using var testRecordsFromStorage = scope.ServiceProvider
                    .GetRequiredKeyedService<ITestDataStorageService>(null)
                    .LoadTestData();
                using var favoritesFromStorage2 = scope.ServiceProvider
                    .GetRequiredKeyedService<IFavoritesDataStorageService>(null)
                    .LoadFavorites();
            }
            catch
            {
                // ignore
            }
        });

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            var args = desktop.Args ?? [];
            var options = ParseArgs(args);
            var vm = serviceProvider.GetRequiredService<MainWindowViewModel>();
            if (options.StartPage.HasValue)
            {
                vm.GoTo(options.StartPage.Value, options.Parameter);
            }

            desktop.MainWindow = new MainWindow
            {
                DataContext = vm,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    private static AppOptions ParseArgs(string[] args)
    {
        ApplicationPageNames? startPage = null;
        string? parameter = null;
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "page" && i + 1 < args.Length)
            {
                if (Enum.TryParse<ApplicationPageNames>(args[i + 1], true, out var page))
                {
                    startPage = page;
                    if (args.Length > i + 1)
                    {
                        parameter = args[i + 1];
                    }

                    break;
                }
            }

            if (args[i] == "search" && i + 1 < args.Length)
            {
                startPage = ApplicationPageNames.Dictionary;
                parameter = args[i + 1];
                break;
            }

            if (args[i] == "test")
            {
                startPage = ApplicationPageNames.Tests;
                parameter = args.Length > i + 1 ? args[i + 1] : "10";
                break;
            }

            if (args[i] == "favorites")
            {
                startPage = ApplicationPageNames.Favorites;
                break;
            }

            if (args[i] == "mistakes")
            {
                startPage = ApplicationPageNames.Mistakes;
                break;
            }

            if (args[i] != "dictionary") continue;
            startPage = ApplicationPageNames.Dictionary;
            if (args.Length > i + 1)
            {
                parameter = args[i + 1];
            }

            break;
        }

        return new AppOptions(args, startPage, parameter);
    }
}

public record AppOptions(string[] Args, ApplicationPageNames? StartPage = null, string? Parameter = null);