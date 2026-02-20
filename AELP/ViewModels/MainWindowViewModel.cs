using System.Collections.ObjectModel;
using AELP.Data;
using AELP.Factories;
using AELP.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AELP.ViewModels;

/// <summary>
/// 主窗口视图模型，负责页面导航与侧边栏状态管理。
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    private readonly PageFactory _pageFactory;

    private ViewModelBase _content;
    
    private readonly ObservableCollection<ViewModelBase> _pages = new();
    
    [ObservableProperty]
    private bool _isBackButtonVisible;

    [ObservableProperty]
    private bool _isSidebarExpanded = true;

    /// <summary>
    /// 初始化 <see cref="MainWindowViewModel"/>。
    /// </summary>
    /// <param name="pageFactory">页面工厂。</param>
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

    /// <summary>
    /// 跳转至指定页面
    /// </summary>
    /// <param name="pageName">页面名称</param>
    public void GoTo(ApplicationPageNames pageName)
    {
        Content = _pageFactory.GetPageViewModel(pageName);
        _pages.Clear();
        IsBackButtonVisible = false;
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

    /// <summary>
    /// 切换侧边栏展开状态。
    /// </summary>
    [RelayCommand]
    private void ToggleSidebar()
    {
        IsSidebarExpanded = !IsSidebarExpanded;
    }

    /// <summary>
    /// 导航到词典页。
    /// </summary>
    [RelayCommand]
    private void GoToDictionaryPage()
    {
        Content = _pageFactory.GetPageViewModel(ApplicationPageNames.Dictionary);
        _pages.Clear();
        IsBackButtonVisible = false;
    }

    /// <summary>
    /// 导航到收藏页。
    /// </summary>
    [RelayCommand]
    private void GoToFavoritesPage()
    {
        Content = _pageFactory.GetPageViewModel(ApplicationPageNames.Favorites);
        _pages.Clear();
        IsBackButtonVisible = false;
    }

    /// <summary>
    /// 导航到测试页。
    /// </summary>
    [RelayCommand]
    private void GoToTestsPage()
    {
        Content = _pageFactory.GetPageViewModel(ApplicationPageNames.Tests);
        _pages.Clear();
        IsBackButtonVisible = false;
    }

    /// <summary>
    /// 导航到错题页。
    /// </summary>
    [RelayCommand]
    private void GoToMistakePage()
    {
        Content = _pageFactory.GetPageViewModel(ApplicationPageNames.Mistakes);
        _pages.Clear();
        IsBackButtonVisible = false;
    }

    /// <summary>
    /// 导航到设置页。
    /// </summary>
    [RelayCommand]
    private void GoToSettingsPage()
    {
        Content = _pageFactory.GetPageViewModel(ApplicationPageNames.Settings);
        _pages.Clear();
        IsBackButtonVisible = false;
    }
    
    /// <summary>
    /// 返回上一页。
    /// </summary>
    [RelayCommand]
    private void GoBack()
    {
        if(_pages.Count <= 0) return;
        Content = _pages[^1];
        _pages.RemoveAt(_pages.Count - 1);
        if(_pages.Count <= 0)IsBackButtonVisible = false;
    }
}