using System.ComponentModel.DataAnnotations;

namespace AELP.Data;

public class WordDataModel
{
    [Key]
    public int Id { get; set; }
    
    public required string Word { get; set; } 

    public required string Translation { get; set; }
    
}