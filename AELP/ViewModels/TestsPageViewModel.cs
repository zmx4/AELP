using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AELP.Data;
using AELP.Messages;
using AELP.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace AELP.ViewModels;

public partial class TestsPageViewModel : PageViewModel
{
    [ObservableProperty] private Axis[] _accuracyXAxes;
    [ObservableProperty] private Axis[] _accuracyYAxes;
    [ObservableProperty] private IEnumerable<ISeries> _accuracyLineSeries;
    [ObservableProperty] private ObservableCollection<MistakeDataModel> _mistakes;
    private readonly ITestDataStorageService _testDataStorageService;
    private readonly IMistakeDataStorageService _mistakeDataStorageService;
    public TestsPageViewModel(ITestDataStorageService testDataStorageService,IMistakeDataStorageService mistakeDataStorageService)
    {
        _testDataStorageService = testDataStorageService;
        _mistakeDataStorageService = mistakeDataStorageService;
        PageNames = Data.ApplicationPageNames.Tests;
        _mistakes = [];
        TestRanges = new ObservableCollection<TestRange>((TestRange[])Enum.GetValues(typeof(TestRange)));
        QuestionCounts = [5, 10, 15, 20];
        SelectedTestRange = TestRange.Cet4;
        QuestionCount = 10;
        StatusText = string.Empty;

        _ = LoadRecentTestsAsync();
        AccuracyLineSeries = Array.Empty<ISeries>();
        AccuracyXAxes = Array.Empty<Axis>();
        AccuracyYAxes = Array.Empty<Axis>();
        _ = LoadAccuracyAsync();
        _ = LoadMistakesAsync();
    }

    [ObservableProperty] private ObservableCollection<TestRange> _testRanges = new();
    [ObservableProperty] private TestRange _selectedTestRange = TestRange.Cet4;
    [ObservableProperty] private ObservableCollection<int> _questionCounts = new();
    [ObservableProperty] private int _questionCount = 10;
    [ObservableProperty] private string _statusText = string.Empty;

    [ObservableProperty] private bool _hasLatestTest;
    [ObservableProperty] private bool _hasPreviousTest;
    [ObservableProperty] private string _latestTestTimeText = "-";
    [ObservableProperty] private string _latestAccuracyText = "-";
    [ObservableProperty] private string _latestTotalQuestionsText = "-";
    [ObservableProperty] private string _latestRightWrongText = "-";
    [ObservableProperty] private string _previousTestTimeText = "-";
    [ObservableProperty] private string _previousAccuracyText = "-";
    [ObservableProperty] private string _previousTotalQuestionsText = "-";
    [ObservableProperty] private string _accuracyDeltaText = "-";
    [ObservableProperty] private string _totalDeltaText = "-";

    public bool HasNoLatestTest => !HasLatestTest;
    public bool HasNoPreviousTest => !HasPreviousTest;

    partial void OnHasLatestTestChanged(bool value)
    {
        OnPropertyChanged(nameof(HasNoLatestTest));
    }

    partial void OnHasPreviousTestChanged(bool value)
    {
        OnPropertyChanged(nameof(HasNoPreviousTest));
    }

    [RelayCommand]
    private void StartTest()
    {
        StatusText = string.Empty;

        if (QuestionCount <= 0)
        {
            StatusText = "题量必须大于 0";
            return;
        }

        var data = new TestSessionParameter(SelectedTestRange, QuestionCount);
        WeakReferenceMessenger.Default.Send(new NavigationMessage(ApplicationPageNames.TestSession, data));
    }

    private async Task LoadRecentTestsAsync()
    {
        var tests = await _testDataStorageService.GetRecentTests(2);

        if (tests.Length == 0)
        {
            HasLatestTest = false;
            HasPreviousTest = false;
            LatestTestTimeText = "-";
            LatestAccuracyText = "-";
            LatestTotalQuestionsText = "-";
            LatestRightWrongText = "-";
            PreviousTestTimeText = "-";
            PreviousAccuracyText = "-";
            PreviousTotalQuestionsText = "-";
            AccuracyDeltaText = "-";
            TotalDeltaText = "-";
            return;
        }

        var latest = tests[0];
        HasLatestTest = true;
        LatestTestTimeText = $"测试时间: {latest.TestTime:yyyy-MM-dd HH:mm}";
        LatestAccuracyText = $"准确率: {latest.Accuracy:P0}";
        LatestTotalQuestionsText = $"题量: {latest.TotalQuestions}";

        var rightCount = (int)Math.Round(latest.Accuracy * latest.TotalQuestions);
        rightCount = Math.Clamp(rightCount, 0, latest.TotalQuestions);
        LatestRightWrongText = $"正确/错误: {rightCount}/{latest.TotalQuestions - rightCount}";

        if (tests.Length > 1)
        {
            var previous = tests[1];
            HasPreviousTest = true;
            PreviousTestTimeText = $"上次时间: {previous.TestTime:yyyy-MM-dd HH:mm}";
            PreviousAccuracyText = $"上次准确率: {previous.Accuracy:P0}";
            PreviousTotalQuestionsText = $"上次题量: {previous.TotalQuestions}";

            var accuracyDelta = latest.Accuracy - previous.Accuracy;
            AccuracyDeltaText = $"准确率变化: {accuracyDelta:+0.0%;-0.0%;0.0%}";

            var totalDelta = latest.TotalQuestions - previous.TotalQuestions;
            TotalDeltaText = $"题量变化: {totalDelta:+#;-#;0}";
        }
        else
        {
            HasPreviousTest = false;
            PreviousTestTimeText = "-";
            PreviousAccuracyText = "-";
            PreviousTotalQuestionsText = "-";
            AccuracyDeltaText = "-";
            TotalDeltaText = "-";
        }
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
    private async Task LoadMistakesAsync()
    {
        var mistakes = await _testDataStorageService.GetRecentTests(1);
        var values = mistakes.Select(t => t.Id).ToList();
        var mistakeData = await _mistakeDataStorageService.LoadMistakeDataByWordIds(values.ToArray());
        
        Mistakes = new ObservableCollection<MistakeDataModel>(mistakeData);
    }
}