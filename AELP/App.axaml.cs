using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using AELP.Data;
using AELP.Factories;
using Avalonia.Markup.Xaml;
using AELP.ViewModels;
using AELP.Views;
using Microsoft.Extensions.DependencyInjection;
using AELP.Services;
using AELP.Models;
using Microsoft.EntityFrameworkCore;

namespace AELP;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    // public static ServiceProvider ServiceProvider { get; private set; }
    
    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        collection.AddDbContext<AppDbContext>(options =>
            options.UseSqlite("Data Source=../../../Assets/Database/stardict.db"));
        collection.AddTransient<IWordQueryService, WordQueryService>();
        collection.AddSingleton<PageFactory>();
        collection.AddSingleton<MainWindowViewModel>();
        collection.AddTransient<DictionaryPageViewModel>();
        collection.AddTransient<FavoritesPageViewModel>();
        collection.AddTransient<TestsPageViewModel>();
        collection.AddTransient<MistakePageViewModel>();
        collection.AddTransient<SummaryPageViewModel>();
        collection.AddTransient<DetailPageViewModel>();
        collection.AddTransient<SettingsPageViewModel>();

        collection.AddSingleton<Func<ApplicationPageNames, PageViewModel>>(x => name => name switch
        {
            ApplicationPageNames.Dictionary => x.GetRequiredService<DictionaryPageViewModel>(),
            ApplicationPageNames.Favorites => x.GetRequiredService<FavoritesPageViewModel>(),
            ApplicationPageNames.Tests => x.GetRequiredService<TestsPageViewModel>(),
            ApplicationPageNames.Mistakes => x.GetRequiredService<MistakePageViewModel>(),
            ApplicationPageNames.Summary => x.GetRequiredService<SummaryPageViewModel>(),
            ApplicationPageNames.Detail => x.GetRequiredService<DetailPageViewModel>(),
            ApplicationPageNames.Settings => x.GetRequiredService<SettingsPageViewModel>(),

            _ => throw new NotImplementedException($"No ViewModel implemented for page {name}")
        });
        
        var serviceProvider = collection.BuildServiceProvider();
        
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