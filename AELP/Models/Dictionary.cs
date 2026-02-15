using System.ComponentModel.DataAnnotations.Schema;

namespace AELP.Models;

/// <summary>
/// 词典主表模型。
/// </summary>
public partial class Dictionary : Word
{
    /// <summary>
    /// 是否属于 CET4 词表（1 表示是）。
    /// </summary>
    [Column("cet4")]
    public int? Cet4 { get; set; }

    /// <summary>
    /// 是否属于 CET6 词表（1 表示是）。
    /// </summary>
    [Column("cet6")]
    public int? Cet6 { get; set; }

    /// <summary>
    /// 是否属于高中词表（1 表示是）。
    /// </summary>
    [Column("hs")]
    public int? Hs { get; set; }

    /// <summary>
    /// 是否属于小学词表（1 表示是）。
    /// </summary>
    [Column("ph")]
    public int? Ph { get; set; }

    /// <summary>
    /// 是否属于托福词表（1 表示是）。
    /// </summary>
    [Column("tf")]
    public int? Tf { get; set; }

    /// <summary>
    /// 是否属于雅思词表（1 表示是）。
    /// </summary>
    [Column("ys")]
    public int? Ys { get; set; }
}