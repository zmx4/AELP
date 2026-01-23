using System;
using System.Threading.Tasks;
using AELP.Data;
using AELP.Models;

namespace AELP.Services;

public class FavoritesDataStorageService : IFavoritesDataStorageService
{
    public async Task SaveFavorites(dictionary[] favorites)
    {
        throw new NotImplementedException();
    }

    public async Task<FavoritesDataModel[]> LoadFavorites()
    {
        throw new NotImplementedException();
    }

    public event EventHandler? OnFavoritesChanged;
}