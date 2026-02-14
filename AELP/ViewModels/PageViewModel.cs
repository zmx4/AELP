using System;
using AELP.Data;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AELP.ViewModels;

public partial class PageViewModel :ViewModelBase, IDisposable
{
    [ObservableProperty]
    private ApplicationPageNames _pageNames;
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}