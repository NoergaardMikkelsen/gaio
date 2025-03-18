using Statistics.Shared.Abstraction.Interfaces.Models.Searchable;

namespace Statistics.Shared.Abstraction.Interfaces.Models.Entity;

public interface IResponse : ISearchableResponse
{
    IArtificialIntelligence Ai { get; set; }
    IPrompt Prompt { get; set; }
}
