using Statistics.Shared.Abstraction.Interfaces.Models.Searchable;
using Statistics.Shared.Abstraction.Interfaces.Persistence;

namespace Statistics.Shared.Abstraction.Interfaces.Models.Entity;

public interface IArtificialIntelligence : ISearchableArtificialIntelligence, IEntity
{
    ICollection<IResponse> Responses { get; set; }
}
