using AELP.Data;
using AELP.Factories;
using AELP.Messages;
using AELP.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using Moq;

namespace AELP.UnitTest.Viewmodels;

public class TestPageViewModel : PageViewModel
{
    public ApplicationPageNames PageType { get; set; }
    public object? ReceivedParameter { get; private set; }

    public override void SetParameter(object parameter)
    {
        ReceivedParameter = parameter;
        base.SetParameter(parameter);
    }
}

public class MainWindowViewModelTest : IDisposable
{
    private readonly MainWindowViewModel _viewModel;
    private readonly Mock<Func<ApplicationPageNames, PageViewModel>> _mockPageFactoryDelegate;

    public MainWindowViewModelTest()
    {
        // 创建页面工厂的Mock委托
        _mockPageFactoryDelegate = new Mock<Func<ApplicationPageNames, PageViewModel>>();
        
        // 设置默认行为：返回一个新的TestPageViewModel
        _mockPageFactoryDelegate
            .Setup(f => f(It.IsAny<ApplicationPageNames>()))
            .Returns((ApplicationPageNames name) => new TestPageViewModel { PageType = name });

        // 使用真实的PageFactory，传入Mock委托
        var pageFactory = new PageFactory(_mockPageFactoryDelegate.Object);
        _viewModel = new MainWindowViewModel(pageFactory);
    }

    public void Dispose()
    {
        // 清理消息订阅，防止测试间干扰
        WeakReferenceMessenger.Default.UnregisterAll(_viewModel);
    }

    [Fact]
    public void Constructor_InitializesCorrectly()
    {
        // 验证初始化时加载了Dictionary页面
        Assert.NotNull(_viewModel.Content);
        Assert.IsType<TestPageViewModel>(_viewModel.Content);
        Assert.Equal(ApplicationPageNames.Dictionary, ((TestPageViewModel)_viewModel.Content).PageType);
        
        // 验证默认侧边栏状态
        Assert.True(_viewModel.IsSidebarExpanded);
        Assert.Equal(120, _viewModel.SidebarWidth); // 展开宽度
        Assert.False(_viewModel.IsBackButtonVisible);
    }

    [Fact]
    public void ToggleSidebarCommand_TogglesState()
    {
        // 初始状态为展开
        Assert.True(_viewModel.IsSidebarExpanded);
        
        // 执行切换命令
        _viewModel.ToggleSidebarCommand.Execute(null);
        
        // 验证状态变为折叠
        Assert.False(_viewModel.IsSidebarExpanded);
        Assert.Equal(70, _viewModel.SidebarWidth); // 折叠宽度

        // 再次切换
        _viewModel.ToggleSidebarCommand.Execute(null);
        Assert.True(_viewModel.IsSidebarExpanded);
    }

    [Fact]
    public void NavigationCommands_GoToDictionary_UpdatesContent()
    {
        _viewModel.GoToDictionaryPageCommand.Execute(null);
        VerifyCurrentPage(ApplicationPageNames.Dictionary);
        Assert.False(_viewModel.IsBackButtonVisible);
    }

    [Fact]
    public void NavigationCommands_GoToFavorites_UpdatesContent()
    {
        _viewModel.GoToFavoritesPageCommand.Execute(null);
        VerifyCurrentPage(ApplicationPageNames.Favorites);
        Assert.False(_viewModel.IsBackButtonVisible);
    }

    [Fact]
    public void NavigationCommands_GoToMistakes_UpdatesContent()
    {
        _viewModel.GoToMistakePageCommand.Execute(null);
        VerifyCurrentPage(ApplicationPageNames.Mistakes);
        Assert.False(_viewModel.IsBackButtonVisible);
    }

    [Fact]
    public void NavigationCommands_GoToSettings_UpdatesContent()
    {
        _viewModel.GoToSettingsPageCommand.Execute(null);
        VerifyCurrentPage(ApplicationPageNames.Settings);
        Assert.False(_viewModel.IsBackButtonVisible);
    }

