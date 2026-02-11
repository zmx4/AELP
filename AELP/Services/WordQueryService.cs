using System.Collections.Generic;
using AELP.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AELP.Services;

public class WordQueryService(IDbContextFactory<AppDbContext> contextFactory) : IWordQueryService
{
    public Dictionary? QueryWordInfo(string word)
    {
        using var context = contextFactory.CreateDbContext();
        return context.Dictionaries.FirstOrDefault(x => x.RawWord == word);
    }

    public async Task<Dictionary?> QueryWordInfoAsync(string word)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        return await context.Dictionaries.FirstOrDefaultAsync(x => x.RawWord == word);
    }

    public string QueryWordTranslation(string word)
    {
        using var context = contextFactory.CreateDbContext();
        var result = context.Dictionaries.FirstOrDefault(x => x.RawWord == word);
        return result?.Translation ?? string.Empty;
    }

    public string QueryWord(string translation)
    {
        using var context = contextFactory.CreateDbContext();
        var result = context.Dictionaries.FirstOrDefault(x => x.Translation != null && x.Translation.Contains(translation));
        return result?.RawWord ?? string.Empty;
    }

    public List<Dictionary> QueryWords(string translation)
    {
        using var context = contextFactory.CreateDbContext();
        var results = context.Dictionaries
            .Where(x => x.Translation != null && x.Translation.Contains(translation))
            //.Select(x => x.word)
            .ToList();  
        return results;
    }

    public async Task<Dictionary[]> QueryWordsAsync(string translation)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        return await context.Dictionaries
            .Where(x => x.Translation != null && x.Translation.Contains(translation))
            .ToArrayAsync();
    }

    public async Task<Dictionary[]> QueryWordsAsync(string translation, int skip, int take)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        return await context.Dictionaries
            .Where(x => x.Translation != null && x.Translation.Contains(translation))
            .Skip(skip)
            .Take(take)
            .ToArrayAsync();
    }
}