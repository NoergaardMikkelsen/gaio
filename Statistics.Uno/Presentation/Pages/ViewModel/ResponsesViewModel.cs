using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Statistics.Uno.Presentation.Pages.ViewModel;

public partial class ResponsesViewModel : ObservableObject
{
    public ResponsesViewModel()
    {
        ExecuteAllPromptsButtonText = "Execute Prompts";
    }

    [ObservableProperty] private ObservableCollection<IResponse> responses;
    [ObservableProperty] private string executeAllPromptsButtonText;
    [ObservableProperty] private string? searchablePromptText;
    [ObservableProperty] private string? searchableResponseText;
    [ObservableProperty] private ArtificialIntelligenceType selectedAiType;
    [ObservableProperty] private string? updatingText;

    public event EventHandler<string>? SearchablePromptTextChanged;
    public event EventHandler<string>? SearchableResponseTextChanged;

    partial void OnSearchableResponseTextChanged(string? value)
    {
        SearchableResponseTextChanged?.Invoke(this, value ?? string.Empty);
    }

    partial void OnSearchablePromptTextChanged(string? value)
    {
        SearchablePromptTextChanged?.Invoke(this, value ?? string.Empty);
    }

}
