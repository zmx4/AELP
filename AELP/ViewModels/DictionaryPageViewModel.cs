using System.Collections.Generic;
using AELP.Services;
using AELP.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AELP.Models;

namespace AELP.ViewModels;

/// <summary>
/// 词典页面视图模型，负责单词查询、候选列表与收藏操作。
/// </summary>
public partial class DictionaryPageViewModel : PageViewModel
{
    private readonly IWordQueryService _wordQueryService;

    private readonly IFavoritesDataStorageService _favoritesDataStorageService;

    [ObservableProperty] private string _searchText = string.Empty;

    [ObservableProperty] private string _searchResult = string.Empty;

    [ObservableProperty] private ObservableCollection<string> _examTags = new();

    [ObservableProperty] private double _contentBlurRadius;

    [ObservableProperty] private ObservableCollection<string> _searchResults = new();

    private List<Dictionary> _rawSearchResults = new();

    private Dictionary _word;

    /// <summary>
    /// 初始化 <see cref="DictionaryPageViewModel"/>。
    /// </summary>
    /// <param name="wordQueryService">单词查询服务。</param>
    /// <param name="favoritesDataStorageService">收藏存储服务。</param>
    public DictionaryPageViewModel(IWordQueryService wordQueryService,
        IFavoritesDataStorageService favoritesDataStorageService)
    {
        PageNames = Data.ApplicationPageNames.Dictionary;
        _wordQueryService = wordQueryService;
        // Apply blur initially as no result is found yet? 
        // Or start clear? User said "when not found... blur".
        // Let's assume start with blur if "not found" is the default state after a failed search.
        // But initially it's empty.
        _favoritesDataStorageService = favoritesDataStorageService;
        ContentBlurRadius = 0;
        _word = new Dictionary();
        // _word = new dictionary();
    }

    /// <summary>
    /// 根据输入单词查询译文。
    /// </summary>
    /// <returns>表示查询完成的异步任务。</returns>
    [RelayCommand]
    private async Task SearchTranslationAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            ContentBlurRadius = 20;
            return;
        }

        var resultDictionary = await _wordQueryService.QueryWordInfoAsync(SearchText);

        if (resultDictionary != null)
        {
            _word = resultDictionary;
            // 搜出的文本包含\n, 需要处理一下
            // SearchResult = resultDictionary.translation ?? string.Empty;
            SearchResult = resultDictionary.Translation?.Replace("\\n", "\n") ?? string.Empty;
            ExamTags.Clear();
            if (resultDictionary.Cet4 == 1) ExamTags.Add("CET4");
            if (resultDictionary.Cet6 == 1) ExamTags.Add("CET6");
            if (resultDictionary.Hs == 1) ExamTags.Add("High School");
            if (resultDictionary.Ph == 1) ExamTags.Add("Primary School");
            if (resultDictionary.Tf == 1) ExamTags.Add("TOEFL");
            if (resultDictionary.Ys == 1) ExamTags.Add("IELTS");

            ContentBlurRadius = 0;
        }
        else
        {
            // Not found
            ContentBlurRadius = 20;
            // Optionally clear result or keep previous? 
            // If I clear result, blur is invisible. 
            // "Blur the content below". Maybe the user implies the content is still there?
            // I'll keep the previous result but blur it to indicate "not this one" or just invalid state.
            // Or maybe I should show "Not Found" and blur THAT? 
            // Let's set message to "Not Found" and blur it? That seems counter-intuitive (you want to read "Not Found").

            // Let's assume the user wants to obscure the area.
            SearchResult = "未找到该单词";
            ExamTags.Clear();
        }
    }

    /// <summary>
    /// 根据关键字查询单词候选列表。
    /// </summary>
    /// <returns>表示查询完成的异步任务。</returns>
    [RelayCommand]
    private async Task SearchWordsAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            return;
        }

        _rawSearchResults.Clear();
        _rawSearchResults = new List<Dictionary>(await _wordQueryService.QueryWordsAsync(SearchText));
        SearchResults.Clear();
        foreach (var word in _rawSearchResults)
        {
            SearchResults.Add(word.RawWord);
        }
    }

    /// <summary>
    /// 打开指定单词的详情页。
    /// </summary>
    /// <param name="word">单词文本。</param>
    [RelayCommand]
    private void OpenDetail(string word)
    {
        if (string.IsNullOrEmpty(word)) return;

        // var wordInfo = _wordQueryService.QueryWordInfo(word);
        var wordInfo = _rawSearchResults.Find(x => x.RawWord == word);
        if (wordInfo != null)
        {
            WeakReferenceMessenger.Default.Send(new NavigationMessage(Data.ApplicationPageNames.Detail, wordInfo));
        }
    }

    /// <summary>
    /// 将当前单词添加到收藏。
    /// </summary>
    /// <returns>表示添加完成的异步任务。</returns>
    [RelayCommand]
    private async Task AddToFavoritesAsync()
    {
        await _favoritesDataStorageService.AddToFavorites(_word);
    }

    /// <summary>
    /// 设置页面参数，触发单词查询。
    /// </summary>
    /// <param name="parameter">单词</param>
    public override void SetParameter(object parameter)
    {
        if (parameter is string parameterString)
        {
            SearchText = parameterString;
            _ = SearchTranslationAsync();
        }
    }
}