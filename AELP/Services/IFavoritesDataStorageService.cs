using System;
using System.Threading.Tasks;
using AELP.Data;
using AELP.Models;

namespace AELP.Services;

public interface IFavoritesDataStorageService
{
    public Task AddToFavorites(dictionary favorite);
    public Task SaveFavorites(dictionary[] favorites);
    public Task<FavoritesDataModel[]> LoadFavorites();
    public event EventHandler OnFavoritesChanged;
}

public class FavoriteStorageUpdatedEventArgs : EventArgs {
    public FavoritesDataModel UpdatedFavorite { get; }

    public FavoriteStorageUpdatedEventArgs(FavoritesDataModel favorite) {
        UpdatedFavorite = favorite;
    }
}