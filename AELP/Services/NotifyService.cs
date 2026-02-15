using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Ursa.Controls;

namespace AELP.Services;

/// <summary>
/// 通知服务实现，负责应用内通知与提示弹窗。
/// </summary>
public class NotifyService(IPreferenceStorage preferenceStorage) : INotifyService
{
    private Ursa.Controls.WindowNotificationManager? _notificationManager;
    private TimeSpan _notifyDuration = TimeSpan.FromSeconds(preferenceStorage.Get("NotificationDuration", 3));


    /// <summary>
    /// 设置默认通知持续时长（秒）并持久化。
    /// </summary>
    /// <param name="seconds">持续时长，单位秒。</param>
    public void SetNotificationDuration(int seconds)
    {
        _notifyDuration = TimeSpan.FromSeconds(seconds);
        preferenceStorage.Set("NotificationDuration", seconds);
    }

    /// <summary>
    /// 显示应用内通知。
    /// </summary>
    /// <param name="title">通知标题。</param>
    /// <param name="message">通知内容。</param>
    /// <param name="type">通知类型。</param>
    /// <param name="duration">可选持续时长，未指定时使用默认时长。</param>
    public void Notify(
        string title,
        string message,
        NotificationType type = NotificationType.Information,
        TimeSpan? duration = null)
    {
        var manager = GetNotificationManager();
        if (manager is null)
        {
            return;
        }

        var effectiveDuration = duration ?? _notifyDuration;
        manager.Show(new Ursa.Controls.Notification(title, message, type, effectiveDuration, true, null, null),
            type,
            classes:["Light"]);
    }

    /// <summary>
    /// 显示消息弹窗。
    /// </summary>
    /// <param name="title">弹窗标题。</param>
    /// <param name="message">弹窗内容。</param>
    /// <returns>表示弹窗流程的异步任务。</returns>
    public async Task Alert(string title, string message)
    {
        await MessageBox.ShowOverlayAsync(title, message);
    }

    private Ursa.Controls.WindowNotificationManager? GetNotificationManager()
    {
        if (_notificationManager is not null)
        {
            return _notificationManager;
        }

        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop
            || desktop.MainWindow is null)
        {
            return null;
        }

        _notificationManager = new Ursa.Controls.WindowNotificationManager(desktop.MainWindow);
        return _notificationManager;
    }
}