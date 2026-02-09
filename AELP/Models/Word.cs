using System.ComponentModel.DataAnnotations.Schema;

namespace AELP.Models;

public class Word
{
    [Column("word")]
    public string RawWord { get; set; } = null!;
    [Column("translation")]
    public string? Translation { get; set; }
}