using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AELP.Data;
using Microsoft.EntityFrameworkCore;

namespace AELP.Services;

public class MistakeDataStorageService : IMistakeDataStorageService
{
    private readonly IWordQueryService _wordQueryService;

    public MistakeDataStorageService(IWordQueryService wordQueryService)
    {
        _wordQueryService = wordQueryService;
    }

    public async Task SaveMistakeData(MistakeDataModel[] mistakeData)
    {
        await using var context = new UserDbContext();
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

    public async Task<MistakeDataModel[]> LoadMistakeData()
    {
        await using var context = new UserDbContext();
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
                translation = _wordQueryService.QueryWordTranslation(wordText);
            }

            item.Word = wordText;
            item.Translation = translation;
        }

        return mistakeData;
    }

    public async Task UpdateMistakeData(MistakeDataModel[] mistakeData)
    {
        await using var context = new UserDbContext();
        await context.Database.EnsureCreatedAsync();
        context.Mistakes.UpdateRange(mistakeData);
        await context.SaveChangesAsync();
    }
}