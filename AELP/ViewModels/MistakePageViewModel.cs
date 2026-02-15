using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AELP.Data;
using AELP.Messages;
using AELP.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AELP.ViewModels;

/// <summary>
/// 错题页面视图模型，提供错题展示、筛选排序与复习入口。
/// </summary>
public partial class MistakePageViewModel : PageViewModel
{
    [ObservableProperty]
    private ObservableCollection<MistakeDataModel> _items;

    [ObservableProperty]
    private SortOption? _selectedSortOption;

    [ObservableProperty]
    private int _maxTestCount = 20;

    [ObservableProperty]
    private bool _showMasteredWords = true;
    
    private readonly IMistakeDataStorageService _mistakeDataStorageService;
    private readonly List<MistakeDataModel> _allMistakes = new();

    /// <summary>
    /// 排序选项列表。
    /// </summary>
    public IReadOnlyList<SortOption> SortOptions { get; } =
    [
        new SortOption("按犯错时间（近到远）", MistakeSortOption.TimeDesc),
        new SortOption("按英文字母顺序", MistakeSortOption.WordAsc),
        new SortOption("按犯错次数（多到少）", MistakeSortOption.CountDesc)
    ];
    /// <summary>
    /// 初始化 <see cref="MistakePageViewModel"/>。
    /// </summary>
    /// <param name="mistakeDataStorageService">错题存储服务。</param>
    public MistakePageViewModel(IMistakeDataStorageService mistakeDataStorageService)
    {
        _items = [];
        PageNames = ApplicationPageNames.Mistakes;
        _mistakeDataStorageService = mistakeDataStorageService;

        SelectedSortOption = SortOptions[0];
        
        LoadMistakesAsync().ConfigureAwait(false);
    }
    
    private async Task LoadMistakesAsync()
    {
        var mistakes = await _mistakeDataStorageService.LoadMistakeData();
        foreach (var mistake in mistakes)
        {
            mistake.Translation = NormalizeTranslation(mistake.Translation);
        }
        _allMistakes.Clear();
        _allMistakes.AddRange(mistakes);
        ApplySort();
    }

    partial void OnSelectedSortOptionChanged(SortOption? value)
    {
        ApplySort();
    }

    partial void OnShowMasteredWordsChanged(bool value)
    {
        ApplySort();
    }

    private void ApplySort()
    {
        var filtered = ShowMasteredWords 
            ? _allMistakes 
            : _allMistakes.Where(x => !x.IsMastered);
            
        var filteredList = filtered.ToList();

        if (filteredList.Count == 0)
        {
            Items = new ObservableCollection<MistakeDataModel>();
            return;
        }

        var option = SelectedSortOption?.Value ?? MistakeSortOption.TimeDesc;
        IEnumerable<MistakeDataModel> ordered = option switch
        {
            MistakeSortOption.WordAsc => filteredList
                .OrderBy(x => x.Word ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenByDescending(x => x.Time),
            MistakeSortOption.TimeDesc => filteredList
                .OrderByDescending(x => x.Time)
                .ThenBy(x => x.Word ?? string.Empty, StringComparer.OrdinalIgnoreCase),
            _ => filteredList
                .OrderByDescending(x => x.Count)
                .ThenByDescending(x => x.Time),
        };

        Items = new ObservableCollection<MistakeDataModel>(ordered);
    }

    private static string? NormalizeTranslation(string? translation)
    {
        return translation?
            .Replace("\\r\\n", "\n", StringComparison.Ordinal)
            .Replace("\\n", "\n", StringComparison.Ordinal);
    }

    /// <summary>
    /// 错题排序项。
    /// </summary>
    public sealed record SortOption(string Display, MistakeSortOption Value);

    /// <summary>
    /// 错题排序方式。
    /// </summary>
    public enum MistakeSortOption
    {
        /// <summary>按时间降序。</summary>
        TimeDesc,
        /// <summary>按单词字母升序。</summary>
        WordAsc,
        /// <summary>按错误次数降序。</summary>
        CountDesc
    }

    /// <summary>
    /// 进入错题复习流程。
    /// </summary>
    [RelayCommand]
    private void GoToReview()
    {
        if (Items.Count == 0) return;

        var shuffledItems = Shuffle(Items.Where(x=>x.Count > 0).ToArray());

        if (MaxTestCount > 0 && MaxTestCount < shuffledItems.Length)
        {
            shuffledItems = shuffledItems.Take(MaxTestCount).ToArray();
        }

        WeakReferenceMessenger.Default.Send(
            new NavigationMessage(ApplicationPageNames.MistakeReview, shuffledItems));
    }

    private static MistakeDataModel[] Shuffle(MistakeDataModel[] source)
    {
        var rng = Random.Shared;
        for (var i = source.Length - 1; i > 0; i--)
        {
            var j = rng.Next(i + 1);
            (source[i], source[j]) = (source[j], source[i]);
        }

        return source;
    }
}