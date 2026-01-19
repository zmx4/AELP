using AELP.Factories;

namespace AELP.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private PageFactory _pageFactory;
    
    public MainWindowViewModel(PageFactory pageFactory)
    {
        _pageFactory = pageFactory;
    }
}