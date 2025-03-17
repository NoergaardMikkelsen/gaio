using Statistics.Shared.Abstraction.Interfaces.Model.Searchable;

namespace Statistics.Shared.Abstraction.Interfaces.Model.Entity;

public interface IResponse : ISearchableResponse
{
    IArtificialIntelligence Ai { get; set; }
    IPrompt Prompt { get; set; }
}
