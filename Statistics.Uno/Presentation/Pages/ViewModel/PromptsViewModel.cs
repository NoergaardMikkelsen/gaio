using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using System.Collections.Generic;

namespace Statistics.Uno.Presentation.Pages.ViewModel;

public partial class PromptsViewModel : ObservableObject
{
    public PromptsViewModel()
    {
        ExecuteAllPromptsButtonText = "Execute Prompts";
    }

    [ObservableProperty] private IEnumerable<IPrompt> prompts;
    [ObservableProperty] private string executeAllPromptsButtonText;
    [ObservableProperty] private string? searchablePromptText;
    [ObservableProperty] private string? updatingText;

    public event EventHandler<string>? SearchablePromptTextChanged;

    partial void OnSearchablePromptTextChanged(string? value)
    {
        SearchablePromptTextChanged?.Invoke(this, value ?? string.Empty);
    }
}
