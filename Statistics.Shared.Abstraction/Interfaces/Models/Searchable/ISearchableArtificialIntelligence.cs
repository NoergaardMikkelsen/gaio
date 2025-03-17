using Statistics.Shared.Abstraction.Interfaces.Persistence;

namespace Statistics.Shared.Abstraction.Interfaces.Model.Searchable;

public interface ISearchableArtificialIntelligence : ISearchable
{
    string Name { get; set; }
    string Key { get; set; }
}
