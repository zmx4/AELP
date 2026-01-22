using System.Collections.Generic;
using AELP.Models;

namespace AELP.Services;

public interface IWordQueryService
{
    public dictionary? QueryWordInfo(string word);
    public string QueryWordTranslation(string word);
    public string QueryWord(string translation);
    public List<string> QueryWords(string translation);
}