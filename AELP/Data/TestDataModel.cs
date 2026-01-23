using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AELP.Data;

public class TestDataModel
{
    [Key, Required]
    public int Id { get; set; }
    [Precision(3)]
    public DateTime TestTime { get; set; }
    
    public List<int> Mistakes { get; set; }
    
    public int TotalQuestions { get; set; }
    
    public double Accuracy { get; set; }
}