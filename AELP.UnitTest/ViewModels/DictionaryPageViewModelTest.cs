using System.Collections.ObjectModel;
using AELP.Services;
using AELP.ViewModels;
using AELP.Models;
using AELP.Messages;
using CommunityToolkit.Mvvm.Messaging;
using Moq;
using Xunit;

namespace AELP.UnitTest.ViewModels;

public class DictionaryPageViewModelTest
{
    private readonly Mock<IWordQueryService> _mockWordQueryService;
    private readonly Mock<IFavoritesDataStorageService> _mockFavoritesService;
    private readonly DictionaryPageViewModel _viewModel;

    public DictionaryPageViewModelTest()
    {
        _mockWordQueryService = new Mock<IWordQueryService>();
        _mockFavoritesService = new Mock<IFavoritesDataStorageService>();
        
        _viewModel = new DictionaryPageViewModel(
            _mockWordQueryService.Object, 
            _mockFavoritesService.Object
        );
    }

    [Fact]
    public void Constructor_InitializesProperties()
    {
        Assert.Equal(0, _viewModel.ContentBlurRadius);
        Assert.Empty(_viewModel.ExamTags);
        Assert.Empty(_viewModel.SearchResults);
        Assert.Empty(_viewModel.SearchText);
        Assert.Empty(_viewModel.SearchResult);
    }

    [Fact]
    public async Task SearchTranslationAsync_EmptyText_BlursContent()
    {
        _viewModel.SearchText = "   ";
        await _viewModel.SearchTranslationCommand.ExecuteAsync(null);
        
        Assert.Equal(20, _viewModel.ContentBlurRadius);
        _mockWordQueryService.Verify(s => s.QueryWordInfoAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SearchTranslationAsync_WordFound_UpdatesResultAndTags()
    {
        var word = "test";
        var dictionary = new Dictionary 
        { 
            RawWord = word, 
            Translation = "测试\\n试验",
            Cet4 = 1,
            Cet6 = 0,
            Hs = 1
        };

        _mockWordQueryService
            .Setup(s => s.QueryWordInfoAsync(word))
            .ReturnsAsync(dictionary);

        _viewModel.SearchText = word;
        await _viewModel.SearchTranslationCommand.ExecuteAsync(null);

        Assert.Equal("测试\n试验", _viewModel.SearchResult);
        Assert.Contains("CET4", _viewModel.ExamTags);
        Assert.Contains("High School", _viewModel.ExamTags);
        Assert.DoesNotContain("CET6", _viewModel.ExamTags);
        Assert.Equal(0, _viewModel.ContentBlurRadius);
    }

    [Fact]
    public async Task SearchTranslationAsync_WordFound_AllTags()
    {
        var word = "comprehensive";
        var dictionary = new Dictionary 
        { 
            RawWord = word,
            Translation = "translation", 
            Cet4 = 1, Cet6 = 1, Hs = 1, Ph = 1, Tf = 1, Ys = 1
        };

        _mockWordQueryService.Setup(s => s.QueryWordInfoAsync(word)).ReturnsAsync(dictionary);
        _viewModel.SearchText = word;

        await _viewModel.SearchTranslationCommand.ExecuteAsync(null);

        Assert.Contains("CET4", _viewModel.ExamTags);
        Assert.Contains("CET6", _viewModel.ExamTags);
        Assert.Contains("High School", _viewModel.ExamTags);
        Assert.Contains("Primary School", _viewModel.ExamTags);
        Assert.Contains("TOEFL", _viewModel.ExamTags);
        Assert.Contains("IELTS", _viewModel.ExamTags);
    }

    [Fact]
    public async Task SearchTranslationAsync_NotFound_SetsNotFoundMessage()
    {
        var word = "unknown";
        _mockWordQueryService.Setup(s => s.QueryWordInfoAsync(word)).ReturnsAsync((Dictionary?)null);

        _viewModel.SearchText = word;
        await _viewModel.SearchTranslationCommand.ExecuteAsync(null);

        Assert.Contains("未找到", _viewModel.SearchResult); // Checking for partial message match
        Assert.Empty(_viewModel.ExamTags);
    }

    [Fact]
    public async Task SearchWordsAsync_PopulatesSearchResults()
    {
        var searchText = "te";
        var results = new List<Dictionary>
        {
            new Dictionary { RawWord = "test" },
            new Dictionary { RawWord = "team" }
        };

        _mockWordQueryService
            .Setup(s => s.QueryWordsAsync(searchText))
            .ReturnsAsync(results.ToArray());

        _viewModel.SearchText = searchText;
        await _viewModel.SearchWordsCommand.ExecuteAsync(null);

        Assert.Equal(2, _viewModel.SearchResults.Count);
        Assert.Contains("test", _viewModel.SearchResults);
        Assert.Contains("team", _viewModel.SearchResults);
    }

    [Fact]
    public async Task SearchWordsAsync_EmptyText_DoesNothing()
    {
        _viewModel.SearchText = "";
        await _viewModel.SearchWordsCommand.ExecuteAsync(null);
        
        _mockWordQueryService.Verify(s => s.QueryWordsAsync(It.IsAny<string>()), Times.Never);
        Assert.Empty(_viewModel.SearchResults);
    }
    
    [Fact]
    public async Task AddToFavoritesAsync_WithCurrentWord_CallsService()
    {
        // Setup initial search to set _word
        var word = "fav";
        var dictionary = new Dictionary { RawWord = word, Translation = "favorite" };
        _mockWordQueryService.Setup(s => s.QueryWordInfoAsync(word)).ReturnsAsync(dictionary);
        
        _viewModel.SearchText = word;
        await _viewModel.SearchTranslationCommand.ExecuteAsync(null);

        // Act
        await _viewModel.AddToFavoritesCommand.ExecuteAsync(null);

        // Assert
        _mockFavoritesService.Verify(s => s.AddToFavorites(dictionary), Times.Once);
    }

    [Fact]
    public async Task AddToFavoritesAsync_NoWordSelected_DoesNotCallService()
    {
        await _viewModel.AddToFavoritesCommand.ExecuteAsync(null);
        
        _mockFavoritesService.Verify(s => s.AddToFavorites(It.IsAny<Dictionary>()), Times.Once);
    }

    [Fact]
    public async Task OpenDetailCommand_NavigatesToDetail_WhenWordExists()
    {
        var word = "detail";
        var dictionary = new Dictionary { RawWord = word, Translation = "detail translation" };
        var results = new List<Dictionary> { dictionary };

        // Pre-populate _rawSearchResults by running SearchWords
        _mockWordQueryService.Setup(s => s.QueryWordsAsync(word)).ReturnsAsync(results.ToArray());
        _viewModel.SearchText = word;
        await _viewModel.SearchWordsCommand.ExecuteAsync(null);

        // Capture navigation message
        NavigationMessage? receivedMessage = null;
        WeakReferenceMessenger.Default.Register<NavigationMessage>(this, (r, m) =>
        {
            receivedMessage = m;
        });

        // Act
        _viewModel.OpenDetailCommand.Execute(word);

        // Assert
        Assert.NotNull(receivedMessage);
        Assert.Equal(Data.ApplicationPageNames.Detail, receivedMessage.Value.PageName);
        Assert.Equal(dictionary, receivedMessage.Value.Parameter);

        // Cleanup
        WeakReferenceMessenger.Default.Unregister<NavigationMessage>(this);
    }
}