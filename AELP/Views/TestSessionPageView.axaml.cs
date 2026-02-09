using Avalonia.Controls;
using Avalonia.Input;
using AELP.ViewModels;
using Avalonia;

namespace AELP.Views;

public partial class TestSessionPageView : UserControl
{
    public TestSessionPageView()
    {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        Focus();
    }

    private async void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is not TestSessionPageViewModel viewModel)
        {
            return;
        }

        var keyChar = MapKeyToChar(e.Key);
        if (keyChar is null)
        {
            return;
        }

        var handled = await viewModel.TryHandleChoiceKeyAsync(keyChar.Value);
        if (handled)
        {
            e.Handled = true;
        }
    }

    private static char? MapKeyToChar(Key key)
    {
        if (key is >= Key.A and <= Key.Z)
        {
            return (char)('a' + (key - Key.A));
        }

        return null;
    }
}
