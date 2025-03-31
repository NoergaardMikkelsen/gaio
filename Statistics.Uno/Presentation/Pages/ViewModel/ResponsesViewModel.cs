using Statistics.Shared.Abstraction.Interfaces.Models.Entity;

namespace Statistics.Uno.Presentation.Pages.ViewModel;

public partial class ResponsesViewModel : ObservableObject
{
    [ObservableProperty] private IEnumerable<IResponse> responses;
}
