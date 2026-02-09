using System.Collections.Generic;
using AELP.Models;
using System.Linq;
using System.Threading.Tasks;

namespace AELP.Services;

public class WordQueryService : IWordQueryService
{
    private readonly AppDbContext _context;

    public WordQueryService(AppDbContext context)
    {
        _context = context;
    }
    
    public Dictionary? QueryWordInfo(string word)
    {
        return _context.Dictionaries.FirstOrDefault(x => x.word == word);
    }

    public async Task<Dictionary?> QueryWordInfoAsync(string word)
    {
        return await Task.Run(() => _context.Dictionaries.FirstOrDefault(x => x.word == word));
    }

    public string QueryWordTranslation(string word)
    {
        var result = _context.Dictionaries.FirstOrDefault(x => x.word == word);
        return result?.translation ?? string.Empty;
    }

    public string QueryWord(string translation)
    {
        var result = _context.Dictionaries.FirstOrDefault(x => x.translation != null && x.translation.Contains(translation));
        return result?.word ?? string.Empty;
    }

    public List<Dictionary> QueryWords(string translation)
    {
        var results = _context.Dictionaries
            .Where(x => x.translation != null && x.translation.Contains(translation))
            //.Select(x => x.word)
            .ToList();  
        return results;
    }

    public async Task<Dictionary[]> QueryWordsAsync(string translation)
    {
        return await Task.Run(() =>
            _context.Dictionaries
                .Where(x => x.translation != null && x.translation.Contains(translation))
                .ToArray());
    }

    public async Task<Dictionary[]> QueryWordsAsync(string translation, int skip, int take)
    {
        return await Task.Run(() =>
            _context.Dictionaries
                .Where(x => x.translation != null && x.translation.Contains(translation))
                .Skip(skip)
                .Take(take)
                .ToArray());
    }
}