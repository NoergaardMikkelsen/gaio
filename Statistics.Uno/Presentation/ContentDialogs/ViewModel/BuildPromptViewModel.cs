using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Models.Entity;

namespace Statistics.Uno.Presentation.ContentDialogs.ViewModel;

public partial class BuildPromptViewModel : ObservableObject
{
    public BuildPromptViewModel(IPrompt prompt)
    {
        _id = prompt.Id;
        _text = prompt.Text;
        _version = prompt.Version;
        _createdDateTime = prompt.CreatedDateTime;
        _updatedDateTime = prompt.UpdatedDateTime;
    }

    [ObservableProperty] private int _id;

    [ObservableProperty] private string _text;

    [ObservableProperty] private uint _version;

    [ObservableProperty] private DateTime _createdDateTime;

    [ObservableProperty] private DateTime _updatedDateTime;
}
