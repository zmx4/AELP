using System.ComponentModel.DataAnnotations;

namespace AELP.Data;

public class WordDataModel
{
    [Key]
    public int Id { get; set; }
    
    public string Word { get; set; } 

    public string Translation { get; set; }
    
}