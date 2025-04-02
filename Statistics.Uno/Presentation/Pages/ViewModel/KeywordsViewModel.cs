using CommunityToolkit.Mvvm.ComponentModel;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;

namespace Statistics.Uno.Presentation.Pages.ViewModel;

public partial class KeywordsViewModel : ObservableObject
{
    [ObservableProperty]
    private IEnumerable<IKeyword> keywords;
}
