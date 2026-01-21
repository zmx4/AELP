using AELP.Data;
using AELP.Factories;
using CommunityToolkit.Mvvm.Input;

namespace AELP.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private PageFactory _pageFactory;

    private ViewModelBase _content;

    public MainWindowViewModel(PageFactory pageFactory)
    {
        _pageFactory = pageFactory;
        
    }

    public ViewModelBase Content
    {
        get => _content;
        private set => SetProperty(ref _content, value);
    }

    [RelayCommand]
    private void GoToDictionaryPage()
    {
        Content = _pageFactory.GetPageViewModel(ApplicationPageNames.Dictionary);
    }

    [RelayCommand]
    private void GoToFavoritesPage()
    {
        Content = _pageFactory.GetPageViewModel(ApplicationPageNames.Favorites);
    }

    [RelayCommand]
    private void GoToTestsPage()
    {
        Content = _pageFactory.GetPageViewModel(ApplicationPageNames.Tests);
    }

    [RelayCommand]
    private void GoToMistakePage()
    {
        Content = _pageFactory.GetPageViewModel(ApplicationPageNames.Mistakes);
    }

    [RelayCommand]
    private void GoToSettingsPage()
    {
        Content = _pageFactory.GetPageViewModel(ApplicationPageNames.Settings);
    }
}