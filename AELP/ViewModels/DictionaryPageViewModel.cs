using AELP.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AELP.ViewModels;

public partial class DictionaryPageViewModel : PageViewModel
{
    private readonly IWordQueryService _wordQueryService;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _searchResult = string.Empty;

    [ObservableProperty]
    private ObservableCollection<string> _examTags = new();

    [ObservableProperty]
    private double _contentBlurRadius = 0;

    public DictionaryPageViewModel(IWordQueryService wordQueryService)
    {
        PageNames = Data.ApplicationPageNames.Dictionary;
        _wordQueryService = wordQueryService;
        // Apply blur initially as no result is found yet? 
        // Or start clear? User said "when not found... blur".
        // Let's assume start with blur if "not found" is the default state after a failed search.
        // But initially it's empty.
        ContentBlurRadius = 0; 
    }

    [RelayCommand]
    private void SearchTranslation()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            ContentBlurRadius = 20;
            return;
        }

        var resultDictionary = _wordQueryService.QueryWordInfo(SearchText);

        if (resultDictionary != null)
        {
            // 搜出的文本包含\n, 需要处理一下
            // SearchResult = resultDictionary.translation ?? string.Empty;
            SearchResult = resultDictionary.translation?.Replace("\\n", "\n") ?? string.Empty;
            ExamTags.Clear();
            if (resultDictionary.cet4 == 1) ExamTags.Add("CET4");
            if (resultDictionary.cet6 == 1) ExamTags.Add("CET6");
            if (resultDictionary.hs == 1) ExamTags.Add("High School");
            if (resultDictionary.ph == 1) ExamTags.Add("Primary School");
            if (resultDictionary.tf == 1) ExamTags.Add("TOEFL");
            if (resultDictionary.ys == 1) ExamTags.Add("IELTS");

            ContentBlurRadius = 0;
        }
        else
        {
            // Not found
            ContentBlurRadius = 20;
            // Optionally clear result or keep previous? 
            // If I clear result, blur is invisible. 
            // "Blur the content below". Maybe the user implies the content is still there?
            // I'll keep the previous result but blur it to indicate "not this one" or just invalid state.
            // Or maybe I should show "Not Found" and blur THAT? 
            // Let's set message to "Not Found" and blur it? That seems counter-intuitive (you want to read "Not Found").
            
            // Let's assume the user wants to obscure the area.
            SearchResult = "未找到该单词"; 
            ExamTags.Clear();
        }
    }
}