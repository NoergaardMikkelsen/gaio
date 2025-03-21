using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces.Models.Searchable;

namespace Statistics.Shared.Models.Searchable;

public class SearchableArtificialIntelligence : ISearchableArtificialIntelligence
{
    /// <inheritdoc />
    public int Id { get; set; }

    /// <inheritdoc />
    public string Name { get; set; }

    /// <inheritdoc />
    public string Key { get; set; }

    /// <inheritdoc />
    public ArtificialIntelligenceType AiType { get; set; }
}
