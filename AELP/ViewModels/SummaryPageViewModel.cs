using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AELP.Data;
using AELP.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace AELP.ViewModels;

public partial class SummaryPageViewModel : PageViewModel
{
    private readonly ITestDataStorageService _testDataStorageService;
    private List<ProblemData> _problems = new();

    public SummaryPageViewModel(ITestDataStorageService testDataStorageService)
    {
        _testDataStorageService = testDataStorageService;
        PageNames = Data.ApplicationPageNames.Summary;
        TimePieSeries = Array.Empty<ISeries>();
        AccuracyLineSeries = Array.Empty<ISeries>();
        AccuracyXAxes = Array.Empty<Axis>();
        AccuracyYAxes = new[]
        {
            new Axis
            {
                MinLimit = 0,
                MaxLimit = 1,
                Labeler = value => $"{value:P0}"
            }
        };

        _ = LoadAccuracyAsync();
    }

    [ObservableProperty] private IEnumerable<ISeries> _timePieSeries;
    [ObservableProperty] private IEnumerable<ISeries> _accuracyLineSeries;
    [ObservableProperty] private Axis[] _accuracyXAxes;
    [ObservableProperty] private Axis[] _accuracyYAxes;

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
                Values = new[] { seconds }
            });
        }

        TimePieSeries = series;
    }

    private async Task LoadAccuracyAsync()
    {
        var tests = await _testDataStorageService.GetRecentTests(10);
        if (tests.Length == 0)
        {
            AccuracyLineSeries = Array.Empty<ISeries>();
            AccuracyXAxes = Array.Empty<Axis>();
            return;
        }

        var ordered = tests.OrderBy(t => t.TestTime).ToList();
        var values = ordered.Select(t => t.Accuracy).ToArray();
        var labels = ordered.Select(t => t.TestTime.ToString("MM-dd")).ToArray();

        AccuracyLineSeries = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = values,
                GeometrySize = 8,
                Fill = null
            }
        };

        AccuracyXAxes = new[]
        {
            new Axis
            {
                Labels = labels
            }
        };
    }
}