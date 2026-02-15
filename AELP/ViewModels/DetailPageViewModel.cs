using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AELP.Data;
using AELP.Models;
using AELP.Services;
using CommunityToolkit.Mvvm.Input;

namespace AELP.ViewModels;

/// <summary>
/// 单词详情页面视图模型。
/// </summary>
public partial class DetailPageViewModel : PageViewModel
{
    private readonly IFavoritesDataStorageService _dataStorageService;
    
    private Dictionary? _word;

    public Dictionary? Word
    {
        get => _word;
        private set
        {
            if (SetProperty(ref _word, value))
            {
                UpdateTags();
            }
        }
    }

    public ObservableCollection<string> Tags { get; } = new();

    /// <summary>
    /// 初始化 <see cref="DetailPageViewModel"/>。
    /// </summary>
    /// <param name="dataStorageService">收藏存储服务。</param>
    public DetailPageViewModel(IFavoritesDataStorageService dataStorageService)
    {
        PageNames = ApplicationPageNames.Detail;
        _dataStorageService = dataStorageService;
    }

    /// <summary>
    /// 设置详情页参数。
    /// </summary>
    /// <param name="parameter">页面参数，期望为 <see cref="Dictionary"/>。</param>
    public override void SetParameter(object parameter)
    {
        if (parameter is Dictionary word)
        {
            Word = word;
            word.Translation = word.Translation?.Replace("\\n", "\n");
        }
    }

    private void UpdateTags()
    {
        Tags.Clear();
        if (Word is null) return;

        if (Word.Cet4 > 0) Tags.Add("CET4");
        if (Word.Cet6 > 0) Tags.Add("CET6");
        if (Word.Hs > 0) Tags.Add("高中");
        if (Word.Ph > 0) Tags.Add("小学");
        if (Word.Tf > 0) Tags.Add("托福");
        if (Word.Ys > 0) Tags.Add("雅思");
    }
    /// <summary>
    /// 将当前单词添加到收藏。
    /// </summary>
    /// <returns>表示添加完成的异步任务。</returns>
    [RelayCommand]
    private async Task AddToFavoritesAsync()
    {
        if (Word is null) return;

        await _dataStorageService.AddToFavorites(Word);
    }
}