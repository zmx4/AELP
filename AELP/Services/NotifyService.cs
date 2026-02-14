using System.Threading.Tasks;
using Avalonia.Controls.Notifications;

namespace AELP.Services;

public class NotifyService : INotifyService
{
    public async Task Notify(string title, string message, NotificationType type = NotificationType.Information)
    {
        throw new System.NotImplementedException();
    }

    public void Alert(string title, string message)
    {
        throw new System.NotImplementedException();
    }
}