using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AELP.Data;
using AELP.Messages;
using AELP.Models;
using AELP.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AELP.ViewModels;

public partial class TestSessionPageViewModel : PageViewModel
{
    private List<Word> _testWords = new();
    private readonly List<ProblemData> _problemData = new();
    private readonly List<MistakeDataModel> _mistakeData = new();
    private readonly Stopwatch _stopwatch = new();
    private readonly Random _random = new();
    private Word? _currentWord;
    private string _missingPart = string.Empty;

    private readonly ITestDataStorageService _testDataStorageService;
    private readonly IMistakeDataStorageService _mistakeDataStorageService;
    private readonly ITestWordGetter _testWordGetter;

    public TestSessionPageViewModel(ITestWordGetter testWordGetter,
        ITestDataStorageService testDataStorageService,
        IMistakeDataStorageService mistakeDataStorageService)
    {
        _testWordGetter = testWordGetter;
        _testDataStorageService = testDataStorageService;
        _mistakeDataStorageService = mistakeDataStorageService;
        PageNames = Data.ApplicationPageNames.TestSession;

        TestRanges = new ObservableCollection<TestRange>((TestRange[])Enum.GetValues(typeof(TestRange)));
        QuestionCounts = [5, 10, 15, 20];
        SelectedTestRange = TestRange.Cet4;
        QuestionCount = 10;
        StatusText = string.Empty;
    }

    [ObservableProperty] private ObservableCollection<TestRange> _testRanges = new();
    [ObservableProperty] private TestRange _selectedTestRange = TestRange.Cet4;
    [ObservableProperty] private ObservableCollection<int> _questionCounts = new();
    [ObservableProperty] private int _questionCount = 10;
    [ObservableProperty] private bool _isTesting;
    [ObservableProperty] private bool _isChoiceQuestion;
    [ObservableProperty] private int _currentIndex;
    [ObservableProperty] private string _currentWordText = string.Empty;
    [ObservableProperty] private string _currentTranslation = string.Empty;
    [ObservableProperty] private string _partialWord = string.Empty;
    [ObservableProperty] private string _fillInput = string.Empty;
    [ObservableProperty] private ObservableCollection<string> _options = new();
    [ObservableProperty] private string _progressText = string.Empty;
    [ObservableProperty] private string _statusText = string.Empty;

    public bool IsNotTesting => !IsTesting;
    public bool IsFillQuestion => IsTesting && !IsChoiceQuestion;

    public override void SetParameter(object parameter)
    {
        if (parameter is TestSessionParameter data)
        {
            SelectedTestRange = data.Range;
            QuestionCount = data.QuestionCount;
            _ = StartTestAsync();
        }
    }

