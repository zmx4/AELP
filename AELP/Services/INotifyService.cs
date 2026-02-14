using System.Threading.Tasks;
using Avalonia.Controls.Notifications;

namespace AELP.Services;

public interface INotifyService
{
    
    public Task Notify(string title, string message,NotificationType type = NotificationType.Information);
    
    public void Alert(string title, string message);
}