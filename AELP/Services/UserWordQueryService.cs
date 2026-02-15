using System.Threading.Tasks;
using AELP.Data;
using Microsoft.EntityFrameworkCore;

namespace AELP.Services;

/// <summary>
/// 用户词汇查询服务，负责查询用户词表中的单词数据。
/// </summary>
public class UserWordQueryService(IDbContextFactory<UserDbContext> contextFactory) : IUserWordQueryService
{
    /// <inheritdoc />
    public async Task<WordDataModel?> QueryUserWordInfoAsync(int wordId)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync();
        var wordData = await dbContext.Words.FindAsync(wordId);
        return wordData;
    }
}