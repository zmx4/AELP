using System.Threading.Tasks;
using AELP.Data;
using Microsoft.EntityFrameworkCore;

namespace AELP.Services;

public class UserWordQueryService(IDbContextFactory<UserDbContext> contextFactory) : IUserWordQueryService
{
    public async Task<WordDataModel?> QueryUserWordInfoAsync(int wordId)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync();
        var wordData = await dbContext.Words.FindAsync(wordId);
        return wordData;
    }
}