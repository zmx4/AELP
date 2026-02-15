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

/// <summary>
/// 收藏页面视图模型，负责收藏加载、详情查看和移除。
/// </summary>
public partial class FavoritesPageViewModel : PageViewModel
{
    [ObservableProperty] private ObservableCollection<FavoritesDataModel> _favorites;
    // [ObservableProperty] private bool _isLoading;

    private readonly IFavoritesDataStorageService _dataStorageService;
    private readonly IUserWordQueryService _wordQueryService;

    /// <summary>
    /// 初始化 <see cref="FavoritesPageViewModel"/>。
    /// </summary>
    /// <param name="dataStorageService">收藏存储服务。</param>
    /// <param name="wordQueryService">用户单词查询服务。</param>
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

    /// <summary>
    /// 加载收藏列表。
    /// </summary>
    /// <returns>表示加载完成的异步任务。</returns>
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

    /// <summary>
    /// 显示指定收藏项详情。
    /// </summary>
    /// <param name="favorite">收藏项。</param>
    /// <returns>表示处理完成的异步任务。</returns>
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

    /// <summary>
    /// 移除指定收藏项。
    /// </summary>
    /// <param name="favorite">收藏项。</param>
    /// <returns>表示移除完成的异步任务。</returns>
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

    /// <summary>
    /// 刷新收藏列表。
    /// </summary>
    /// <returns>表示刷新完成的异步任务。</returns>
    [RelayCommand]
    private async Task Refresh()
    {
        await LoadFavorites();
    }
}