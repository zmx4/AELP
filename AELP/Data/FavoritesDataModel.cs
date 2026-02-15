using System.ComponentModel.DataAnnotations;

namespace AELP.Data;

/// <summary>
/// 收藏数据模型。
/// </summary>
public class FavoritesDataModel
{
    /// <summary>
    /// 关联单词 ID（主键）。
    /// </summary>
    [Key] public int WordId { get; init; }

    /// <summary>
    /// 是否属于 CET4。
    /// </summary>
    public bool IsCet4 { get; set; }

    /// <summary>
    /// 是否属于 CET6。
    /// </summary>
    public bool IsCet6 { get; set; }

    /// <summary>
    /// 是否属于高中词表。
    /// </summary>
    public bool IsHs { get; set; }

    /// <summary>
    /// 是否属于小学词表。
    /// </summary>
    public bool IsPh { get; set; }

    /// <summary>
    /// 是否属于托福词表。
    /// </summary>
    public bool IsTf { get; set; }

    /// <summary>
    /// 是否属于雅思词表。
    /// </summary>
    public bool IsYs { get; set; }

    /// <summary>
    /// 是否已收藏。
    /// </summary>
    public bool IsFavorite { get; set; }

    /// <summary>
    /// 关联单词实体。
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.ForeignKey("WordId")]
    public WordDataModel? Word { get; set; }
    
}