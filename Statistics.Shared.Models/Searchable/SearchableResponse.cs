using Statistics.Shared.Abstraction.Interfaces.Models.Searchable;

namespace Statistics.Shared.Models.Searchable;

public class SearchableResponse : ISearchableResponse
{
    /// <inheritdoc />
    public string Text { get; set; }

    /// <inheritdoc />
    public int AiId { get; set; }

    /// <inheritdoc />
    public int PromptId { get; set; }

    /// <inheritdoc />
    public int Id { get; set; }
}
