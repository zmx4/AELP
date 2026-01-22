using CommunityToolkit.Mvvm.ComponentModel;

namespace AELP.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    public virtual void SetParameter(object parameter) { }
}