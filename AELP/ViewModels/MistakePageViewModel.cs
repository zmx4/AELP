using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AELP.Data;
using AELP.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AELP.ViewModels;

public partial class MistakePageViewModel : PageViewModel
{
    [ObservableProperty]
    private ObservableCollection<MistakeDataModel> _items;
    
    private readonly IMistakeDataStorageService _mistakeDataStorageService;
    public MistakePageViewModel(IMistakeDataStorageService mistakeDataStorageService)
    {
        _items = [];
        PageNames = Data.ApplicationPageNames.Mistakes;
        _mistakeDataStorageService = mistakeDataStorageService;
        
        LoadMistakesAsync().ConfigureAwait(false);
    }
    
    private async Task LoadMistakesAsync()
    {
        var mistakes = await _mistakeDataStorageService.LoadMistakeData();
        Items = new ObservableCollection<MistakeDataModel>(mistakes);
    }
}