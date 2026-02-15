using System.Linq;
using System.Threading.Tasks;
using AELP.Data;
using Microsoft.EntityFrameworkCore;

namespace AELP.Services;

/// <summary>
/// 测试记录存储服务，负责测试数据持久化与查询。
/// </summary>
public class TestDataStorageService(IDbContextFactory<UserDbContext> contextFactory) : ITestDataStorageService
{
    /// <inheritdoc />
    public async Task SaveTestData(TestDataModel[] testData)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        await context.Database.EnsureCreatedAsync();
        
        foreach (var test in testData)
        {
            if (test.Id == 0)
            {
                await context.Tests.AddAsync(test);
            }
            else
            {
                context.Tests.Update(test);
            }
        }
        await context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<TestDataModel[]> LoadTestData()
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        await context.Database.EnsureCreatedAsync();
        return await context.Tests.ToArrayAsync();
    }

    /// <inheritdoc />
    public async Task<TestDataModel[]> GetRecentTests(int count = 10)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        await context.Database.EnsureCreatedAsync();
        return await context.Tests.OrderByDescending(t => t.TestTime).Take(count).ToArrayAsync();
    }
}