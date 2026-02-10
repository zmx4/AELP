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
                for(var i=0; i<10; i++) context.Cet4S.Add(new Cet4 { RawWord = $"c4_{i}", Translation = "t" });
                break;
            }
            case TestRange.Cet6:
            {
                for(var i=0; i<10; i++) context.Cet6S.Add(new Cet6 { RawWord = $"c6_{i}", Translation = "t" });
                break;
            }
            case TestRange.Senior:
            {
                for(var i=0; i<10; i++) context.HighSchools.Add(new HighSchool { RawWord = $"hs_{i}", Translation = "t" });
                break;
            }
            case TestRange.Toefl:
            {
                for(var i=0; i<10; i++) context.Tfs.Add(new Tf { RawWord = $"tf_{i}", Translation = "t" });
                break;
            }
            case TestRange.Ielts:
            {
                for(var i=0; i<10; i++) context.Ys.Add(new Y { RawWord = $"ys_{i}", Translation = "t" });
                break;
            }
            case TestRange.Primary:
            {
                for(var i=0; i<10; i++) context.PrimarySchools.Add(new PrimarySchool { RawWord = $"ps_{i}", Translation = "t" });
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
        Assert.All(result, w => Assert.NotNull(w.RawWord));
        Assert.All(result, w => Assert.NotEmpty(w.RawWord));
    }
}