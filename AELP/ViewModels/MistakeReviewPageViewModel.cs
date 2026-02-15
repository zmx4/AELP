using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AELP.Data;
using AELP.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AELP.ViewModels;

/// <summary>
/// 错题复习页面视图模型，负责复习流程与结果提交。
/// </summary>
public partial class MistakeReviewPageViewModel : PageViewModel
{
    private readonly List<MistakeDataModel> _mistakes = new();
    private readonly Dictionary<int, int> _countDeltas = new();
    private string _currentWord = string.Empty;
    private int _rightCount;

    private readonly IMistakeDataStorageService _mistakeDataStorageService;
    
    [ObservableProperty] private bool _isReviewing;
    [ObservableProperty] private int _currentIndex;
    [ObservableProperty] private string _currentTranslation = string.Empty;
    [ObservableProperty] private string _userInput = string.Empty;
    [ObservableProperty] private string _progressText = string.Empty;
    [ObservableProperty] private string _statusText = string.Empty;
    [ObservableProperty] private int _totalCount;
    [ObservableProperty] private bool _hasMistakes;

    public bool IsNotReviewing => !IsReviewing;
    public bool IsEmpty => !HasMistakes;

    /// <summary>
    /// 初始化 <see cref="MistakeReviewPageViewModel"/>。
    /// </summary>
    /// <param name="mistakeDataStorageService">错题存储服务。</param>
    public MistakeReviewPageViewModel(IMistakeDataStorageService mistakeDataStorageService)
    {
        PageNames = ApplicationPageNames.MistakeReview;
        _mistakeDataStorageService = mistakeDataStorageService;
    }

    partial void OnIsReviewingChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotReviewing));
    }

    partial void OnHasMistakesChanged(bool value)
    {
        OnPropertyChanged(nameof(IsEmpty));
    }

    /// <summary>
    /// 设置复习数据参数。
    /// </summary>
    /// <param name="parameter">页面参数，期望为错题数组。</param>
    public override void SetParameter(object parameter)
    {
        if (parameter is not MistakeDataModel[] mistakeDataModels) return;

        _mistakes.Clear();
        
        _mistakes.AddRange(mistakeDataModels);
        _countDeltas.Clear();
        TotalCount = _mistakes.Count;
        HasMistakes = TotalCount > 0;
        ResetReviewState();

        if (HasMistakes)
        {
            StartReview();
        }
        else
        {
            StatusText = "暂无可复习的错误单词";
        }
    }

    /// <summary>
    /// 开始错题复习。
    /// </summary>
    [RelayCommand]
    private void StartReview()
    {
        if (!HasMistakes)
        {
            StatusText = "暂无可复习的错误单词";
            return;
        }

        _rightCount = 0;
        CurrentIndex = 0;
        IsReviewing = true;
        StatusText = string.Empty;
        SetupQuestion();
    }

    /// <summary>
    /// 提交当前答案。
    /// </summary>
    /// <returns>表示提交流程完成的异步任务。</returns>
    [RelayCommand]
    private async Task SubmitAnswer()
    {
        if (!IsReviewing || CurrentIndex < 0 || CurrentIndex >= _mistakes.Count) return;

        var input = (UserInput ?? string.Empty).Trim();
        var isRight = string.Equals(input, _currentWord, StringComparison.OrdinalIgnoreCase);
        var current = _mistakes[CurrentIndex];
        if (isRight)
        {
            _rightCount++;
            StatusText = "正确";
            AdjustCount(current, -1);
        }
        else
        {
            StatusText = $"错误，正确答案: {_currentWord}";
            AdjustCount(current, 1);
        }

        await AdvanceToNext();
    }

    private void SetupQuestion()
    {
        if (CurrentIndex < 0 || CurrentIndex >= _mistakes.Count) return;

        var current = _mistakes[CurrentIndex];
        _currentWord = current.Word ?? string.Empty;
        var translation = current.Translation ?? "无翻译";
        CurrentTranslation = NormalizeTranslation(translation);
        UserInput = string.Empty;
        ProgressText = $"{CurrentIndex + 1}/{_mistakes.Count}";
    }

    private static string NormalizeTranslation(string translation)
    {
        return translation
            .Replace("\\r\\n", "\n", StringComparison.Ordinal)
            .Replace("\\n", "\n", StringComparison.Ordinal);
    }

    private async Task AdvanceToNext()
    {
        CurrentIndex++;
        if (CurrentIndex >= _mistakes.Count)
        {
            await EndReview();
            return;
        }

        SetupQuestion();
    }

    private async Task EndReview()
    {
        IsReviewing = false;
        ProgressText = string.Empty;
        StatusText = $"复习完成：正确 {_rightCount}/{_mistakes.Count}";

        if (_countDeltas.Count > 0)
        {
            var updates = _mistakes
                .Where(m => m.Id != 0 && _countDeltas.ContainsKey(m.Id))
                .ToArray();

            if (updates.Length > 0)
            {
                await _mistakeDataStorageService.UpdateMistakeData(updates);
            }
        }
    }

    private void AdjustCount(MistakeDataModel current, int delta)
    {
        if (current.Id == 0) return;

        if (_countDeltas.TryGetValue(current.Id, out var existing))
        {
            _countDeltas[current.Id] = existing + delta;
        }
        else
        {
            _countDeltas[current.Id] = delta;
        }

        current.Count = Math.Max(0, current.Count + delta);
    }

    private void ResetReviewState()
    {
        IsReviewing = false;
        CurrentIndex = 0;
        CurrentTranslation = string.Empty;
        UserInput = string.Empty;
        ProgressText = string.Empty;
        StatusText = string.Empty;
        _currentWord = string.Empty;
        _rightCount = 0;
    }
}