using CommunityToolkit.Mvvm.ComponentModel;

namespace AELP.ViewModels;

/// <summary>
/// 所有视图模型的基类。
/// </summary>
public abstract class ViewModelBase : ObservableObject
{
    /// <summary>
    /// 设置页面导航参数。
    /// </summary>
    /// <param name="parameter">页面参数对象。</param>
    public virtual void SetParameter(object parameter) { }
}