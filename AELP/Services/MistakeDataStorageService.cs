using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AELP.Data;
using Microsoft.EntityFrameworkCore;

namespace AELP.Services;

/// <summary>
/// 错题数据存储服务，负责错题读写与关联单词信息补全。
/// </summary>
public class MistakeDataStorageService(
    IWordQueryService wordQueryService,
    IDbContextFactory<UserDbContext> contextFactory) : IMistakeDataStorageService
{
    /// <inheritdoc />
    public async Task SaveMistakeData(MistakeDataModel[] mistakeData)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        await context.Database.EnsureCreatedAsync();

        // 1. Resolve Word IDs for items that have a Word string but possibly no WordId (or 0)
        var itemsByWord = mistakeData
            .Where(m => !string.IsNullOrEmpty(m.Word) && m.WordId == 0)
            .GroupBy(m => m.Word!)
            .ToDictionary(g => g.Key, g => g.ToList());

        if (itemsByWord.Any())
        {
            var words = itemsByWord.Keys.ToList();
            var existingWords = await context.Words
                .Where(w => words.Contains(w.Word))
                .ToDictionaryAsync(w => w.Word);

            var newWords = new List<WordDataModel>();
            foreach (var wordText in words)
            {
                if (!existingWords.ContainsKey(wordText))
                {
                    var newWord = new WordDataModel
                    {
                        Word = wordText,
                        Translation = "" 
                    };
                    newWords.Add(newWord);
                    existingWords[wordText] = newWord;
                }
            }

            if (newWords.Any())
            {
                await context.Words.AddRangeAsync(newWords);
                await context.SaveChangesAsync();
            }

            foreach (var kvp in itemsByWord)
            {
                if (existingWords.TryGetValue(kvp.Key, out var wordData))
                {
                    foreach (var mistakeItem in kvp.Value)
                    {
                        mistakeItem.WordId = wordData.Id;
                    }
                }
            }
        }
        
        foreach (var item in mistakeData)
        {
            if (item.Id == 0)
            {
                await context.Mistakes.AddAsync(item);
            }
            else
            {
                context.Mistakes.Update(item);
            }
        }
        await context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<MistakeDataModel[]> LoadMistakeData()
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        await context.Database.EnsureCreatedAsync();
        var mistakeData = await context.Mistakes
            .Include(m => m.RawWord)
            .ToArrayAsync();

        foreach (var item in mistakeData)
        {
            var wordText = item.RawWord?.Word ?? item.Word;
            var translation = item.RawWord?.Translation;

            if (string.IsNullOrWhiteSpace(translation) && !string.IsNullOrWhiteSpace(wordText))
            {
                translation = wordQueryService.QueryWordTranslation(wordText);
            }

            item.Word = wordText;
            item.Translation = translation;
        }

        return mistakeData;
    }

    /// <inheritdoc />
    public async Task<MistakeDataModel[]> LoadMistakeData(int count)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        await context.Database.EnsureCreatedAsync();
        var mistakeData = await context.Mistakes
            .Include(m => m.RawWord)
            .Take(count)
            .ToArrayAsync();

        foreach (var item in mistakeData)
        {
            var wordText = item.RawWord?.Word ?? item.Word;
            var translation = item.RawWord?.Translation;

            if (string.IsNullOrWhiteSpace(translation) && !string.IsNullOrWhiteSpace(wordText))
            {
                translation = wordQueryService.QueryWordTranslation(wordText);
            }

            item.Word = wordText;
            item.Translation = translation;
        }
        return  mistakeData;
    }


    /// <inheritdoc />
    public async Task UpdateMistakeData(MistakeDataModel[] mistakeData)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        await context.Database.EnsureCreatedAsync();
        context.Mistakes.UpdateRange(mistakeData);
        await context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<MistakeDataModel[]> LoadMistakeDataByWordIds(int[] wordIds)
    {
        var wordIdSet = wordIds.ToHashSet();
        await using var context = await contextFactory.CreateDbContextAsync();
        await context.Database.EnsureCreatedAsync();
        var mistakeData = await context.Mistakes
            .Where(m => wordIdSet.Contains(m.WordId))
            .Include(m => m.RawWord)
            .ToArrayAsync();
        return mistakeData;
    }
}