using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace AELP.Behaviors;

public class InfiniteScrollBehavior
{
    // 注册附加属性 LoadMoreCommand
    public static readonly AttachedProperty<ICommand?> LoadMoreCommandProperty =
        AvaloniaProperty.RegisterAttached<InfiniteScrollBehavior, Control, ICommand?>("LoadMoreCommand");

    public static void SetLoadMoreCommand(Control element, ICommand? value)
    {
        element.SetValue(LoadMoreCommandProperty, value);
    }

    public static ICommand? GetLoadMoreCommand(Control element)
    {
        return element.GetValue(LoadMoreCommandProperty);
    }

    // 静态构造函数中监听属性的变化
    static InfiniteScrollBehavior()
    {
        LoadMoreCommandProperty.Changed.AddClassHandler<Control>(HandleLoadMoreCommandChanged);
    }

    private static void HandleLoadMoreCommandChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is ICommand)
        {
            // 当 Command 被绑定时，订阅滚动事件
            control.AddHandler(ScrollViewer.ScrollChangedEvent, ScrollChanged,
                Avalonia.Interactivity.RoutingStrategies.Bubble);
        }
        else
        {
            // 当 Command 被解绑时，取消订阅
            control.RemoveHandler(ScrollViewer.ScrollChangedEvent, ScrollChanged);
        }
    }

    private static void ScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (sender is not Control control || e.Source is not ScrollViewer scrollViewer) return;
        const double threshold = 50; // 距离底部 50 像素时触发

        // 判断是否滚动到底部
        if (!(scrollViewer.Offset.Y + scrollViewer.Viewport.Height >= scrollViewer.Extent.Height - threshold)) return;
        // 获取绑定的 Command 并执行
        var command = GetLoadMoreCommand(control);
        if (command != null && command.CanExecute(null))
        {
            command.Execute(null);
        }
    }
}