    partial void OnIsTestingChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotTesting));
        OnPropertyChanged(nameof(IsFillQuestion));
    }

    partial void OnIsChoiceQuestionChanged(bool value)
    {
        OnPropertyChanged(nameof(IsFillQuestion));
    }

    [RelayCommand]
    private async Task StartTestAsync()
    {
        StatusText = string.Empty;

        if (QuestionCount <= 0)
        {
            StatusText = "题量必须大于 0";
            return;
        }

        _testWords = await _testWordGetter.GetTestWords(QuestionCount, SelectedTestRange);
        if (_testWords.Count == 0)
        {
            StatusText = "未获取到测试单词";
            return;
        }

        _problemData.Clear();
        _mistakeData.Clear();
        CurrentIndex = 0;
        IsTesting = true;
        IsChoiceQuestion = true;

        SetupQuestion();
    }

    [RelayCommand]
    private async Task ChooseOptionAsync(string option)
    {
        if (!IsTesting || !IsChoiceQuestion || _currentWord is null) return;

        _stopwatch.Stop();
        var isRight = string.Equals(option, CurrentTranslation, StringComparison.OrdinalIgnoreCase);
        RecordAnswer(isRight);
        await AdvanceToNextQuestionAsync();
    }

    [RelayCommand]
    private async Task SubmitFillAsync()
    {
        if (!IsTesting || IsChoiceQuestion || _currentWord is null) return;

        _stopwatch.Stop();
        var userInput = (FillInput ?? string.Empty).Trim();
        var isRight = string.Equals(userInput, _missingPart, StringComparison.OrdinalIgnoreCase);
        RecordAnswer(isRight);
        await AdvanceToNextQuestionAsync();
    }

    private void SetupQuestion()
    {
        if (CurrentIndex < 0 || CurrentIndex >= _testWords.Count)
        {
            return;
        }

        _currentWord = _testWords[CurrentIndex];
        CurrentWordText = _currentWord.word;
        CurrentTranslation = _currentWord.translation ?? "无翻译";

        IsChoiceQuestion = CurrentIndex % 2 == 0;

        if (IsChoiceQuestion)
        {
            BuildOptions();
        }
        else
        {
            BuildPartialWord();
        }

        FillInput = string.Empty;
        ProgressText = $"{CurrentIndex + 1}/{_testWords.Count}";

        _stopwatch.Reset();
        _stopwatch.Start();
    }

    private void BuildOptions()
    {
        Options.Clear();
        var correct = CurrentTranslation;
        var translations = _testWords
            .Select(w => w.translation)
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var distractors = translations
            .Where(t => !string.Equals(t, correct, StringComparison.OrdinalIgnoreCase))
            .OrderBy(_ => _random.Next())
            .Take(3)
            .ToList();

        var options = new List<string> { correct };
        options.AddRange(distractors);

        foreach (var opt in options.Distinct(StringComparer.OrdinalIgnoreCase)
                     .OrderBy(_ => _random.Next()))
        {
            Options.Add(opt);
        }
    }

    private void BuildPartialWord()
    {
        if (_currentWord is null) return;

        var word = _currentWord.word ?? string.Empty;
        if (string.IsNullOrEmpty(word))
        {
            _missingPart = string.Empty;
            PartialWord = string.Empty;
            return;
        }
        var missingLength = Math.Max(1, word.Length / 2);
        var prefix = word.Substring(0, word.Length - missingLength);
        _missingPart = word.Substring(word.Length - missingLength);
        PartialWord = prefix + new string('_', missingLength);
    }

    private void RecordAnswer(bool isRight)
    {
        if (_currentWord is null) return;

        var elapsed = _stopwatch.ElapsedMilliseconds;
        _problemData.Add(new ProblemData
        {
            Word = _currentWord.word,
            Translation = CurrentTranslation,
            CostTimeMs = elapsed,
            IsRight = isRight
        });

        if (!isRight)
        {
            _mistakeData.Add(new MistakeDataModel
            {
                Word = _currentWord.word,
                Time = DateTime.Now,
                Count = 1
            });
        }
    }

    private async Task AdvanceToNextQuestionAsync()
    {
        CurrentIndex++;
        if (CurrentIndex >= _testWords.Count)
        {
            await EndTestAsync();
            return;
        }

        SetupQuestion();
    }

    private async Task EndTestAsync()
    {
        IsTesting = false;
        ProgressText = string.Empty;

        var rightCount = _problemData.Count(p => p.IsRight);
        var accuracy = _problemData.Count == 0 ? 0 : (double)rightCount / _problemData.Count;

        var testData = new TestDataModel
        {
            TestTime = DateTime.Now,
            Accuracy = accuracy,
            TotalQuestions = _problemData.Count,
            Mistakes = new List<int>()
        };

        await _testDataStorageService.SaveTestData([testData]);

        if (_mistakeData.Count > 0)
        {
            await _mistakeDataStorageService.SaveMistakeData(_mistakeData.ToArray());
        }

        WeakReferenceMessenger.Default.Send(new NavigationMessage(ApplicationPageNames.Summary, _problemData));
    }
}

public record TestSessionParameter(TestRange Range, int QuestionCount);

public record ProblemData
{
    public string Word { get; init; } = string.Empty;
    public string Translation { get; init; } = string.Empty;
    public long CostTimeMs { get; init; }
    public bool IsRight { get; init; }
}
