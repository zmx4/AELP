using AELP.Data;
using AELP.Models;
using AELP.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;

namespace AELP.UnitTest.Services;

public class TestWordGetterTest
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;
        var context = new AppDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();
        return context;
    }

    [Theory]
    [InlineData(TestRange.Cet4)]
    [InlineData(TestRange.Cet6)]
    [InlineData(TestRange.Senior)]
    [InlineData(TestRange.Toefl)]
    [InlineData(TestRange.Ielts)]
    [InlineData(TestRange.Primary)]
    public async Task GetTestWords_ReturnsCorrectNumberOfWords(TestRange range)
    {
        // Arrange
        await using var context = GetDbContext();

        switch (range)
        {
            // Seed data
            case TestRange.Cet4:
            {
                for(var i=0; i<10; i++) context.CET4s.Add(new CET4 { word = $"c4_{i}", translation = "t" });
                break;
            }
            case TestRange.Cet6:
            {
                for(var i=0; i<10; i++) context.CET6s.Add(new CET6 { word = $"c6_{i}", translation = "t" });
                break;
            }
            case TestRange.Senior:
            {
                for(var i=0; i<10; i++) context.HighSchools.Add(new HighSchool { word = $"hs_{i}", translation = "t" });
                break;
            }
            case TestRange.Toefl:
            {
                for(var i=0; i<10; i++) context.tfs.Add(new tf { word = $"tf_{i}", translation = "t" });
                break;
            }
            case TestRange.Ielts:
            {
                for(var i=0; i<10; i++) context.ys.Add(new y { word = $"ys_{i}", translation = "t" });
                break;
            }
            case TestRange.Primary:
            {
                for(var i=0; i<10; i++) context.PrimarySchools.Add(new PrimarySchool { word = $"ps_{i}", translation = "t" });
                break;
            }
        }

        await context.SaveChangesAsync();

        var service = new TestWordGetter(context);

        // Act
        const int count = 5;
        var result = await service.GetTestWords(count, range);

        // Assert
        Assert.Equal(count, result.Count);
        Assert.All(result, w => Assert.NotNull(w.word));
        Assert.All(result, w => Assert.NotEmpty(w.word));
    }
}