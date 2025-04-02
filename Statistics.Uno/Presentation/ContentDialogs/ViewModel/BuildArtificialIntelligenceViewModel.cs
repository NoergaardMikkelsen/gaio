using CommunityToolkit.Mvvm.ComponentModel;
using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;

namespace Statistics.Uno.Presentation.ContentDialogs.ViewModel;

public partial class BuildArtificialIntelligenceViewModel : ObservableObject
{
    public BuildArtificialIntelligenceViewModel(IArtificialIntelligence ai)
    {
        Id = ai.Id;
        Name = ai.Name;
        Key = ai.Key;
        AiType = ai.AiType;
        Version = ai.Version;
        CreatedDateTime = ai.CreatedDateTime;
        UpdatedDateTime = ai.UpdatedDateTime;
    }

    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private string key;

    [ObservableProperty]
    private ArtificialIntelligenceType aiType;

    [ObservableProperty]
    private uint version;

    [ObservableProperty]
    private DateTime createdDateTime;

    [ObservableProperty]
    private DateTime updatedDateTime;
}
