using System.Collections.ObjectModel;
using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces.Models;

namespace Statistics.Uno.Presentation.Pages.ViewModel;

public partial class AppliedKeywordsViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<IAppliedKeyword> appliedKeywords;

    [ObservableProperty] private ArtificialIntelligenceType selectedAiType;
}
