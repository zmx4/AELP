using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AELP.Data;
using AELP.Helper;
using AELP.Messages;
using AELP.Models;
using AELP.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AELP.ViewModels;

public partial class FavoritesPageViewModel : PageViewModel
{
    [ObservableProperty] private ObservableCollection<FavoritesDataModel> _favorites;
    // [ObservableProperty] private bool _isLoading;

    private readonly IFavoritesDataStorageService _dataStorageService;
    private readonly IUserWordQueryService _wordQueryService;

    public FavoritesPageViewModel(IFavoritesDataStorageService dataStorageService,
        IUserWordQueryService wordQueryService)
    {
        PageNames = ApplicationPageNames.Favorites;

        _dataStorageService = dataStorageService;

        _wordQueryService = wordQueryService;

        _favorites = new ObservableCollection<FavoritesDataModel>();

        // LoadFavorites().ConfigureAwait(false);
        LoadFavoritesCommand.Execute(null);

        // _ = LoadFavorites();
    }

    [RelayCommand]
    private async Task LoadFavorites()
    {
        var favoritesFromStorage = await _dataStorageService.LoadFavorites();

        foreach (var favoritesDataModel in favoritesFromStorage)
        {
            favoritesDataModel.Word?.Translation =
                StringNormalizeHelper.NormalizeTranslation(favoritesDataModel.Word?.Translation);
        }

        Favorites = new ObservableCollection<FavoritesDataModel>(favoritesFromStorage);
    }

    [RelayCommand]
    private async Task ShowFavoriteDetails(FavoritesDataModel favorite)
    {
        var wordInfo = await _wordQueryService.QueryUserWordInfoAsync(favorite.WordId);
        if (wordInfo != null)
        {
            var temp = new Dictionary
            {
                RawWord = wordInfo.Word,
                Translation = wordInfo.Translation,
                Cet4 = (favorite.IsCet4) ? 1 : 0,
                Cet6 = (favorite.IsCet6) ? 1 : 0,
                Tf = (favorite.IsTf) ? 1 : 0,
                Ys = (favorite.IsYs) ? 1 : 0,
                Hs = (favorite.IsHs) ? 1 : 0,
                Ph = (favorite.IsPh) ? 1 : 0
            };

            WeakReferenceMessenger.Default.Send(new NavigationMessage(ApplicationPageNames.Detail, temp));
        }
    }

    [RelayCommand]
    private async Task RemoveFromFavorites(FavoritesDataModel favorite)
    {
        var wordInfo = await _wordQueryService.QueryUserWordInfoAsync(favorite.WordId);
        if (wordInfo != null)
        {
            var temp = new Dictionary
            {
                RawWord = wordInfo.Word,
                Translation = wordInfo.Translation,
                Cet4 = (favorite.IsCet4) ? 1 : 0,
                Cet6 = (favorite.IsCet6) ? 1 : 0,
                Tf = (favorite.IsTf) ? 1 : 0,
                Ys = (favorite.IsYs) ? 1 : 0,
                Hs = (favorite.IsHs) ? 1 : 0,
                Ph = (favorite.IsPh) ? 1 : 0
            };

            await _dataStorageService.RemoveFromFavorites(temp);
            Favorites.Remove(favorite);
        }
    }

    [RelayCommand]
    private async Task Refresh()
    {
        await LoadFavorites();
    }
}