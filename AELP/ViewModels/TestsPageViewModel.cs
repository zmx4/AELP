using System.Collections.Generic;
using AELP.Models;
using AELP.Services;

namespace AELP.ViewModels;

public class TestsPageViewModel : PageViewModel
{
    private List<Word> _TestWords;
    private List<ProblemData> _problemDatas;
    
    private readonly ITestDataStorageService _testDataStorageService;
    private readonly IMistakeDataStorageService _mistakeDataStorageService;
    private readonly ITestWordGetter _testWordGetter;
    public TestsPageViewModel(ITestWordGetter testWordGetter,
                              ITestDataStorageService testDataStorageService,
                              IMistakeDataStorageService mistakeDataStorageService)
    {
        _testWordGetter = testWordGetter;
        _testDataStorageService = testDataStorageService;
        _mistakeDataStorageService = mistakeDataStorageService;
        PageNames = Data.ApplicationPageNames.Tests;
    }




    
}

public record ProblemData
{
    public string Word { get; init; }
    public string Translation { get; init; }
    public long CostTimeMs { get; init; }
    public bool IsRight { get; init; }
}