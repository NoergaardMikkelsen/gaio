using Statistics.Shared.Abstraction.Interfaces.Models.Entity;

namespace Statistics.Uno.Presentation.ContentDialogs.ViewModel;

public partial class BuildKeywordViewModel : ObservableObject
{
    [ObservableProperty] private DateTime createdDateTime;

    [ObservableProperty] private DateTime? endSearch;

    [ObservableProperty] private int id;

    [ObservableProperty] private DateTime? startSearch;

    [ObservableProperty] private string text;

    [ObservableProperty] private DateTime updatedDateTime;

    [ObservableProperty] private bool useRegex;

    [ObservableProperty] private uint version;

    public BuildKeywordViewModel(IKeyword keyword)
    {
        Id = keyword.Id;
        Text = keyword.Text;
        Version = keyword.Version;
        CreatedDateTime = keyword.CreatedDateTime;
        UpdatedDateTime = keyword.UpdatedDateTime;
        UseRegex = keyword.UseRegex;
        StartSearch = keyword.StartSearch;
        EndSearch = keyword.EndSearch;
    }
}
