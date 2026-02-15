using System.ComponentModel.DataAnnotations.Schema;

namespace AELP.Models;

/// <summary>
/// 词条基础模型。
/// </summary>
public class Word
{
    /// <summary>
    /// 原始单词文本。
    /// </summary>
    [Column("word")]
    public string RawWord { get; set; } = null!;

    /// <summary>
    /// 单词译文。
    /// </summary>
    [Column("translation")]
    public string? Translation { get; set; }
}