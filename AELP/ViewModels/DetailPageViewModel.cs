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
    
    private Dictionary? _word;

    public Dictionary? Word
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
        if (parameter is Dictionary word)
        {
            Word = word;
            word.Translation = word.Translation?.Replace("\\n", "\n");
        }
    }

    private void UpdateTags()
    {
        Tags.Clear();
        if (Word is null) return;

        if (Word.Cet4 > 0) Tags.Add("CET4");
        if (Word.Cet6 > 0) Tags.Add("CET6");
        if (Word.Hs > 0) Tags.Add("高中");
        if (Word.Ph > 0) Tags.Add("小学");
        if (Word.Tf > 0) Tags.Add("托福");
        if (Word.Ys > 0) Tags.Add("雅思");
    }
    [RelayCommand]
    private async Task AddToFavoritesAsync()
    {
        if (Word is null) return;

        await _dataStorageService.AddToFavorites(Word);
    }
}