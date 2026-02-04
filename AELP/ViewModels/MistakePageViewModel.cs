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

public partial class MistakePageViewModel : PageViewModel
{
    [ObservableProperty]
    private ObservableCollection<MistakeDataModel> _items;

    [ObservableProperty]
    private SortOption? _selectedSortOption;
    
    private readonly IMistakeDataStorageService _mistakeDataStorageService;
    private readonly List<MistakeDataModel> _allMistakes = new();

    public IReadOnlyList<SortOption> SortOptions { get; } =
    [
        new SortOption("按犯错时间（近到远）", MistakeSortOption.TimeDesc),
        new SortOption("按英文字母顺序", MistakeSortOption.WordAsc),
        new SortOption("按犯错次数（多到少）", MistakeSortOption.CountDesc)
    ];
    public MistakePageViewModel(IMistakeDataStorageService mistakeDataStorageService)
    {
        _items = [];
        PageNames = Data.ApplicationPageNames.Mistakes;
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

    private void ApplySort()
    {
        if (_allMistakes.Count == 0)
        {
            Items = new ObservableCollection<MistakeDataModel>();
            return;
        }

        var option = SelectedSortOption?.Value ?? MistakeSortOption.TimeDesc;
        IEnumerable<MistakeDataModel> ordered = option switch
        {
            MistakeSortOption.WordAsc => _allMistakes
                .OrderBy(x => x.Word ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenByDescending(x => x.Time),
            MistakeSortOption.TimeDesc => _allMistakes
                .OrderByDescending(x => x.Time)
                .ThenBy(x => x.Word ?? string.Empty, StringComparer.OrdinalIgnoreCase),
            _ => _allMistakes
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

    public sealed record SortOption(string Display, MistakeSortOption Value);

    public enum MistakeSortOption
    {
        TimeDesc,
        WordAsc,
        CountDesc
    }

    [RelayCommand]
    private void GoToReview()
    {
        if (Items.Count == 0) return;

        var shuffledItems = Shuffle(Items.ToArray());
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