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

public partial class TestsPageViewModel : PageViewModel
{
    private List<Word> _testWords = new();
    private List<ProblemData> _problemDatas = new();
    
    private readonly ITestDataStorageService _testDataStorageService;
    private readonly IMistakeDataStorageService _mistakeDataStorageService;
    private readonly ITestWordGetter _testWordGetter;
    
    private int _currentQuestionIndex;
    private Stopwatch _questionStopwatch = new();
    private Word? _currentWord;
    private Random _random = new();

    // 测试设置相关属性
    [ObservableProperty] private bool _isSettingsVisible = true;
    [ObservableProperty] private bool _isTestingVisible = false;
    [ObservableProperty] private int _questionCount = 10;
    [ObservableProperty] private TestRange _selectedTestRange = TestRange.Cet4;
    
    // 测试进行中相关属性
    [ObservableProperty] private int _currentQuestionNumber;
    [ObservableProperty] private int _totalQuestions;
    [ObservableProperty] private string _currentWordDisplay = string.Empty;
    [ObservableProperty] private string _currentTranslationDisplay = string.Empty;
    [ObservableProperty] private string _questionTypeText = string.Empty;
    
    // 选择题相关
    [ObservableProperty] private bool _isChoiceQuestion;
    [ObservableProperty] private ObservableCollection<ChoiceOption> _choiceOptions = new();
    
    // 填空题相关
    [ObservableProperty] private bool _isFillQuestion;
    [ObservableProperty] private string _fillHint = string.Empty;
    [ObservableProperty] private string _userInput = string.Empty;
    [ObservableProperty] private bool _showFillResult;
    [ObservableProperty] private bool _isFillCorrect;
    [ObservableProperty] private string _correctAnswer = string.Empty;

    // 可用的测试范围列表
    public ObservableCollection<TestRangeItem> TestRanges { get; } = new()
    {
        new TestRangeItem { Range = TestRange.Primary, DisplayName = "小学词汇" },
        new TestRangeItem { Range = TestRange.Senior, DisplayName = "高中词汇" },
        new TestRangeItem { Range = TestRange.Cet4, DisplayName = "四级词汇" },
        new TestRangeItem { Range = TestRange.Cet6, DisplayName = "六级词汇" },
        new TestRangeItem { Range = TestRange.Toefl, DisplayName = "托福词汇" },
        new TestRangeItem { Range = TestRange.Ielts, DisplayName = "雅思词汇" }
    };

    [ObservableProperty] private TestRangeItem? _selectedTestRangeItem;

    public TestsPageViewModel(ITestWordGetter testWordGetter,
                              ITestDataStorageService testDataStorageService,
                              IMistakeDataStorageService mistakeDataStorageService)
    {
        _testWordGetter = testWordGetter;
        _testDataStorageService = testDataStorageService;
        _mistakeDataStorageService = mistakeDataStorageService;
        PageNames = ApplicationPageNames.Tests;
        SelectedTestRangeItem = TestRanges[2]; // 默认CET4
    }

    [RelayCommand]
    private async Task StartTestAsync()
    {
        if (SelectedTestRangeItem == null) return;
        
        _testWords = await _testWordGetter.GetTestWords(QuestionCount * 2, SelectedTestRangeItem.Range);
        if (_testWords.Count < 4)
        {
            // 单词数量不足
            return;
        }
        
        _problemDatas.Clear();
        _currentQuestionIndex = 0;
        TotalQuestions = Math.Min(QuestionCount, _testWords.Count / 2);
        
        IsSettingsVisible = false;
        IsTestingVisible = true;
        
        ShowNextQuestion();
    }

    private void ShowNextQuestion()
    {
        if (_currentQuestionIndex >= TotalQuestions)
        {
            EndTest();
            return;
        }

        _currentWord = _testWords[_currentQuestionIndex];
        CurrentQuestionNumber = _currentQuestionIndex + 1;
        
        // 随机决定题目类型: 0 = 选择正确翻译, 1 = 填空补全单词
        var questionType = _random.Next(2);
        
        if (questionType == 0)
        {
            SetupChoiceQuestion();
        }
        else
        {
            SetupFillQuestion();
        }
        
        _questionStopwatch.Restart();
    }

