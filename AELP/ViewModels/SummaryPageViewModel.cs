using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AELP.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace AELP.ViewModels;

/// <summary>
/// 测试总结页面视图模型，展示耗时分布与准确率趋势。
/// </summary>
public partial class SummaryPageViewModel : PageViewModel
{
    private readonly ITestDataStorageService _testDataStorageService;
    private List<ProblemData> _problems = new();

    /// <summary>
    /// 初始化 <see cref="SummaryPageViewModel"/>。
    /// </summary>
    /// <param name="testDataStorageService">测试数据存储服务。</param>
    public SummaryPageViewModel(ITestDataStorageService testDataStorageService)
    {
        _testDataStorageService = testDataStorageService;
        PageNames = Data.ApplicationPageNames.Summary;
        TimePieSeries = Array.Empty<ISeries>();
        AccuracyLineSeries = Array.Empty<ISeries>();
        AccuracyXAxes = Array.Empty<Axis>();
        AccuracyYAxes =
        [
            new Axis
            {
                MinLimit = 0,
                MaxLimit = 1,
                Labeler = value => $"{value:P0}"
            }
        ];

        _ = LoadAccuracyAsync();
    }

    [ObservableProperty] private IEnumerable<ISeries> _timePieSeries;
    [ObservableProperty] private IEnumerable<ISeries> _accuracyLineSeries;
    [ObservableProperty] private Axis[] _accuracyXAxes;
    [ObservableProperty] private Axis[] _accuracyYAxes;

    /// <summary>
    /// 设置总结页面参数。
    /// </summary>
    /// <param name="parameter">页面参数，期望为题目统计列表。</param>
    public override void SetParameter(object parameter)
    {
        if (parameter is List<ProblemData> problems)
        {
            _problems = problems;
            BuildTimePieSeries();
            _ = LoadAccuracyAsync();
        }
    }

    private void BuildTimePieSeries()
    {
        if (_problems.Count == 0)
        {
            TimePieSeries = Array.Empty<ISeries>();
            return;
        }

        var series = new List<ISeries>();
        foreach (var problem in _problems)
        {
            var seconds = Math.Max(0.1, problem.CostTimeMs / 1000d);
            series.Add(new PieSeries<double>
            {
                Name = problem.Word,
                Values = [seconds]
            });
        }

        TimePieSeries = series;
    }

    private async Task LoadAccuracyAsync()
    {
        var tests = await _testDataStorageService.GetRecentTests(10);
        if (tests.Length == 0)
        {
            AccuracyLineSeries = [];
            AccuracyXAxes = [];
            return;
        }

        var ordered = tests.OrderBy(t => t.TestTime).ToList();
        var values = ordered.Select(t => t.Accuracy).ToArray();
        var labels = ordered.Select(t => t.TestTime.ToString("MM-dd")).ToArray();

        AccuracyLineSeries =
        [
            new LineSeries<double>
            {
                Values = values,
                GeometrySize = 8,
                Fill = null
            }
        ];

        AccuracyXAxes =
        [
            new Axis
            {
                Labels = labels
            }
        ];
    }
}