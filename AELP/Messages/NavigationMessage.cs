using AELP.Data;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AELP.Messages;

public class NavigationMessage : ValueChangedMessage<(ApplicationPageNames PageName, object? Parameter)>
{
    public NavigationMessage(ApplicationPageNames pageName, object? parameter = null) 
        : base((pageName, parameter))
    {
    }
}