    [Fact]
    public void NavigationCommands_GoToTests_UpdatesContent()
    {
        _viewModel.GoToTestsPageCommand.Execute(null);
        VerifyCurrentPage(ApplicationPageNames.Tests);
        Assert.False(_viewModel.IsBackButtonVisible);
    }

    [Fact]
    public void NavigationMessage_PushesPageAndShowsBackButton()
    {
        // 发送导航消息
        var targetPage = ApplicationPageNames.Tests;
        WeakReferenceMessenger.Default.Send(new NavigationMessage(targetPage));

        // 验证页面切换
        VerifyCurrentPage(targetPage);
        
        // 验证返回按钮可见
        Assert.True(_viewModel.IsBackButtonVisible);
    }

    [Fact]
    public void NavigationMessage_WithParameter_PassesParameter()
    {
        // 准备带参数的导航消息
        var targetPage = ApplicationPageNames.TestSession;
        var parameter = "TestParam";
        
        WeakReferenceMessenger.Default.Send(new NavigationMessage(targetPage, parameter));

        // 验证页面切换
        VerifyCurrentPage(targetPage);
        
        // 验证参数传递
        var currentPage = _viewModel.Content as TestPageViewModel;
        Assert.NotNull(currentPage);
        Assert.Equal(parameter, currentPage.ReceivedParameter);
    }

    [Fact]
    public void GoBackCommand_NavigatesBack()
    {
        // 初始页面 (Dictionary)
        var initialPage = _viewModel.Content;

        // 导航到新页面 (Tests)
        WeakReferenceMessenger.Default.Send(new NavigationMessage(ApplicationPageNames.Tests));
        Assert.NotEqual(initialPage, _viewModel.Content);
        Assert.True(_viewModel.IsBackButtonVisible);

        // 执行返回命令
        _viewModel.GoBackCommand.Execute(null);

        // 验证返回到了初始页面
        Assert.Equal(initialPage, _viewModel.Content);
        Assert.False(_viewModel.IsBackButtonVisible);
    }

    [Fact]
    public void GoBackCommand_MultipleLevels_NavigatesBackCorrectly()
    {
        // Dictionary -> Tests -> Settings
        WeakReferenceMessenger.Default.Send(new NavigationMessage(ApplicationPageNames.Tests));
        WeakReferenceMessenger.Default.Send(new NavigationMessage(ApplicationPageNames.Settings));
        
        Assert.True(_viewModel.IsBackButtonVisible);
        VerifyCurrentPage(ApplicationPageNames.Settings);

        // Back -> Tests
        _viewModel.GoBackCommand.Execute(null);
        VerifyCurrentPage(ApplicationPageNames.Tests);
        Assert.True(_viewModel.IsBackButtonVisible); // 还有一页在栈中

        // Back -> Dictionary
        _viewModel.GoBackCommand.Execute(null);
        VerifyCurrentPage(ApplicationPageNames.Dictionary);
        Assert.False(_viewModel.IsBackButtonVisible); // 栈空
    }

    [Fact]
    public void MainNavigationCommands_ClearHistory()
    {
        // 导航入栈: Dictionary -> Tests
        WeakReferenceMessenger.Default.Send(new NavigationMessage(ApplicationPageNames.Tests));
        Assert.True(_viewModel.IsBackButtonVisible);

        // 使用主导航命令 (如 GoToFavorites)
        _viewModel.GoToFavoritesPageCommand.Execute(null);

        // 验证历史记录被清除
        VerifyCurrentPage(ApplicationPageNames.Favorites);
        Assert.False(_viewModel.IsBackButtonVisible);
    }

    private void VerifyCurrentPage(ApplicationPageNames expectedPage)
    {
        Assert.NotNull(_viewModel.Content);
        Assert.IsType<TestPageViewModel>(_viewModel.Content);
        Assert.Equal(expectedPage, ((TestPageViewModel)_viewModel.Content).PageType);
    }
}