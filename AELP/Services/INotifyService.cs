using System;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;

namespace AELP.Services;

public interface INotifyService
{
    public void SetNotificationDuration(int seconds);
    
    public void Notify(
        string title,
        string message,
        NotificationType type = NotificationType.Information,
        TimeSpan? duration = null);
    
    public Task Alert(string title, string message);
}