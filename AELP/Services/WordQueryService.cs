using System.Collections.Generic;
using AELP.Models;
using System.Linq;

namespace AELP.Services;

public class WordQueryService : IWordQueryService
{
    private readonly AppDbContext _context;

    public WordQueryService(AppDbContext context)
    {
        _context = context;
    }
    
    public dictionary? QueryWordInfo(string word)
    {
        return _context.dictionaries.FirstOrDefault(x => x.word == word);
    }
    
    public string QueryWordTranslation(string word)
    {
        var result = _context.dictionaries.FirstOrDefault(x => x.word == word);
        return result?.translation ?? string.Empty;
    }

    public string QueryWord(string translation)
    {
        var result = _context.dictionaries.FirstOrDefault(x => x.translation != null && x.translation.Contains(translation));
        return result?.word ?? string.Empty;
    }

    public List<string> QueryWords(string translation)
    {
        var results = _context.dictionaries
            .Where(x => x.translation != null && x.translation.Contains(translation))
            .Select(x => x.word)
            .ToList();  
        return results;
    }
}