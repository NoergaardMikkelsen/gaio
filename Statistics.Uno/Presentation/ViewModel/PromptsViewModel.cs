using Statistics.Shared.Abstraction.Interfaces.Models.Entity;

public partial class PromptsViewModel : ObservableObject
{
    [ObservableProperty]
    private IEnumerable<IPrompt> prompts;
}
