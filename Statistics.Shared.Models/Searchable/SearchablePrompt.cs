using Statistics.Shared.Abstraction.Interfaces.Models.Searchable;

namespace Statistics.Shared.Models.Searchable;

public class SearchablePrompt : ISearchablePrompt
{
    /// <inheritdoc />
    public string? Text { get; set; }

    /// <inheritdoc />
    public int Id { get; set; }
}