    private void SetupChoiceQuestion()
    {
        IsChoiceQuestion = true;
        IsFillQuestion = false;
        QuestionTypeText = "选择正确的翻译";
        CurrentWordDisplay = _currentWord?.word ?? "";
        CurrentTranslationDisplay = "";
        ShowFillResult = false;
        UserInput = "";
        
        // 生成选项
        ChoiceOptions.Clear();
        var correctTranslation = _currentWord?.translation?.Replace("\\n", "\n") ?? "";
        
        // 获取3个错误选项
        var wrongOptions = _testWords
            .Where(w => w.word != _currentWord?.word)
            .OrderBy(_ => _random.Next())
            .Take(3)
            .Select(w => w.translation?.Replace("\\n", "\n") ?? "")
            .ToList();
        
        // 合并并打乱
        var allOptions = new List<string> { correctTranslation };
        allOptions.AddRange(wrongOptions);
        allOptions = allOptions.OrderBy(_ => _random.Next()).ToList();
        
        for (int i = 0; i < allOptions.Count; i++)
        {
            ChoiceOptions.Add(new ChoiceOption
            {
                Index = i,
                Text = allOptions[i],
                IsCorrect = allOptions[i] == correctTranslation
            });
        }
    }

    private void SetupFillQuestion()
    {
        IsChoiceQuestion = false;
        IsFillQuestion = true;
        QuestionTypeText = "根据翻译补全单词";
        CurrentTranslationDisplay = _currentWord?.translation?.Replace("\\n", "\n") ?? "";
        ShowFillResult = false;
        UserInput = "";
        CorrectAnswer = _currentWord?.word ?? "";
        
        // 生成填空提示 (隐藏部分字母)
        var word = _currentWord?.word ?? "";
        if (word.Length <= 2)
        {
            FillHint = new string('_', word.Length);
            CurrentWordDisplay = FillHint;
        }
        else
        {
            var chars = word.ToCharArray();
            var hideCount = Math.Max(1, word.Length / 2);
            var indicesToHide = Enumerable.Range(0, word.Length)
                .OrderBy(_ => _random.Next())
                .Take(hideCount)
                .ToList();
            
            foreach (var idx in indicesToHide)
            {
                chars[idx] = '_';
            }
            
            FillHint = new string(chars);
            CurrentWordDisplay = FillHint;
        }
    }

    [RelayCommand]
    private void SelectChoice(ChoiceOption option)
    {
        var elapsed = _questionStopwatch.ElapsedMilliseconds;
        _questionStopwatch.Stop();
        
        _problemDatas.Add(new ProblemData
        {
            Word = _currentWord?.word ?? "",
            Translation = _currentWord?.translation ?? "",
            CostTimeMs = elapsed,
            IsRight = option.IsCorrect
        });
        
        _currentQuestionIndex++;
        ShowNextQuestion();
    }

    [RelayCommand]
    private void SubmitFillAnswer()
    {
        var elapsed = _questionStopwatch.ElapsedMilliseconds;
        _questionStopwatch.Stop();
        
        var isCorrect = string.Equals(UserInput?.Trim(), _currentWord?.word, StringComparison.OrdinalIgnoreCase);
        IsFillCorrect = isCorrect;
        ShowFillResult = true;
        
        _problemDatas.Add(new ProblemData
        {
            Word = _currentWord?.word ?? "",
            Translation = _currentWord?.translation ?? "",
            CostTimeMs = elapsed,
            IsRight = isCorrect
        });
    }

    [RelayCommand]
    private void NextQuestion()
    {
        _currentQuestionIndex++;
        ShowFillResult = false;
        ShowNextQuestion();
    }

    private async void EndTest()
    {
        IsTestingVisible = false;
        
        // 保存测试数据
        var testData = new TestDataModel
        {
            TestTime = DateTime.Now,
            TotalQuestions = _problemDatas.Count,
            Accuracy = _problemDatas.Count > 0 
                ? (double)_problemDatas.Count(p => p.IsRight) / _problemDatas.Count 
                : 0,
            Mistakes = new List<int>()
        };
        
        await _testDataStorageService.SaveTestData(new[] { testData });
        
        // 导航到Summary页面
        WeakReferenceMessenger.Default.Send(new NavigationMessage(ApplicationPageNames.Summary, _problemDatas.ToList()));
        
        // 重置设置界面
        IsSettingsVisible = true;
    }

    [RelayCommand]
    private void CancelTest()
    {
        IsTestingVisible = false;
        IsSettingsVisible = true;
        _problemDatas.Clear();
    }
}

public record ProblemData
{
    public string Word { get; init; } = "";
    public string Translation { get; init; } = "";
    public long CostTimeMs { get; init; }
    public bool IsRight { get; init; }
}

public class ChoiceOption
{
    public int Index { get; set; }
    public string Text { get; set; } = "";
    public bool IsCorrect { get; set; }
}

public class TestRangeItem
{
    public TestRange Range { get; set; }
    public string DisplayName { get; set; } = "";
}