using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Ursa.Controls;

namespace AELP.Services;

public class NotifyService : INotifyService
{
    private Ursa.Controls.WindowNotificationManager? _notificationManager;

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

        manager.Show(new Avalonia.Controls.Notifications.Notification(title, message, type, duration, null, null));
    }

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