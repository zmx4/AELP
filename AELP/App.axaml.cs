using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using System.Threading.Tasks;
using AELP.Data;
using AELP.Factories;
using Avalonia.Markup.Xaml;
using AELP.ViewModels;
using AELP.Views;
using Microsoft.Extensions.DependencyInjection;
using AELP.Services;
using AELP.Models;
using Microsoft.EntityFrameworkCore;
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
                .AddLightTheme() 
        );
    }

    // public static ServiceProvider ServiceProvider { get; private set; }
    
    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        var dbPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "Database", "stardict.db");
        collection.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));
        collection.AddDbContext<UserDbContext>();
        collection.AddSingleton<IPreferenceStorage, FilePreferenceStorage>();
        collection.AddSingleton<IThemeService, ThemeService>();
        collection.AddTransient<IWordQueryService, WordQueryService>();
        collection.AddTransient<IFavoritesDataStorageService, FavoritesDataStorageService>();
        collection.AddTransient<IMistakeDataStorageService, MistakeDataStorageService>();
        collection.AddTransient<ITestDataStorageService, TestDataStorageService>();
        collection.AddSingleton<IUserWordQueryService, UserWordQueryService>();
        collection.AddTransient<ITestWordGetter, TestWordGetter>();
        collection.AddSingleton<PageFactory>();
        collection.AddSingleton<MainWindowViewModel>();
        collection.AddTransient<DictionaryPageViewModel>();
        collection.AddTransient<FavoritesPageViewModel>();
        collection.AddTransient<TestsPageViewModel>();
        collection.AddTransient<MistakePageViewModel>();
        collection.AddTransient<MistakeReviewPageViewModel>();
        collection.AddTransient<SummaryPageViewModel>(sp => new SummaryPageViewModel(
            sp.GetRequiredService<ITestDataStorageService>()));
        collection.AddTransient<DetailPageViewModel>();
        collection.AddTransient<SettingsPageViewModel>();

        collection.AddSingleton<Func<ApplicationPageNames, PageViewModel>>(x => name => name switch
        {
            ApplicationPageNames.Dictionary => x.GetRequiredService<DictionaryPageViewModel>(),
            ApplicationPageNames.Favorites => x.GetRequiredService<FavoritesPageViewModel>(),
            ApplicationPageNames.Tests => x.GetRequiredService<TestsPageViewModel>(),
            ApplicationPageNames.Mistakes => x.GetRequiredService<MistakePageViewModel>(),
            ApplicationPageNames.MistakeReview => x.GetRequiredService<MistakeReviewPageViewModel>(),
            ApplicationPageNames.Summary => x.GetRequiredService<SummaryPageViewModel>(),
            ApplicationPageNames.Detail => x.GetRequiredService<DetailPageViewModel>(),
            ApplicationPageNames.Settings => x.GetRequiredService<SettingsPageViewModel>(),

            _ => throw new NotImplementedException($"No ViewModel implemented for page {name}")
        });
        
        var serviceProvider = collection.BuildServiceProvider();
        
        // 初始化主题
        var themeService = serviceProvider.GetRequiredService<IThemeService>();
        var savedTheme = themeService.GetSavedTheme();
        themeService.SetTheme(savedTheme);
        
        Task.Run(async () =>
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
                await db.Database.EnsureCreatedAsync();
                using var favoritesFromStorage =  scope.ServiceProvider
                    .GetRequiredKeyedService<IFavoritesDataStorageService>(null)
                    .LoadFavorites();
                using var mistakesFromStorage =  scope.ServiceProvider
                    .GetRequiredKeyedService<IMistakeDataStorageService>(null)
                    .LoadMistakeData();
                using var testRecordsFromStorage =  scope.ServiceProvider
                    .GetRequiredKeyedService<ITestDataStorageService>(null)
                    .LoadTestData();
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
            desktop.MainWindow = new MainWindow
            {
                DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>(),
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
}