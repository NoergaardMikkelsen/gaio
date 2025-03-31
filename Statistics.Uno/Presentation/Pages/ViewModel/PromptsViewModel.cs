using Statistics.Shared.Abstraction.Interfaces.Models.Entity;

namespace Statistics.Uno.Presentation.Pages.ViewModel;

public partial class PromptsViewModel : ObservableObject
{
    [ObservableProperty] private IEnumerable<IPrompt> prompts;
}
