using System.Collections.Generic;
using AELP.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AELP.Services;

/// <summary>
/// 词典查询服务，基于词典数据库提供单词检索。
/// </summary>
public class WordQueryService(IDbContextFactory<AppDbContext> contextFactory) : IWordQueryService
{
    /// <inheritdoc />
    public Dictionary? QueryWordInfo(string word)
    {
        using var context = contextFactory.CreateDbContext();
        return context.Dictionaries.FirstOrDefault(x => x.RawWord == word);
    }

    /// <inheritdoc />
    public async Task<Dictionary?> QueryWordInfoAsync(string word)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        return await context.Dictionaries.FirstOrDefaultAsync(x => x.RawWord == word);
    }

    /// <inheritdoc />
    public string QueryWordTranslation(string word)
    {
        using var context = contextFactory.CreateDbContext();
        var result = context.Dictionaries.FirstOrDefault(x => x.RawWord == word);
        return result?.Translation ?? string.Empty;
    }

    /// <inheritdoc />
    public string QueryWord(string translation)
    {
        using var context = contextFactory.CreateDbContext();
        var result = context.Dictionaries.FirstOrDefault(x => x.Translation != null && x.Translation.Contains(translation));
        return result?.RawWord ?? string.Empty;
    }

    /// <inheritdoc />
    public List<Dictionary> QueryWords(string translation)
    {
        using var context = contextFactory.CreateDbContext();
        var results = context.Dictionaries
            .Where(x => x.Translation != null && x.Translation.Contains(translation))
            //.Select(x => x.word)
            .ToList();  
        return results;
    }

    /// <inheritdoc />
    public async Task<Dictionary[]> QueryWordsAsync(string translation)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        return await context.Dictionaries
            .Where(x => x.Translation != null && x.Translation.Contains(translation))
            .OrderByDescending(x => x.Hs + x.Cet4 +x.Cet6 + x.Tf + x.Hs + x.Ph)
            .ToArrayAsync();
    }

    /// <inheritdoc />
    public async Task<Dictionary[]> QueryWordsAsync(string translation, int skip, int take)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        var result =  await context.Dictionaries
            .Where(x => x.Translation != null && x.Translation.Contains(translation))
            .OrderByDescending(x => x.Hs + x.Cet4 +x.Cet6 + x.Tf + x.Hs + x.Ph)
            .Skip(skip)
            .Take(take)
            .ToArrayAsync();
        
        await Task.Delay(200);

        return result;
    }
}