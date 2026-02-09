using System.ComponentModel.DataAnnotations.Schema;

namespace AELP.Models;

public partial class Dictionary : Word
{
    [Column("cet4")]
    public int? Cet4 { get; set; }
    [Column("cet6")]
    public int? Cet6 { get; set; }
    [Column("hs")]
    public int? Hs { get; set; }
    [Column("ph")]
    public int? Ph { get; set; }
    [Column("tf")]
    public int? Tf { get; set; }
    [Column("ys")]
    public int? Ys { get; set; }
}