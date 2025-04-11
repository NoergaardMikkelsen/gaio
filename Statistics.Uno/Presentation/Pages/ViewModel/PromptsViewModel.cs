using System.Collections.ObjectModel;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;

namespace Statistics.Uno.Presentation.Pages.ViewModel;

public partial class PromptsViewModel : ObservableObject
{
    [ObservableProperty] private string executeAllPromptsButtonText;

    [ObservableProperty] private ObservableCollection<IPrompt> prompts;
    [ObservableProperty] private string? searchablePromptText;
    [ObservableProperty] private string? updatingText;

    public PromptsViewModel()
    {
        ExecuteAllPromptsButtonText = "Execute Prompts";
    }

    public event EventHandler<string>? SearchablePromptTextChanged;

    partial void OnSearchablePromptTextChanged(string? value)
    {
        SearchablePromptTextChanged?.Invoke(this, value ?? string.Empty);
    }
}
