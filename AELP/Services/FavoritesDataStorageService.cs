using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AELP.Data;
using AELP.Models;
using Microsoft.EntityFrameworkCore;

namespace AELP.Services;

public class FavoritesDataStorageService : IFavoritesDataStorageService
{
    private static bool _dbChecked = false;
    public event EventHandler? OnFavoritesChanged;

    public async Task AddToFavorites(Dictionary favorite)
    {
        await using var context = new UserDbContext();
        if (!_dbChecked)
        {
            await context.Database.EnsureCreatedAsync();
            _dbChecked = true;
        }
        
        var wordModel = await context.Words.FirstOrDefaultAsync(w => w.Word == favorite.word);
        if (wordModel == null)
        {
            wordModel = new WordDataModel
            {
                Word = favorite.word,
                Translation = favorite.translation ?? ""
            };
            await context.Words.AddAsync(wordModel);
            await context.SaveChangesAsync();
        }

        var favModel = await context.Favorites.FindAsync(wordModel.Id);
        if (favModel == null)
        {
            favModel = new FavoritesDataModel { WordId = wordModel.Id };
            await context.Favorites.AddAsync(favModel);
        }

        favModel.IsFavorite = true;
        favModel.IsCet4 = favorite.Cet4 == 1;
        favModel.IsCet6 = favorite.Cet6 == 1;
        favModel.IsHs = favorite.Hs == 1;
        favModel.IsPh = favorite.Ph == 1;
        favModel.IsTf = favorite.Tf == 1;
        favModel.IsYs = favorite.Ys == 1;

        await context.SaveChangesAsync();
        OnFavoritesChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task RemoveFromFavorites(Dictionary favorite)
    {
        await using var context = new UserDbContext();
        if (!_dbChecked)
        {
            await context.Database.EnsureCreatedAsync();
            _dbChecked = true;
        }
        
        var wordModel = await context.Words.FirstOrDefaultAsync(w => w.Word == favorite.word);
        if (wordModel == null) return;

        var favModel = await context.Favorites.FindAsync(wordModel.Id);
        if (favModel != null)
        {
            favModel.IsFavorite = false;
            await context.SaveChangesAsync();
            OnFavoritesChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public async Task SaveFavorites(Dictionary[] favorites)
    {
        await using var context = new UserDbContext();
        if (!_dbChecked)
        {
            await context.Database.EnsureCreatedAsync();
            _dbChecked = true;
        }

        // 1. Get IDs of words in the new favorites list. Ensure they exist in WordDataModel.
        var dictWords = favorites.ToDictionary(d => d.word);
        var inputWordStrings = dictWords.Keys.ToList();

        // Check existing WordDataModels
        var existingWords = await context.Words
            .Where(w => inputWordStrings.Contains(w.Word))
            .ToListAsync();
        
        var existingWordMap = existingWords.ToDictionary(w => w.Word);
        
        // Add missing words
        var newWords = new List<WordDataModel>();
        foreach (var wordStr in inputWordStrings)
        {
            if (!existingWordMap.ContainsKey(wordStr))
            {
                var dictItem = dictWords[wordStr];
                var newWord = new WordDataModel
                {
                    Word = dictItem.word,
                    Translation = dictItem.translation ?? ""
                };
                newWords.Add(newWord);
                // Temporarily add to map, ID will be 0 until save, but we handle Save below for batch
                existingWordMap[wordStr] = newWord;
            }
        }

        if (newWords.Any())
        {
            await context.Words.AddRangeAsync(newWords);
            await context.SaveChangesAsync(); // IDs are generated here
        }

        // Now we have IDs for all input words.
        var activeWordIds = existingWordMap.Values.Select(w => w.Id).ToHashSet();

        // 2. Un-favorite items not in the list
        var currentlyFavorited = await context.Favorites.Where(f => f.IsFavorite).ToListAsync();
        
        foreach (var oldFav in currentlyFavorited)
        {
            if (!activeWordIds.Contains(oldFav.WordId))
            {
                oldFav.IsFavorite = false;
            }
        }

        // 3. Update or Insert items from the list
        // Retrieve or use tracked entities for the input list
        foreach (var kvp in existingWordMap)
        {
            string wordStr = kvp.Key;
            WordDataModel wordModel = kvp.Value;
            var dictItem = dictWords[wordStr];

            // Local lookup first (from currentlyFavorited list)
            var favModel = currentlyFavorited.FirstOrDefault(f => f.WordId == wordModel.Id);
            
            if (favModel == null)
            {
                // Check persistence if not in memory list
                favModel = await context.Favorites.FindAsync(wordModel.Id);
                if (favModel == null)
                {
                    favModel = new FavoritesDataModel { WordId = wordModel.Id };
                    await context.Favorites.AddAsync(favModel);
                }
            }

            // Update
            favModel.IsFavorite = true;
            favModel.IsCet4 = dictItem.Cet4 == 1;
            favModel.IsCet6 = dictItem.Cet6 == 1;
            favModel.IsHs = dictItem.Hs == 1;
            favModel.IsPh = dictItem.Ph == 1;
            favModel.IsTf = dictItem.Tf == 1;
            favModel.IsYs = dictItem.Ys == 1;
        }

        await context.SaveChangesAsync();
        OnFavoritesChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task<FavoritesDataModel[]> LoadFavorites()
    {
        await using var context = new UserDbContext();
        if (!_dbChecked)
        {
            await context.Database.EnsureCreatedAsync();
            _dbChecked = true;
        }
        return await context.Favorites.Include(f => f.Word).Where(f => f.IsFavorite).ToArrayAsync();
    }
}