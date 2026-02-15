using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AELP.Data;

/// <summary>
/// 测试记录数据模型。
/// </summary>
public class TestDataModel
{
    /// <summary>
    /// 主键 ID。
    /// </summary>
    [Key, Required]
    public int Id { get; init; }

    /// <summary>
    /// 测试时间。
    /// </summary>
    [Precision(3)]
    public DateTime TestTime { get; init; }
    
    /// <summary>
    /// 错题 ID 列表。
    /// </summary>
    public required List<int> Mistakes { get; init; }
    
    /// <summary>
    /// 题目总数。
    /// </summary>
    public int TotalQuestions { get; init; }
    
    /// <summary>
    /// 准确率（0~1）。
    /// </summary>
    public double Accuracy { get; init; }
}