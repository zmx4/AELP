using AELP.Data;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AELP.ViewModels;

public partial class PageViewModel :ViewModelBase
{
    [ObservableProperty]
    private ApplicationPageNames _pageNames;
}