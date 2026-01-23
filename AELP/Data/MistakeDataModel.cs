using System;
using System.ComponentModel.DataAnnotations;

namespace AELP.Data;

public class MistakeDataModel
{
    [Key]
    public int Id { get; set; }
    public int WordId { get; set; }
    public DateTime Time { get; set; }
    public int Count { get; set; }
}