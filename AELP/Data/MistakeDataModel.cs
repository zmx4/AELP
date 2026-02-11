using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AELP.Data;

public sealed class MistakeDataModel
{
    [Key]
    public int Id { get; init; }
    public int WordId { get; set; }
    [NotMapped]
    public string? Word { get; set; }
    public DateTime Time { get; set; }
    public int Count { get; set; }
    [NotMapped]
    public string? Translation { get; set; }
    [NotMapped]
    public bool IsMastered => Count <= 0;
    [ForeignKey("WordId")]
    public WordDataModel? RawWord { get; init; }
}