using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AELP.Data;

public class MistakeDataModel
{
    [Key]
    public int Id { get; set; }
    public int WordId { get; set; }
    [NotMapped]
    public string? Word { get; set; }
    public DateTime Time { get; set; }
    public int Count { get; set; }
}