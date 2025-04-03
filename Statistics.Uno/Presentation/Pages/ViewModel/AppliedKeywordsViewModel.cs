using Statistics.Shared.Abstraction.Interfaces.Models;

namespace Statistics.Uno.Presentation.Pages.ViewModel
{
    public partial class AppliedKeywordsViewModel : ObservableObject
    {
        [ObservableProperty]
        private List<IAppliedKeyword> appliedKeywords;
    }
}
