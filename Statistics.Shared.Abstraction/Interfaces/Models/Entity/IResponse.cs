using Statistics.Shared.Abstraction.Interfaces.Models.Searchable;
using Statistics.Shared.Abstraction.Interfaces.Persistence;

namespace Statistics.Shared.Abstraction.Interfaces.Models.Entity;

public interface IResponse : ISearchableResponse, IEntity
{
    IArtificialIntelligence Ai { get; set; }
    IPrompt Prompt { get; set; }
}
