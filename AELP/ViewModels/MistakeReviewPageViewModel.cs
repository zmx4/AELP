using System;
using System.Collections.Generic;
using System.Linq;
using AELP.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AELP.ViewModels;

public partial class MistakeReviewPageViewModel : PageViewModel
{
    private readonly List<MistakeDataModel> _mistakes = new();
    private string _currentWord = string.Empty;
    private int _rightCount;
    
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

    public MistakeReviewPageViewModel()
    {
        PageNames = ApplicationPageNames.MistakeReview;
    }

    partial void OnIsReviewingChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotReviewing));
    }

    partial void OnHasMistakesChanged(bool value)
    {
        OnPropertyChanged(nameof(IsEmpty));
    }

    public override void SetParameter(object parameter)
    {
        if (parameter is not MistakeDataModel[] mistakeDataModels) return;

        _mistakes.Clear();
        foreach (var mistake in mistakeDataModels)
        {
            if (string.IsNullOrWhiteSpace(mistake.Word)) continue;
            if (string.IsNullOrWhiteSpace(mistake.Translation))
            {
                mistake.Translation = "无翻译";
            }
            _mistakes.Add(mistake);
        }

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

    [RelayCommand]
    private void SubmitAnswer()
    {
        if (!IsReviewing || CurrentIndex < 0 || CurrentIndex >= _mistakes.Count) return;

        var input = (UserInput ?? string.Empty).Trim();
        var isRight = string.Equals(input, _currentWord, StringComparison.OrdinalIgnoreCase);
        if (isRight)
        {
            _rightCount++;
            StatusText = "正确";
        }
        else
        {
            StatusText = $"错误，正确答案: {_currentWord}";
        }

        AdvanceToNext();
    }

    private void SetupQuestion()
    {
        if (CurrentIndex < 0 || CurrentIndex >= _mistakes.Count) return;

        var current = _mistakes[CurrentIndex];
        _currentWord = current.Word ?? string.Empty;
        CurrentTranslation = current.Translation ?? "无翻译";
        UserInput = string.Empty;
        ProgressText = $"{CurrentIndex + 1}/{_mistakes.Count}";
    }

    private void AdvanceToNext()
    {
        CurrentIndex++;
        if (CurrentIndex >= _mistakes.Count)
        {
            EndReview();
            return;
        }

        SetupQuestion();
    }

    private void EndReview()
    {
        IsReviewing = false;
        ProgressText = string.Empty;
        StatusText = $"复习完成：正确 {_rightCount}/{_mistakes.Count}";
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