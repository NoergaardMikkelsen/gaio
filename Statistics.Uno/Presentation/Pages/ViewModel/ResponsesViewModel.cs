using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using System.Collections.Generic;

namespace Statistics.Uno.Presentation.Pages.ViewModel;

public partial class ResponsesViewModel : ObservableObject
{
    public ResponsesViewModel()
    {
        ExecuteAllPromptsButtonText = "Execute Prompts";
    }

    [ObservableProperty] private IEnumerable<IResponse> responses;
    [ObservableProperty] private string executeAllPromptsButtonText;
    [ObservableProperty] private string? searchableResponseText;

    public event EventHandler<string>? SearchableResponseTextChanged;

    partial void OnSearchableResponseTextChanged(string? value)
    {
        SearchableResponseTextChanged?.Invoke(this, value ?? string.Empty);
    }
}
