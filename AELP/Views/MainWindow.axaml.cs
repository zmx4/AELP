using Avalonia.Controls;
using Avalonia.Input;
using AELP.ViewModels;

namespace AELP.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void NavigationPanel_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel && viewModel.ToggleSidebarCommand.CanExecute(null))
        {
            viewModel.ToggleSidebarCommand.Execute(null);
        }
    }
}