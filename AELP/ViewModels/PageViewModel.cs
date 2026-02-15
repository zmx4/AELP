using System;
using AELP.Data;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AELP.ViewModels;

/// <summary>
/// 页面视图模型基类，提供页面标识和释放能力。
/// </summary>
public partial class PageViewModel :ViewModelBase, IDisposable
{
    [ObservableProperty]
    private ApplicationPageNames _pageNames;
    
    /// <summary>
    /// 释放页面资源。
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}