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

/// <summary>
/// 测试会话页面视图模型，负责出题、答题与结果持久化。
/// </summary>
public partial class TestSessionPageViewModel : PageViewModel
{
    private List<Word> _testWords = new();
    private readonly List<ProblemData> _problemData = new();
    private readonly List<MistakeDataModel> _mistakeData = new();
    private readonly Stopwatch _stopwatch = new();
    private readonly Stopwatch _totalStopwatch = new();
    private Avalonia.Threading.DispatcherTimer? _timer;
    private readonly Random _random = new();
    private Word? _currentWord;
    private string _missingPart = string.Empty;

    private readonly ITestDataStorageService _testDataStorageService;
    private readonly IMistakeDataStorageService _mistakeDataStorageService;
    private readonly ITestWordGetter _testWordGetter;
    private readonly IKeyboardPreferenceService _keyboardPreferenceService;

    /// <summary>
    /// 初始化 <see cref="TestSessionPageViewModel"/>。
    /// </summary>
    /// <param name="testWordGetter">测试单词获取服务。</param>
    /// <param name="testDataStorageService">测试数据存储服务。</param>
    /// <param name="mistakeDataStorageService">错题数据存储服务。</param>
    /// <param name="keyboardPreferenceService">按键偏好服务。</param>
    public TestSessionPageViewModel(ITestWordGetter testWordGetter,
        ITestDataStorageService testDataStorageService,
        IMistakeDataStorageService mistakeDataStorageService,
        IKeyboardPreferenceService keyboardPreferenceService)
    {
        _distractors = [];
        
        _testWordGetter = testWordGetter;
        _testDataStorageService = testDataStorageService;
        _mistakeDataStorageService = mistakeDataStorageService;
        _keyboardPreferenceService = keyboardPreferenceService;
        PageNames = ApplicationPageNames.TestSession;

        TestRanges = new ObservableCollection<TestRange>((TestRange[])Enum.GetValues(typeof(TestRange)));
        QuestionCounts = [5, 10, 15, 20];
        SelectedTestRange = TestRange.Cet4;
        QuestionCount = 10;
        StatusText = string.Empty;
        ChoiceKeyMapping = _keyboardPreferenceService.GetChoiceOptionKeys();
        
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
    [ObservableProperty] private ObservableCollection<OptionItem> _options = new();
    [ObservableProperty] private string _progressText = string.Empty;
    [ObservableProperty] private string _statusText = string.Empty;
    [ObservableProperty] private string _choiceKeyMapping = string.Empty;
    [ObservableProperty] private string _totalTimeText = "00:00";
    [ObservableProperty] private string _currentTimeText = "00:00";

    private List<string> _distractors;

    public bool IsNotTesting => !IsTesting;
    public bool IsFillQuestion => IsTesting && !IsChoiceQuestion;

    /// <summary>
    /// 设置测试会话参数并启动测试。
    /// </summary>
    /// <param name="parameter">页面参数，期望为 <see cref="TestSessionParameter"/>。</param>
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

    /// <summary>
    /// 启动测试流程。
    /// </summary>
    /// <returns>表示启动流程完成的异步任务。</returns>
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
        var distractors = await _testWordGetter.GetTestWords(QuestionCount * 4, SelectedTestRange);
        _distractors = distractors.Select(w => w.Translation).Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t!).ToList();
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

