using System.Collections.ObjectModel;
using AELP.Data;
using AELP.Factories;
using AELP.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AELP.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly PageFactory _pageFactory;

    private ViewModelBase _content;
    
    private readonly ObservableCollection<ViewModelBase> _pages = new();
    
    [ObservableProperty]
    private bool _isBackButtonVisible;

    [ObservableProperty]
    private bool _isSidebarExpanded = true;

    public MainWindowViewModel(PageFactory pageFactory)
    {
        _pageFactory = pageFactory;

        _content = _pageFactory.GetPageViewModel(ApplicationPageNames.Dictionary);

        WeakReferenceMessenger.Default.Register<NavigationMessage>(this, (r, m) =>
        {
            var page = _pageFactory.GetPageViewModel(m.Value.PageName, m.Value.Parameter);
            IsBackButtonVisible = true;
            PushContent(page);
        });
    }

    
    
    public ViewModelBase Content
    {
        get => _content;
        private set => SetProperty(ref _content, value);
    }

    public double SidebarWidth => IsSidebarExpanded ? 120 : 70;

    private void PushContent(ViewModelBase page)
    {
        _pages.Add(Content);
        
        Content = page;
    }

    partial void OnIsSidebarExpandedChanged(bool value)
    {
        OnPropertyChanged(nameof(SidebarWidth));
    }

    [RelayCommand]
    private void ToggleSidebar()
    {
        IsSidebarExpanded = !IsSidebarExpanded;
    }

    [RelayCommand]
    private void GoToDictionaryPage()
    {
        Content = _pageFactory.GetPageViewModel(ApplicationPageNames.Dictionary);
        _pages.Clear();
        IsBackButtonVisible = false;
    }

    [RelayCommand]
    private void GoToFavoritesPage()
    {
        Content = _pageFactory.GetPageViewModel(ApplicationPageNames.Favorites);
        _pages.Clear();
        IsBackButtonVisible = false;
    }

    [RelayCommand]
    private void GoToTestsPage()
    {
        Content = _pageFactory.GetPageViewModel(ApplicationPageNames.Tests);
        _pages.Clear();
        IsBackButtonVisible = false;
    }

    [RelayCommand]
    private void GoToMistakePage()
    {
        Content = _pageFactory.GetPageViewModel(ApplicationPageNames.Mistakes);
        _pages.Clear();
        IsBackButtonVisible = false;
    }

    [RelayCommand]
    private void GoToSettingsPage()
    {
        Content = _pageFactory.GetPageViewModel(ApplicationPageNames.Settings);
        _pages.Clear();
        IsBackButtonVisible = false;
    }
    
    [RelayCommand]
    private void GoBack()
    {
        if(_pages.Count <= 0) return;
        Content = _pages[^1];
        _pages.RemoveAt(_pages.Count - 1);
        if(_pages.Count <= 0)IsBackButtonVisible = false;
    }
}