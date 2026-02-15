using System;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;

namespace AELP.Services;

/// <summary>
/// 提供应用内通知与提醒弹窗能力。
/// </summary>
public interface INotifyService
{
    /// <summary>
    /// 设置通知持续时间（秒）
    /// </summary>
    /// <param name="seconds">通知持续时间（秒）</param>
    public void SetNotificationDuration(int seconds);
    /// <summary>
    /// 显示通知
    /// </summary>
    /// <param name="title">通知标题</param>
    /// <param name="message">通知信息</param>
    /// <param name="type">通知类型</param>
    /// <param name="duration">通知持续时间</param>
    public void Notify(
        string title,
        string message,
        NotificationType type = NotificationType.Information,
        TimeSpan? duration = null);
    /// <summary>
    /// 显示警告对话框
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="message">内容</param>
    /// <returns>表示对话框显示流程的异步任务。</returns>
    public Task Alert(string title, string message);
}