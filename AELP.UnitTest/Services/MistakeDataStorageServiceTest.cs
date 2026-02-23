using AELP.Data;
using AELP.Services;
using AELP.UnitTest.Helper;
using Moq;
using Microsoft.EntityFrameworkCore;

namespace AELP.UnitTest.Services;

public class MistakeDataStorageServiceTest : IDisposable
{
    private readonly string _dbPath;
    private readonly IDbContextFactory<UserDbContext> _contextFactory;

    public MistakeDataStorageServiceTest()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"aelp_mistake_test_{Guid.NewGuid():N}.db");
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseSqlite($"Data Source={_dbPath}")
            .Options;
        _contextFactory = new TestDbContextFactory<UserDbContext>(options);
    }

    public void Dispose()
    {
        using var context = _contextFactory.CreateDbContext();
        context.Database.EnsureDeleted();
        if (File.Exists(_dbPath))
        {
            File.Delete(_dbPath);
        }
    }

    [Fact]
    public async Task SaveMistakeData_MergesDuplicateWordsInSingleBatch()
    {
        var service = CreateService();

        await service.SaveMistakeData([
            new MistakeDataModel { Word = "apple", Count = 1, Time = new DateTime(2026, 2, 20) },
            new MistakeDataModel { Word = "apple", Count = 2, Time = new DateTime(2026, 2, 21) }
        ]);

        var mistakes = await service.LoadMistakeData();
        var appleMistakes = mistakes.Where(x => x.Word == "apple").ToArray();

        Assert.Single(appleMistakes);
        Assert.Equal(3, appleMistakes[0].Count);
        Assert.Equal(new DateTime(2026, 2, 21), appleMistakes[0].Time);
    }

    [Fact]
    public async Task SaveMistakeData_WhenWordExists_UpdatesCountInsteadOfAdding()
    {
        var service = CreateService();

        await service.SaveMistakeData([
            new MistakeDataModel { Word = "banana", Count = 1, Time = new DateTime(2026, 2, 20) }
        ]);

        await service.SaveMistakeData([
            new MistakeDataModel { Word = "banana", Count = 2, Time = new DateTime(2026, 2, 22) }
        ]);

        var mistakes = await service.LoadMistakeData();
        var bananaMistakes = mistakes.Where(x => x.Word == "banana").ToArray();

        Assert.Single(bananaMistakes);
        Assert.Equal(3, bananaMistakes[0].Count);
        Assert.Equal(new DateTime(2026, 2, 22), bananaMistakes[0].Time);
    }

    private MistakeDataStorageService CreateService()
    {
        var wordQueryService = new Mock<IWordQueryService>();
        wordQueryService
            .Setup(x => x.QueryWordTranslation(It.IsAny<string>()))
            .Returns(string.Empty);

        return new MistakeDataStorageService(wordQueryService.Object, _contextFactory);
    }
}
