using Statistics.Shared.Abstraction.Interfaces.Models.Entity;

namespace Statistics.Uno.Presentation.Pages.ViewModel;

public partial class PromptsViewModel : ObservableObject
{
    public PromptsViewModel()
    {
        ExecuteAllPromptsButtonText = "Execute Prompts";
    }

    [ObservableProperty] private IEnumerable<IPrompt> prompts;
    
    [ObservableProperty] private string executeAllPromptsButtonText;
}
