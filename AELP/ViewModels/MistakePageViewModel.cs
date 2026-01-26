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

    [RelayCommand]
    private void GoToReview()
    {
        if (Items.Count == 0) return;

        WeakReferenceMessenger.Default.Send(
            new NavigationMessage(ApplicationPageNames.MistakeReview, Items.ToArray()));
    }
}