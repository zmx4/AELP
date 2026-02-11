using AELP.Models;
using AELP.Services;
using AELP.UnitTest.Helper;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace AELP.UnitTest.Services;

[TestSubject(typeof(IWordQueryService))]
public class WordQueryServiceTest
{
    private static DbContextOptions<AppDbContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=../../../../AELP/Assets/Database/stardict.db")
            .Options;
    }

    [Fact]
    public async Task QueryWordInfoTest()
    {
        var factory = new TestDbContextFactory<AppDbContext>(CreateOptions());
        var service = new WordQueryService(factory);

        var word = await service.QueryWordInfoAsync("we");
        
        Assert.NotNull(word);
    }

    [Fact]
    public async Task QueryWordTranslationTest()
    {
        var factory = new TestDbContextFactory<AppDbContext>(CreateOptions());
        var service = new WordQueryService(factory);

        var translation = await service.QueryWordsAsync("人");

        Assert.NotNull(translation);
        Assert.NotEmpty(translation);
    }
}