        _totalStopwatch.Restart();
        _timer?.Stop();
        _timer = new Avalonia.Threading.DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };
        _timer.Tick += (s, e) =>
        {
            TotalTimeText = $"{(int)_totalStopwatch.Elapsed.TotalMinutes:D2}:{_totalStopwatch.Elapsed.Seconds:D2}";
            CurrentTimeText = $"{(int)_stopwatch.Elapsed.TotalMinutes:D2}:{_stopwatch.Elapsed.Seconds:D2}";
        };
        _timer.Start();

        SetupQuestion();
    }

    /// <summary>
    /// 提交选择题答案。
    /// </summary>
    /// <param name="option">用户选择的选项文本。</param>
    /// <returns>表示提交完成的异步任务。</returns>
    [RelayCommand]
    private async Task ChooseOptionAsync(string option)
    {
        if (!IsTesting || !IsChoiceQuestion || _currentWord is null) return;

        _stopwatch.Stop();
        var isRight = string.Equals(option, CurrentTranslation, StringComparison.OrdinalIgnoreCase);
        RecordAnswer(isRight);
        await AdvanceToNextQuestionAsync();
    }

    /// <summary>
    /// 提交填空题答案。
    /// </summary>
    /// <returns>表示提交完成的异步任务。</returns>
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
        CurrentWordText = _currentWord.RawWord;
        CurrentTranslation = _currentWord.Translation ?? "无翻译";

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
        
        // 正确答案
        var correct = CurrentTranslation;
        
        // var translations = _testWords
        //     .Select(w => w.Translation)
        //     .Where(t => !string.IsNullOrWhiteSpace(t))
        //     .Select(t => t!)
        //     .Distinct(StringComparer.OrdinalIgnoreCase)
        //     .ToList();
        //
        // var distractors = translations
        //     .Where(t => !string.Equals(t, correct, StringComparison.OrdinalIgnoreCase))
        //     .OrderBy(_ => _random.Next())
        //     .Take(3)
        //     .ToList();
        
        var distractors = _distractors
            .Where(t => !string.Equals(t, correct, StringComparison.OrdinalIgnoreCase))
            .OrderBy(_ => _random.Next())
            .Take(3)
            .ToList();
        
        _distractors.RemoveAll(t => distractors.Contains(t, StringComparer.OrdinalIgnoreCase));

        var options = new List<string> { correct };
        options.AddRange(distractors);

        var choiceKeys = _keyboardPreferenceService.GetChoiceOptionKeys();
        ChoiceKeyMapping = choiceKeys;

        var optionList = options.Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(_ => _random.Next())
            .ToList();

        for (var i = 0; i < optionList.Count; i++)
        {
            var keyLabel = i < choiceKeys.Length
                ? char.ToUpperInvariant(choiceKeys[i]).ToString()
                : string.Empty;
            Options.Add(new OptionItem(keyLabel, optionList[i]));
        }
    }

    private void BuildPartialWord()
    {
        if (_currentWord is null) return;

        var word = _currentWord.RawWord ?? string.Empty;
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
            Word = _currentWord.RawWord,
            Translation = CurrentTranslation,
            CostTimeMs = elapsed,
            IsRight = isRight
        });

        if (!isRight)
        {
            _mistakeData.Add(new MistakeDataModel
            {
                Word = _currentWord.RawWord,
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
        _timer?.Stop();
        _totalStopwatch.Stop();
        _stopwatch.Stop();

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

    /// <summary>
    /// 尝试处理选择题快捷键输入。
    /// </summary>
    /// <param name="keyChar">输入字符。</param>
    /// <returns>若成功处理返回 <see langword="true"/>；否则返回 <see langword="false"/>。</returns>
    public async Task<bool> TryHandleChoiceKeyAsync(char keyChar)
    {
        if (!IsTesting || !IsChoiceQuestion)
        {
            return false;
        }

        var mapping = _keyboardPreferenceService.GetChoiceOptionKeys();
        var index = GetChoiceKeyIndex(mapping, keyChar);
        if (index < 0 || index >= Options.Count)
        {
            return false;
        }

        await ChooseOptionAsync(Options[index].Text);
        return true;
    }

    private static int GetChoiceKeyIndex(string mapping, char keyChar)
    {
        if (string.IsNullOrWhiteSpace(mapping))
        {
            return -1;
        }

        var normalized = mapping.Trim().ToLowerInvariant();
        var key = char.ToLowerInvariant(keyChar);
        return normalized.IndexOf(key);
    }
}

/// <summary>
/// 测试会话参数。
/// </summary>
/// <param name="Range">测试范围。</param>
/// <param name="QuestionCount">题目数量。</param>
public record TestSessionParameter(TestRange Range, int QuestionCount);

/// <summary>
/// 单题作答统计数据。
/// </summary>
public record ProblemData
{
    /// <summary>单词文本。</summary>
    public string Word { get; init; } = string.Empty;
    /// <summary>单词译文。</summary>
    public string Translation { get; init; } = string.Empty;
    /// <summary>耗时（毫秒）。</summary>
    public long CostTimeMs { get; init; }
    /// <summary>是否答对。</summary>
    public bool IsRight { get; init; }
}

/// <summary>
/// 选择题选项数据。
/// </summary>
/// <param name="KeyLabel">按键标签。</param>
/// <param name="Text">选项文本。</param>
public record OptionItem(string KeyLabel, string Text);
