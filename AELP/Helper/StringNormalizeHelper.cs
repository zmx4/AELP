using System;

namespace AELP.Helper;

/// <summary>
/// 字符串规范化辅助工具。
/// </summary>
public static class StringNormalizeHelper
{
    /// <summary>
    /// 将转义换行符规范化为真实换行。
    /// </summary>
    /// <param name="translation">待处理文本。</param>
    /// <returns>规范化后的文本。</returns>
    public  static string? NormalizeTranslation(string? translation)
    {
        return translation?
            .Replace("\\r\\n", "\n", StringComparison.Ordinal)
            .Replace("\\n", "\n", StringComparison.Ordinal);
    }
    
    /// <summary>
    /// 截取文本首行。
    /// </summary>
    /// <param name="translation">待处理文本。</param>
    /// <returns>首行文本。</returns>
    public  static string? ShortenString(string? translation)
    {
        return translation?
            .Split("\\n")[0];
    }
}