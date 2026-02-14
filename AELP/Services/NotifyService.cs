using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Ursa.Controls;

namespace AELP.Services;

public class NotifyService(IPreferenceStorage preferenceStorage) : INotifyService
{
    private Ursa.Controls.WindowNotificationManager? _notificationManager;
    private readonly TimeSpan _notifyDuration = TimeSpan.FromSeconds(preferenceStorage.Get("NotificationDuration", 3));
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