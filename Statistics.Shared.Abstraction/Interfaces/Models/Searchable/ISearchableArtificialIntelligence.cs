using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces.Persistence;

namespace Statistics.Shared.Abstraction.Interfaces.Models.Searchable;

public interface ISearchableArtificialIntelligence : ISearchable
{
    string Name { get; set; }
    string Key { get; set; }
    ArtificialIntelligenceType? AiType { get; set; }
}
