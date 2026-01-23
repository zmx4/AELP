using System;
using System.Collections.Generic;

namespace AELP.Data;

public class TestDataModel
{
    public DateTime TestTime { get; set; }
    
    public List<MistakeDataModel> Mistakes { get; set; }
    
    public int TotalQuestions { get; set; }
    
    public double Accuracy { get; set; }
}