// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System.Diagnostics;

namespace QuickToAELP;

internal sealed partial class QuickToAELPPage : ListPage
{
    public QuickToAELPPage()
    {
        // Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        Icon = new("\ud83d\udcd6");
        Title = "AELP";
        Name = "Open";
    }

    public override IListItem[] GetItems()
    {
        // var command = new OpenUrlCommand("https://learn.microsoft.com");
        var command = new StartAppCommand(@"D:\Dev\Project\AELP\AELP.Desktop\bin\Debug\net10.0\win-x64\AELP.Desktop.exe");
        return [
            new ListItem(command)
            {
                Title = "Start AELP",
            }
        ];
    }
}

internal sealed class StartAppCommand : InvokableCommand
{
    private readonly string _path;

    public StartAppCommand(string path)
    {
        _path = path;
        Name = "Start AELP";
        Icon = new("\ud83d\ude80"); // 🚀 图标
    }

    public override CommandResult Invoke()
    {
        try
        {
            Process.Start(new ProcessStartInfo(_path)
            {
                UseShellExecute = true
            });
        }
        catch
        {
            // 忽略启动失败的异常或在此处记录日志
        }

        return CommandResult.Dismiss();
    }
}
