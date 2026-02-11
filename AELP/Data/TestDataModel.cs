using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AELP.Data;

public class TestDataModel
{
    [Key, Required]
    public int Id { get; init; }
    [Precision(3)]
    public DateTime TestTime { get; init; }
    
    public required List<int> Mistakes { get; init; }
    
    public int TotalQuestions { get; init; }
    
    public double Accuracy { get; init; }
}