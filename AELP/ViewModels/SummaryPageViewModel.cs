using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AELP.Data;
using AELP.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace AELP.ViewModels;

public partial class SummaryPageViewModel : PageViewModel
{
    private List<ProblemData> _problems;
    private readonly ITestDataStorageService _testDataStorageService;
    
    // 测试结果统计
    [ObservableProperty] private int _totalQuestions;
    [ObservableProperty] private int _correctCount;
    [ObservableProperty] private int _wrongCount;
    [ObservableProperty] private double _accuracy;
    [ObservableProperty] private string _accuracyText = "0%";
    [ObservableProperty] private long _totalTimeMs;
    [ObservableProperty] private string _totalTimeText = "0秒";
    
    // 饼状图 - 每个单词耗时分布
    [ObservableProperty] private ISeries[] _timePieSeries = Array.Empty<ISeries>();
    
    // 折线图 - 历次测试准确率
    [ObservableProperty] private ISeries[] _accuracyLineSeries = Array.Empty<ISeries>();
    [ObservableProperty] private Axis[] _accuracyXAxes = Array.Empty<Axis>();
    [ObservableProperty] private Axis[] _accuracyYAxes = Array.Empty<Axis>();
    
    // 单词详情列表
    public ObservableCollection<WordResultItem> WordResults { get; } = new();

    public SummaryPageViewModel(ITestDataStorageService testDataStorageService)
    {
        _problems = new List<ProblemData>();
        _testDataStorageService = testDataStorageService;
        PageNames = ApplicationPageNames.Summary;
    }

    public override void SetParameter(object parameter)
    {
        if (parameter is List<ProblemData> problems)
        {
            _problems = problems;
            CalculateStatistics();
            BuildTimePieChart();
            _ = LoadHistoryAndBuildLineChartAsync();
        }
    }

    private void CalculateStatistics()
    {
        TotalQuestions = _problems.Count;
        CorrectCount = _problems.Count(p => p.IsRight);
        WrongCount = TotalQuestions - CorrectCount;
        Accuracy = TotalQuestions > 0 ? (double)CorrectCount / TotalQuestions : 0;
        AccuracyText = $"{Accuracy:P0}";
        TotalTimeMs = _problems.Sum(p => p.CostTimeMs);
        
        var seconds = TotalTimeMs / 1000.0;
        if (seconds >= 60)
        {
            var minutes = (int)(seconds / 60);
            var remainingSeconds = (int)(seconds % 60);
            TotalTimeText = $"{minutes}分{remainingSeconds}秒";
        }
        else
        {
            TotalTimeText = $"{seconds:F1}秒";
        }
        
        // 填充单词结果列表
        WordResults.Clear();
        foreach (var problem in _problems)
        {
            WordResults.Add(new WordResultItem
            {
                Word = problem.Word,
                Translation = problem.Translation?.Replace("\\n", " ") ?? "",
                IsCorrect = problem.IsRight,
                TimeSpent = $"{problem.CostTimeMs / 1000.0:F1}秒"
            });
        }
    }

    private void BuildTimePieChart()
    {
        if (_problems.Count == 0) return;
        
        var colors = new[]
        {
            SKColors.CornflowerBlue,
            SKColors.Coral,
            SKColors.MediumSeaGreen,
            SKColors.Orange,
            SKColors.MediumPurple,
            SKColors.Teal,
            SKColors.Crimson,
            SKColors.DodgerBlue,
            SKColors.ForestGreen,
            SKColors.Gold
        };
        
        var pieData = _problems.Select((p, i) => new PieSeries<double>
        {
            Values = new[] { (double)p.CostTimeMs },
            Name = p.Word.Length > 10 ? p.Word.Substring(0, 10) + "..." : p.Word,
            Fill = new SolidColorPaint(colors[i % colors.Length]),
            DataLabelsSize = 12,
            DataLabelsPaint = new SolidColorPaint(SKColors.White),
            DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle
        }).ToArray();
        
        TimePieSeries = pieData;
    }

    private async Task LoadHistoryAndBuildLineChartAsync()
    {
        try
        {
            var recentTests = await _testDataStorageService.GetRecentTests(10);
            
            if (recentTests.Length == 0)
            {
                // 如果没有历史数据，只显示当前测试
                AccuracyLineSeries = new ISeries[]
                {
                    new LineSeries<double>
                    {
                        Values = new[] { Accuracy * 100 },
                        Name = "准确率 (%)",
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.CornflowerBlue) { StrokeThickness = 3 },
                        GeometrySize = 12,
                        GeometryStroke = new SolidColorPaint(SKColors.CornflowerBlue) { StrokeThickness = 2 }
                    }
                };
            }
            else
            {
                // 按时间排序，显示历史准确率趋势
                var sortedTests = recentTests.OrderBy(t => t.TestTime).ToList();
                var accuracies = sortedTests.Select(t => t.Accuracy * 100).ToList();
                
                // 添加当前测试结果
                accuracies.Add(Accuracy * 100);
                
                AccuracyLineSeries = new ISeries[]
                {
                    new LineSeries<double>
                    {
                        Values = accuracies.ToArray(),
                        Name = "准确率 (%)",
                        Fill = new SolidColorPaint(SKColors.CornflowerBlue.WithAlpha(50)),
                        Stroke = new SolidColorPaint(SKColors.CornflowerBlue) { StrokeThickness = 3 },
                        GeometrySize = 10,
                        GeometryFill = new SolidColorPaint(SKColors.White),
                        GeometryStroke = new SolidColorPaint(SKColors.CornflowerBlue) { StrokeThickness = 2 },
                        LineSmoothness = 0.3
                    }
                };
            }
            
            AccuracyXAxes = new Axis[]
            {
                new Axis
                {
                    Name = "测试次数",
                    NamePaint = new SolidColorPaint(SKColors.Gray),
                    LabelsPaint = new SolidColorPaint(SKColors.Gray),
                    MinStep = 1
                }
            };
            
            AccuracyYAxes = new Axis[]
            {
                new Axis
                {
                    Name = "准确率 (%)",
                    NamePaint = new SolidColorPaint(SKColors.Gray),
                    LabelsPaint = new SolidColorPaint(SKColors.Gray),
                    MinLimit = 0,
                    MaxLimit = 100
                }
            };
        }
        catch
        {
            // 处理加载失败的情况
        }
    }
}

public class WordResultItem
{
    public string Word { get; set; } = "";
    public string Translation { get; set; } = "";
    public bool IsCorrect { get; set; }
    public string TimeSpent { get; set; } = "";
}