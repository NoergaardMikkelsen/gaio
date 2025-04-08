using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using System.Collections.Generic;

namespace Statistics.Uno.Presentation.Pages.ViewModel;

public partial class ArtificialIntelligenceViewModel : ObservableObject
{
    [ObservableProperty] private IEnumerable<IArtificialIntelligence> artificialIntelligences;
    [ObservableProperty] private string? searchableAiName;
    [ObservableProperty] private string? searchableAiKey;
    [ObservableProperty] private string? updatingText;

    public event EventHandler<string>? SearchableAiNameChanged;
    public event EventHandler<string>? SearchableAiKeyChanged;

    partial void OnSearchableAiNameChanged(string? value)
    {
        SearchableAiNameChanged?.Invoke(this, value ?? string.Empty);
    }

    partial void OnSearchableAiKeyChanged(string? value)
    {
        SearchableAiKeyChanged?.Invoke(this, value ?? string.Empty);
    }
}
