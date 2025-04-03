using Statistics.Shared.Abstraction.Interfaces.Models.Entity;

namespace Statistics.Uno.Presentation.Pages.ViewModel;

public partial class ResponsesViewModel : ObservableObject
{
    public ResponsesViewModel()
    {
        ExecuteAllPromptsButtonText = "Execute Prompts";
    }

    [ObservableProperty] private IEnumerable<IResponse> responses;

    [ObservableProperty] private string executeAllPromptsButtonText;
}
