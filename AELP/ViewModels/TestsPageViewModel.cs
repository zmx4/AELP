using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AELP.Data;
using AELP.Messages;
using AELP.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AELP.ViewModels;

public partial class TestsPageViewModel : PageViewModel
{
    private readonly ITestDataStorageService _testDataStorageService;

    public TestsPageViewModel(ITestDataStorageService testDataStorageService)
    {
        _testDataStorageService = testDataStorageService;
        PageNames = Data.ApplicationPageNames.Tests;

        TestRanges = new ObservableCollection<TestRange>((TestRange[])Enum.GetValues(typeof(TestRange)));
        QuestionCounts = [5, 10, 15, 20];
        SelectedTestRange = TestRange.Cet4;
        QuestionCount = 10;
        StatusText = string.Empty;

        _ = LoadRecentTestsAsync();
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
}