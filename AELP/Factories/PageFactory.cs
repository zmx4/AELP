using System;
using AELP.Data;
using AELP.ViewModels;

namespace AELP.Factories;

public class PageFactory(Func<ApplicationPageNames, PageViewModel> factory)
{
    public PageViewModel GetPageViewModel(ApplicationPageNames pageName, Object? parameter = null)
    {
        var page = factory.Invoke(pageName);
        if (parameter is not null)
        {
            page.SetParameter(parameter);
        }
        return page;
    }
}