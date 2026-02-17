using AELP.Data;
using AELP.Helper;
using AELP.UnitTest.Helper;
using Microsoft.EntityFrameworkCore;

namespace AELP.UnitTest.Factories;

public class TestUserDbContext
{
    public static IDbContextFactory<UserDbContext> CreateContextFactory()
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseSqlite($"Data Source={PathHelper.GetLocalFilePath("test"+UserDbContext.DbName)}")
            .Options;
        return new TestDbContextFactory<UserDbContext>(options);
    }
}