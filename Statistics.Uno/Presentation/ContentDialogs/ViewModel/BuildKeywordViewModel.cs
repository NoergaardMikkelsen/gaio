using CommunityToolkit.Mvvm.ComponentModel;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;

namespace Statistics.Uno.Presentation.ContentDialogs.ViewModel;

public partial class BuildKeywordViewModel : ObservableObject
{
    public BuildKeywordViewModel(IKeyword keyword)
    {
        Id = keyword.Id;
        Text = keyword.Text;
        Version = keyword.Version;
        CreatedDateTime = keyword.CreatedDateTime;
        UpdatedDateTime = keyword.UpdatedDateTime;
        UseRegex = keyword.UseRegex;
    }

    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private string text;

    [ObservableProperty]
    private uint version;

    [ObservableProperty]
    private DateTime createdDateTime;

    [ObservableProperty]
    private DateTime updatedDateTime;

    [ObservableProperty]
    private bool useRegex;
}

