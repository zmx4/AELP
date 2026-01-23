using AELP.Data;
using AELP.Models;

namespace AELP.Services;

public interface IFavoritesDataStorageService
{
    public void SaveFavorites(dictionary[] favorites);
    public FavoritesDataModel[] LoadFavorites();
}