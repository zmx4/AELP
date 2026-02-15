using System.ComponentModel.DataAnnotations;

namespace AELP.Data;

/// <summary>
/// 用户词汇数据模型。
/// </summary>
public class WordDataModel
{
    /// <summary>
    /// 主键 ID。
    /// </summary>
    [Key]
    public int Id { get; init; }
    
    /// <summary>
    /// 单词文本。
    /// </summary>
    public required string Word { get; set; } 

    /// <summary>
    /// 单词译文。
    /// </summary>
    public required string? Translation { get; set; }
    
}