using AELP.Models;
using AELP.Services;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace AELP.UnitTest.Services;

[TestSubject(typeof(IWordQueryService))]
public class WordQueryServiceTest
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=../../../../AELP/Assets/Database/stardict.db")
            .Options;
        var context = new AppDbContext(options);
        // var context = new AppDbContext();
        context.Database.OpenConnection();
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task QueryWordInfoTest()
    {
        await using var context = GetDbContext();
        
        var service = new WordQueryService(context);

        var word = await service.QueryWordInfoAsync("we");
        
        Assert.NotNull(word);
    }

    [Fact]
    public async Task QueryWordTranslationTest()
    {
        await using var context = GetDbContext();

        var service = new WordQueryService(context);

        var translation = await service.QueryWordsAsync("人");

        Assert.NotNull(translation);
        Assert.NotEmpty(translation);
    }
}