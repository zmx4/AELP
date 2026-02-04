using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AELP.Helper;
using AELP.Models;
using AELP.Services;
using JetBrains.Annotations;

namespace AELP.UnitTest.Services;

[Collection("UserDb")]
[TestSubject(typeof(FavoritesDataStorageService))]
public class FavoritesDataStorageServiceTest : IDisposable
{
	public FavoritesDataStorageServiceTest()
	{
		ResetDatabase();
	}

	public void Dispose()
	{
		ResetDatabase();
	}

	[Fact]
	public async Task AddToFavorites_CreatesWordAndFavoriteAndRaisesEvent()
	{
		var service = new FavoritesDataStorageService();
		var eventCount = 0;
		service.OnFavoritesChanged += (_, _) => eventCount++;

		var input = new dictionary
		{
			word = "apple",
			translation = "苹果",
			cet4 = 1
		};

		await service.AddToFavorites(input);

		var favorites = await service.LoadFavorites();
		var favorite = Assert.Single(favorites);

		Assert.Equal(1, eventCount);
		Assert.True(favorite.IsFavorite);
		Assert.True(favorite.IsCet4);
		Assert.False(favorite.IsCet6);
		Assert.False(favorite.IsHs);
		Assert.False(favorite.IsPh);
		Assert.False(favorite.IsTf);
		Assert.False(favorite.IsYs);
		Assert.NotNull(favorite.Word);
		Assert.Equal("apple", favorite.Word.Word);
		Assert.Equal("苹果", favorite.Word.Translation);
	}

	[Fact]
	public async Task AddToFavorites_UpdatesExistingFavoriteFlags()
	{
		var service = new FavoritesDataStorageService();

		await service.AddToFavorites(new dictionary
		{
			word = "orange",
			translation = "橙子",
			cet4 = 1
		});

		await service.AddToFavorites(new dictionary
		{
			word = "orange",
			translation = "橙子",
			cet6 = 1,
			hs = 1
		});

		var favorites = await service.LoadFavorites();
		var favorite = Assert.Single(favorites);

		Assert.True(favorite.IsFavorite);
		Assert.False(favorite.IsCet4);
		Assert.True(favorite.IsCet6);
		Assert.True(favorite.IsHs);
	}

	[Fact]
	public async Task RemoveFromFavorites_UnfavoritesAndRaisesEvent()
	{
		var service = new FavoritesDataStorageService();
		var eventCount = 0;
		service.OnFavoritesChanged += (_, _) => eventCount++;

		var input = new dictionary
		{
			word = "banana",
			translation = "香蕉",
			cet4 = 1
		};

		await service.AddToFavorites(input);
		await service.RemoveFromFavorites(input);

		var favorites = await service.LoadFavorites();

		Assert.Empty(favorites);
		Assert.Equal(2, eventCount);
	}

	[Fact]
	public async Task SaveFavorites_ReplacesAndUpdatesFlags()
	{
		var service = new FavoritesDataStorageService();
		var eventCount = 0;
		service.OnFavoritesChanged += (_, _) => eventCount++;

		await service.SaveFavorites(new[]
		{
			new dictionary { word = "alpha", translation = "阿尔法", cet4 = 1 },
			new dictionary { word = "beta", translation = "贝塔", cet6 = 1 }
		});

		await service.SaveFavorites(new[]
		{
			new dictionary { word = "beta", translation = "贝塔", cet6 = 1, hs = 1 },
			new dictionary { word = "gamma", translation = "伽马", ph = 1 }
		});

		var favorites = await service.LoadFavorites();

		Assert.Equal(2, favorites.Length);
		Assert.Equal(2, eventCount);

		var beta = favorites.Single(f => f.Word.Word == "beta");
		var gamma = favorites.Single(f => f.Word.Word == "gamma");

		Assert.True(beta.IsFavorite);
		Assert.True(beta.IsCet6);
		Assert.True(beta.IsHs);
		Assert.False(beta.IsCet4);

		Assert.True(gamma.IsFavorite);
		Assert.True(gamma.IsPh);
		Assert.False(gamma.IsCet4);
		Assert.False(gamma.IsCet6);
	}

	private static void ResetDatabase()
	{
		var dbPath = PathHelper.GetLocalFilePath("userdata.sqlite");
		if (File.Exists(dbPath))
		{
			File.Delete(dbPath);
		}

		var field = typeof(FavoritesDataStorageService)
			.GetField("_dbChecked", BindingFlags.NonPublic | BindingFlags.Static);
		field?.SetValue(null, false);
	}
}