using System;
using System.Threading.Tasks;
using AELP.Data;
using AELP.Models;

namespace AELP.Services;

public interface IFavoritesDataStorageService
{
    public Task AddToFavorites(Dictionary favorite);
    public Task RemoveFromFavorites(Dictionary favorite);
    public Task SaveFavorites(Dictionary[] favorites);
    public Task<FavoritesDataModel[]> LoadFavorites();
    public event EventHandler OnFavoritesChanged;
}

public class FavoriteStorageUpdatedEventArgs(FavoritesDataModel favorite) : EventArgs
{
    public FavoritesDataModel UpdatedFavorite { get; } = favorite;
}