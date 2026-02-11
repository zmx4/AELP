using System.Reflection;
using AELP.Data;
using AELP.Helper;
using AELP.Models;
using AELP.Services;
using AELP.UnitTest.Helper;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

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
		var service = new FavoritesDataStorageService(CreateContextFactory());
		var eventCount = 0;
		service.OnFavoritesChanged += (_, _) => eventCount++;

		var input = new Dictionary
		{
			RawWord = "apple",
			Translation = "苹果",
			Cet4 = 1
		};

		await service.AddToFavorites(input);

		var favorites = await service.LoadFavorites();
		var favorite = favorites[0];

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
		var service = new FavoritesDataStorageService(CreateContextFactory());

		await service.AddToFavorites(new Dictionary
		{
			RawWord = "orange",
			Translation = "橙子",
			Cet4 = 1
		});

		await service.AddToFavorites(new Dictionary
		{
			RawWord = "orange",
			Translation = "橙子",
			Cet6 = 1,
			Hs = 1
		});

		var favorites = await service.LoadFavorites();
		var favorite = favorites[0];

		Assert.True(favorite.IsFavorite);
		Assert.False(favorite.IsCet4);
		Assert.True(favorite.IsCet6);
		Assert.True(favorite.IsHs);
	}

	[Fact]
	public async Task RemoveFromFavorites_UnfavoritesAndRaisesEvent()
	{
		var service = new FavoritesDataStorageService(CreateContextFactory());
		var eventCount = 0;
		service.OnFavoritesChanged += (_, _) => eventCount++;

		var input = new Dictionary
		{
			RawWord = "banana",
			Translation = "香蕉",
			Cet4 = 1
		};

		await service.AddToFavorites(input);
		await service.RemoveFromFavorites(input);

		var favorites = await service.LoadFavorites();
		Assert.DoesNotContain(favorites, f => f.Word.Word == "banana" && f.IsFavorite);

		// Assert.Empty(favorites);
		Assert.Equal(2, eventCount);
	}

	[Fact]
	public async Task SaveFavorites_ReplacesAndUpdatesFlags()
	{
		var service = new FavoritesDataStorageService(CreateContextFactory());
		var eventCount = 0;
		service.OnFavoritesChanged += (_, _) => eventCount++;

		await service.SaveFavorites(new[]
		{
			new Dictionary { RawWord = "alpha", Translation = "阿尔法", Cet4 = 1 },
			new Dictionary { RawWord = "beta", Translation = "贝塔", Cet6 = 1 }
		});

		await service.SaveFavorites(new[]
		{
			new Dictionary { RawWord = "beta", Translation = "贝塔", Cet6 = 1, Hs = 1 },
			new Dictionary { RawWord = "gamma", Translation = "伽马", Ph = 1 }
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
		var dbPath = PathHelper.GetLocalFilePath("test.db");
		if (File.Exists(dbPath))
		{
			File.Delete(dbPath);
		}

		var field = typeof(FavoritesDataStorageService)
			.GetField("_dbChecked", BindingFlags.NonPublic | BindingFlags.Static);
		field?.SetValue(null, false);
	}

	private static IDbContextFactory<UserDbContext> CreateContextFactory()
	{
		var options = new DbContextOptionsBuilder<UserDbContext>()
			.UseSqlite($"Data Source={PathHelper.GetLocalFilePath(UserDbContext.DbName)}")
			.Options;
		return new TestDbContextFactory<UserDbContext>(options);
	}
}