using System;
using AELP.Data;
using AELP.Factories;
using AELP.Helper;
using AELP.Models;
using AELP.Services;
using AELP.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AELP;

public static class AELPDISetup 
{
    public static void AddAelpServices(this IServiceCollection services)
    {
        services.AddSingleton<IPreferenceStorage, JsonPreferenceStorage>();
        services.AddSingleton<INotifyService, NotifyService>();
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<IKeyboardPreferenceService, KeyboardPreferenceService>();
        services.AddSingleton<IWordQueryService, WordQueryService>();
        services.AddSingleton<IFavoritesDataStorageService, FavoritesDataStorageService>();
        services.AddSingleton<IMistakeDataStorageService, MistakeDataStorageService>();
        services.AddSingleton<ITestDataStorageService, TestDataStorageService>();
        services.AddSingleton<IUserWordQueryService, UserWordQueryService>();
        services.AddSingleton<ITestWordGetter, TestWordGetter>();
    }

    public static void AddAelpViewModels(this IServiceCollection services)
    {
        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<DictionaryPageViewModel>();
        services.AddTransient<FavoritesPageViewModel>();
        services.AddTransient<TestsPageViewModel>();
        services.AddTransient<TestSessionPageViewModel>();
        services.AddTransient<MistakePageViewModel>();
        services.AddTransient<MistakeReviewPageViewModel>();
        services.AddTransient<SummaryPageViewModel>(sp => new SummaryPageViewModel(
            sp.GetRequiredService<ITestDataStorageService>()));
        services.AddTransient<DetailPageViewModel>();
        services.AddTransient<SettingsPageViewModel>();
        
        services.AddSingleton<Func<ApplicationPageNames, PageViewModel>>(x => name => name switch
        {
            ApplicationPageNames.Dictionary    => x.GetRequiredService<DictionaryPageViewModel   >(),
            ApplicationPageNames.Favorites     => x.GetRequiredService<FavoritesPageViewModel    >(),
            ApplicationPageNames.Tests         => x.GetRequiredService<TestsPageViewModel        >(),
            ApplicationPageNames.TestSession   => x.GetRequiredService<TestSessionPageViewModel  >(),
            ApplicationPageNames.Mistakes      => x.GetRequiredService<MistakePageViewModel      >(),
            ApplicationPageNames.MistakeReview => x.GetRequiredService<MistakeReviewPageViewModel>(),
            ApplicationPageNames.Summary       => x.GetRequiredService<SummaryPageViewModel      >(),
            ApplicationPageNames.Detail        => x.GetRequiredService<DetailPageViewModel       >(),
            ApplicationPageNames.Settings      => x.GetRequiredService<SettingsPageViewModel     >(),

            _ => throw new NotImplementedException($"No ViewModel implemented for page {name}")
        });
    }
    
    public static void AddAelpFactories(this IServiceCollection services)
    {
        services.AddSingleton<PageFactory>();
    }

    public static void AddAelpDbContexts(this IServiceCollection services)
    {
        var dbPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "Database", "stardict.db");
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));
        services.AddDbContextFactory<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));
        services.AddDbContext<UserDbContext>(options => 
            options.UseSqlite($"Data Source={PathHelper.GetLocalFilePath(UserDbContext.DbName)}"));
        services.AddDbContextFactory<UserDbContext>(options =>
            options.UseSqlite($"Data Source={PathHelper.GetLocalFilePath(UserDbContext.DbName)}"));
    }
}