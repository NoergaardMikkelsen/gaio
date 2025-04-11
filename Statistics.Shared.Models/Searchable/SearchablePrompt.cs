using Statistics.Shared.Abstraction.Interfaces.Models.Searchable;

namespace Statistics.Shared.Models.Searchable;

public class SearchablePrompt : ISearchablePrompt
{
    public SearchablePrompt()
    {
        Text = string.Empty;
    }

    /// <inheritdoc />
    public string Text { get; set; }

    /// <inheritdoc />
    public int Id { get; set; }
}
