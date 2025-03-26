using Statistics.Shared.Abstraction.Interfaces.Models.Entity;

namespace Statistics.Uno.Presentation.ViewModel;

public partial class ResponsesViewModel : ObservableObject
{
    [ObservableProperty]
    private IEnumerable<IResponse> responses;
}
