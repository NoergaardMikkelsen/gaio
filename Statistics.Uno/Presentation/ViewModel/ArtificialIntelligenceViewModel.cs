using System.Collections.ObjectModel;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;

namespace Statistics.Uno.Presentation.ViewModel;

public partial class ArtificialIntelligenceViewModel : ObservableObject
{
    [ObservableProperty]
    private IEnumerable<IArtificialIntelligence> artificialIntelligences;
}
