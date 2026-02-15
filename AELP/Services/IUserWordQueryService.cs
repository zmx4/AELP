using System.Threading.Tasks;
using AELP.Data;

namespace AELP.Services;

/// <summary>
/// 提供用户词汇数据查询能力。
/// </summary>
public interface IUserWordQueryService
{
    /// <summary>
    /// 根据单词 ID 异步查询用户侧单词信息。
    /// </summary>
    /// <param name="wordId">单词 ID。</param>
    /// <returns>查询到的用户单词信息；未找到时返回 <see langword="null"/>。</returns>
    public Task<WordDataModel?> QueryUserWordInfoAsync(int wordId);
}