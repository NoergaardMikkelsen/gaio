using Statistics.Shared.Abstraction.Interfaces.Models.Searchable;

namespace Statistics.Shared.Models.Searchable;

public class ComplexSearchable : IComplexSearchable
{
    /// <inheritdoc />
    public ISearchableArtificialIntelligence? SearchableArtificialIntelligence { get; set; }

    /// <inheritdoc />
    public ISearchablePrompt? SearchablePrompt { get; set; }

    /// <inheritdoc />
    public ISearchableKeyword? SearchableKeyword { get; set; }

    /// <inheritdoc />
    public ISearchableResponse? SearchableResponse { get; set; }
}
