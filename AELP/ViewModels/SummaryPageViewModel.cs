using System.Collections.Generic;

namespace AELP.ViewModels;

public class SummaryPageViewModel : PageViewModel
{
    List<ProblemData> _problems;
    public SummaryPageViewModel()
    {
        _problems = new List<ProblemData>();
        PageNames = Data.ApplicationPageNames.Summary;
    }

    public override void SetParameter(object parameter)
    {
        if (parameter is List<ProblemData> problems)
        {
            _problems = problems;
        }
    }
}