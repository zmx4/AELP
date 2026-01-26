using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AELP.Data;
using AELP.Models;
using AELP.Services;
using CommunityToolkit.Mvvm.Input;

namespace AELP.ViewModels;

public partial class DetailPageViewModel : PageViewModel
{
    private readonly IFavoritesDataStorageService _dataStorageService;
    
    private dictionary? _word;

    public dictionary? Word
    {
        get => _word;
        private set
        {
            if (SetProperty(ref _word, value))
            {
                UpdateTags();
            }
        }
    }

    public ObservableCollection<string> Tags { get; } = new();

    public DetailPageViewModel(IFavoritesDataStorageService dataStorageService)
    {
        PageNames = ApplicationPageNames.Detail;
        _dataStorageService = dataStorageService;
    }

    public override void SetParameter(object parameter)
    {
        if (parameter is dictionary word)
        {
            Word = word;
            word.translation = word.translation?.Replace("\\n", "\n");
        }
    }

    private void UpdateTags()
    {
        Tags.Clear();
        if (Word is null) return;

        if (Word.cet4 > 0) Tags.Add("CET4");
        if (Word.cet6 > 0) Tags.Add("CET6");
        if (Word.hs > 0) Tags.Add("高中");
        if (Word.ph > 0) Tags.Add("小学");
        if (Word.tf > 0) Tags.Add("托福");
        if (Word.ys > 0) Tags.Add("雅思");
    }
    [RelayCommand]
    private async Task AddToFavoritesAsync()
    {
        if (Word is null) return;

        await _dataStorageService.AddToFavorites(Word);
    }
}