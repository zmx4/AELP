using System.Collections.Generic;
using System.Threading.Tasks;
using AELP.Models;

namespace AELP.Services;

/// <summary>
/// 提供词典查询能力，包括单词详情、翻译和分页检索。
/// </summary>
public interface IWordQueryService
{
    /// <summary>
    /// 同步查询单词的完整词典信息。
    /// </summary>
    /// <param name="word">要查询的单词。</param>
    /// <returns>匹配到的词典条目；未找到时返回 <see langword="null"/>。</returns>
    public Dictionary? QueryWordInfo(string word);

    /// <summary>
    /// 异步查询单词的完整词典信息。
    /// </summary>
    /// <param name="word">要查询的单词。</param>
    /// <returns>匹配到的词典条目；未找到时返回 <see langword="null"/>。</returns>
    public Task<Dictionary?> QueryWordInfoAsync(string word);

    /// <summary>
    /// 根据单词查询其译文文本。
    /// </summary>
    /// <param name="word">要查询的单词。</param>
    /// <returns>译文字符串；若无结果通常返回空字符串。</returns>
    public string QueryWordTranslation(string word);

    /// <summary>
    /// 根据译文反查一个单词。
    /// </summary>
    /// <param name="translation">译文关键字。</param>
    /// <returns>匹配到的单词文本；若无结果通常返回空字符串。</returns>
    public string QueryWord(string translation);

    /// <summary>
    /// 根据译文关键字同步查询多个词典条目。
    /// </summary>
    /// <param name="translation">译文关键字。</param>
    /// <returns>匹配到的词典条目列表。</returns>
    public List<Dictionary> QueryWords(string translation);
    // public Task<List<dictionary>> QueryWordsAsync(string translation);

    /// <summary>
    /// 根据译文关键字异步查询词典条目。
    /// </summary>
    /// <param name="translation">译文关键字。</param>
    /// <returns>匹配到的词典条目数组。</returns>
    public Task<Dictionary[]> QueryWordsAsync(string translation);

    /// <summary>
    /// 根据译文关键字异步分页查询词典条目。
    /// </summary>
    /// <param name="translation">译文关键字。</param>
    /// <param name="skip">跳过的记录数。</param>
    /// <param name="take">获取的记录数。</param>
    /// <returns>匹配到的词典条目数组。</returns>
    public Task<Dictionary[]> QueryWordsAsync(string translation,int skip,int take);

}