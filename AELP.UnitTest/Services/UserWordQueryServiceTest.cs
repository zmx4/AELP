using AELP.Data;
using AELP.Services;
using AELP.UnitTest.Factories;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace AELP.UnitTest.Services;

[TestSubject(typeof(IUserWordQueryService))]
public class UserWordQueryServiceTest
{
    private static readonly IDbContextFactory<UserDbContext> ContextFactory = TestUserDbContext.CreateContextFactory();

    [Fact]
    public async Task QueryUserWordInfo_Failed()
    {
        var service = new UserWordQueryService(ContextFactory);
        var result = await service.QueryUserWordInfoAsync(9999);
        Assert.Null(result);
    }
}