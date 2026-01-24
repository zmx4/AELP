using System.Threading.Tasks;
using AELP.Data;

namespace AELP.Services;

public class UserWordQueryService : IUserWordQueryService
{
    public async Task<WordDataModel?> QueryUserWordInfoAsync(int wordId)
    {
        await using var dbContext = new UserDbContext();
        var wordData = await dbContext.Words.FindAsync(wordId);
        return wordData;
    }
}