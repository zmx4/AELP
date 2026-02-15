using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AELP.Data;

/// <summary>
/// 错题数据模型。
/// </summary>
public sealed class MistakeDataModel
{
    /// <summary>
    /// 主键 ID。
    /// </summary>
    [Key]
    public int Id { get; init; }

    /// <summary>
    /// 关联单词 ID。
    /// </summary>
    public int WordId { get; set; }

    /// <summary>
    /// 单词文本（非映射字段）。
    /// </summary>
    [NotMapped]
    public string? Word { get; set; }

    /// <summary>
    /// 最近错误时间。
    /// </summary>
    public DateTime Time { get; set; }

    /// <summary>
    /// 错误计数。
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// 单词译文（非映射字段）。
    /// </summary>
    [NotMapped]
    public string? Translation { get; set; }

    /// <summary>
    /// 是否已掌握。
    /// </summary>
    [NotMapped]
    public bool IsMastered => Count <= 0;

    /// <summary>
    /// 关联单词实体。
    /// </summary>
    [ForeignKey("WordId")]
    public WordDataModel? RawWord { get; init; }
}