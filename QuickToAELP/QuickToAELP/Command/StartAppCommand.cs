using System.Diagnostics;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace QuickToAELP;

internal sealed partial class StartAppCommand : InvokableCommand
{
    private readonly string _path;
    private readonly string _parameter;

    public StartAppCommand(string path, string parameter = "")
    {
        _path = path;
        _parameter = parameter;
        Name = "Start AELP";
        Icon = new("\ud83d\ude80"); // 🚀 图标
    }

    public override CommandResult Invoke()
    {
        try
        {
            Process.Start(new ProcessStartInfo(_path, _parameter)
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