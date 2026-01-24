using AELP.Data;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AELP.Messages;

public class NavigationMessage(ApplicationPageNames pageName, object? parameter = null)
    : ValueChangedMessage<(ApplicationPageNames PageName, object? Parameter)>((pageName, parameter));
