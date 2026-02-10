using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using AELP.ViewModels;

namespace AELP.Views;

public partial class TestSessionPageView : UserControl
{
    private TestSessionPageViewModel? _viewModel;

    public TestSessionPageView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        EnsureQuestionFocus();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        UnsubscribeViewModel();
        base.OnDetachedFromVisualTree(e);
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

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        UnsubscribeViewModel();
        _viewModel = DataContext as TestSessionPageViewModel;
        if (_viewModel is not null)
        {
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        EnsureQuestionFocus();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(TestSessionPageViewModel.CurrentIndex)
            or nameof(TestSessionPageViewModel.IsChoiceQuestion)
            or nameof(TestSessionPageViewModel.IsTesting))
        {
            EnsureQuestionFocus();
        }
    }

    private void EnsureQuestionFocus()
    {
        if (_viewModel is null || !_viewModel.IsTesting)
        {
            return;
        }

        Dispatcher.UIThread.Post(() =>
        {
            if (_viewModel.IsChoiceQuestion)
            {
                Focus();
                return;
            }

            FillInputBox?.Focus();
        }, DispatcherPriority.Background);
    }

    private void UnsubscribeViewModel()
    {
        if (_viewModel is not null)
        {
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
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
