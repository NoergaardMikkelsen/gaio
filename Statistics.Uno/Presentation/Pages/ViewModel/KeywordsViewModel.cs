using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using System.Collections.Generic;

namespace Statistics.Uno.Presentation.Pages.ViewModel;

public partial class KeywordsViewModel : ObservableObject
{
    [ObservableProperty] private IEnumerable<IKeyword> keywords;
    [ObservableProperty] private string? searchableKeywordText;
    [ObservableProperty] private string? updatingText;

    public event EventHandler<string>? SearchableKeywordTextChanged;

    partial void OnSearchableKeywordTextChanged(string? value)
    {
        SearchableKeywordTextChanged?.Invoke(this, value ?? string.Empty);
    }
}
