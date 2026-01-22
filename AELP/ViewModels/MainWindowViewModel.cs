using System.Collections.ObjectModel;
using AELP.Data;
using AELP.Factories;
using CommunityToolkit.Mvvm.Input;

namespace AELP.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private PageFactory _pageFactory;

    private ViewModelBase _content;
    
    private ObservableCollection<ViewModelBase> _pages = new();

    public MainWindowViewModel(PageFactory pageFactory)
    {
        _pageFactory = pageFactory;

        _content = _pageFactory.GetPageViewModel(ApplicationPageNames.Dictionary);
    }

    public ViewModelBase Content
    {
        get => _content;
        private set => SetProperty(ref _content, value);
    }
    
    public void PushContent(ViewModelBase page)
    {
        _pages.Add(Content);
        
        Content = page;
    }

    [RelayCommand]
    private void GoToDictionaryPage()
    {
        Content = _pageFactory.GetPageViewModel(ApplicationPageNames.Dictionary);
        _pages.Clear();
    }

    [RelayCommand]
    private void GoToFavoritesPage()
    {
        Content = _pageFactory.GetPageViewModel(ApplicationPageNames.Favorites);
        _pages.Clear();
    }

    [RelayCommand]
    private void GoToTestsPage()
    {
        Content = _pageFactory.GetPageViewModel(ApplicationPageNames.Tests);
        _pages.Clear();
    }

    [RelayCommand]
    private void GoToMistakePage()
    {
        Content = _pageFactory.GetPageViewModel(ApplicationPageNames.Mistakes);
        _pages.Clear();
    }

    [RelayCommand]
    private void GoToSettingsPage()
    {
        Content = _pageFactory.GetPageViewModel(ApplicationPageNames.Settings);
        _pages.Clear();
    }
    
    [RelayCommand]
    private void GoBack()
    {
        if(_pages.Count <= 0) return;
        
        Content = _pages[^1];
        _pages.RemoveAt(_pages.Count - 1);
    }
}