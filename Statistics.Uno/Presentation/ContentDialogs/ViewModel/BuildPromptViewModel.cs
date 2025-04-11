using Statistics.Shared.Abstraction.Interfaces.Models.Entity;

namespace Statistics.Uno.Presentation.ContentDialogs.ViewModel;

public partial class BuildPromptViewModel : ObservableObject
{
    [ObservableProperty] private DateTime _createdDateTime;

    [ObservableProperty] private int _id;

    [ObservableProperty] private string _text;

    [ObservableProperty] private DateTime _updatedDateTime;

    [ObservableProperty] private uint _version;

    public BuildPromptViewModel(IPrompt prompt)
    {
        _id = prompt.Id;
        _text = prompt.Text;
        _version = prompt.Version;
        _createdDateTime = prompt.CreatedDateTime;
        _updatedDateTime = prompt.UpdatedDateTime;
    }
}
