using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using AELP.ViewModels;

namespace AELP.Views;

public partial class DictionaryPageView : UserControl
{
    public DictionaryPageView()
    {
        InitializeComponent();
    }

    private void OnItemDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is Control control && control.DataContext is string word)
        {
            if (DataContext is DictionaryPageViewModel vm)
            {
                vm.OpenDetailCommand.Execute(word);
            }
        }
    }
}