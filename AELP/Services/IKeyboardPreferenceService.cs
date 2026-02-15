namespace AELP.Services;

/// <summary>
/// 提供按键偏好设置的读取与保存能力。
/// </summary>
public interface IKeyboardPreferenceService
{
    /// <summary>
    /// 获取当前选择题选项按键映射。
    /// </summary>
    /// <returns>
    /// 包含按键映射信息的字符串，具体格式由实现决定。
    /// </returns>
    string GetChoiceOptionKeys();

    /// <summary>
    /// 保存选择题选项按键映射。
    /// </summary>
    /// <param name="keys">按键映射字符串。</param>
    void SetChoiceOptionKeys(string keys);
}