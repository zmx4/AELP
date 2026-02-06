using System.Collections.Generic;
using System.Threading.Tasks;
using AELP.Models;

namespace AELP.Services;

public interface IWordQueryService
{
    public dictionary? QueryWordInfo(string word);
    public Task<dictionary?> QueryWordInfoAsync(string word);
    public string QueryWordTranslation(string word);
    public string QueryWord(string translation);
    public List<dictionary> QueryWords(string translation);
    // public Task<List<dictionary>> QueryWordsAsync(string translation);
    public Task<dictionary[]> QueryWordsAsync(string translation);
    public Task<dictionary[]> QueryWordsAsync(string translation,int skip,int take);

}