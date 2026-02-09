using System.Collections.Generic;
using System.Threading.Tasks;
using AELP.Models;

namespace AELP.Services;

public interface IWordQueryService
{
    public Dictionary? QueryWordInfo(string word);
    public Task<Dictionary?> QueryWordInfoAsync(string word);
    public string QueryWordTranslation(string word);
    public string QueryWord(string translation);
    public List<Dictionary> QueryWords(string translation);
    // public Task<List<dictionary>> QueryWordsAsync(string translation);
    public Task<Dictionary[]> QueryWordsAsync(string translation);
    public Task<Dictionary[]> QueryWordsAsync(string translation,int skip,int take);

}