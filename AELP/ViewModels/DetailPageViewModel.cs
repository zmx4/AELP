using System.Collections.ObjectModel;
using AELP.Data;
using AELP.Models;

namespace AELP.ViewModels;

public class DetailPageViewModel : PageViewModel
{
    private dictionary? _word;

    public dictionary? Word
    {
        get => _word;
        set
        {
            if (SetProperty(ref _word, value))
            {
                UpdateTags();
            }
        }
    }

    public ObservableCollection<string> Tags { get; } = new();

    public DetailPageViewModel()
    {
        PageNames = ApplicationPageNames.Detail;
    }

    public override void SetParameter(object parameter)
    {
        if (parameter is dictionary word)
        {
            Word = word;
        }
    }

    private void UpdateTags()
    {
        Tags.Clear();
        if (Word == null) return;

        if (Word.cet4 > 0) Tags.Add("CET4");
        if (Word.cet6 > 0) Tags.Add("CET6");
        if (Word.hs > 0) Tags.Add("高中");
        if (Word.ph > 0) Tags.Add("小学");
        if (Word.tf > 0) Tags.Add("托福");
        if (Word.ys > 0) Tags.Add("雅思");
    }
}