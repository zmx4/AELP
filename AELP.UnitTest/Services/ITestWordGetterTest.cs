using AELP.Data;
using AELP.Models;
using AELP.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;

namespace AELP.UnitTest.Services;

public class ITestWordGetterTest
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
        using var context = GetDbContext();
        
        // Seed data
        if (range == TestRange.Cet4) 
            for(int i=0; i<10; i++) context.CET4s.Add(new CET4 { word = $"c4_{i}", translation = "t" });
        if (range == TestRange.Cet6) 
            for(int i=0; i<10; i++) context.CET6s.Add(new CET6 { word = $"c6_{i}", translation = "t" });
        if (range == TestRange.Senior) 
            for(int i=0; i<10; i++) context.HighSchools.Add(new HighSchool { word = $"hs_{i}", translation = "t" });
        if (range == TestRange.Toefl) 
            for(int i=0; i<10; i++) context.tfs.Add(new tf { word = $"tf_{i}", translation = "t" });
        if (range == TestRange.Ielts) 
            for(int i=0; i<10; i++) context.ys.Add(new y { word = $"ys_{i}", translation = "t" });
        if (range == TestRange.Primary) 
            for(int i=0; i<10; i++) context.PrimarySchools.Add(new PrimarySchool { word = $"ps_{i}", translation = "t" });

        await context.SaveChangesAsync();

        var service = new TestWordGetter(context);

        // Act
        var count = 5;
        var result = await service.GetTestWords(count, range);

        // Assert
        Assert.Equal(count, result.Count);
        Assert.All(result, w => Assert.NotNull(w.word));
        Assert.All(result, w => Assert.NotEmpty(w.word));
    }
}