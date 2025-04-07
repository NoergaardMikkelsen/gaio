using Statistics.Shared.Abstraction.Interfaces.Models.Searchable;

namespace Statistics.Shared.Models.Searchable;

public class SearchableKeyword : ISearchableKeyword
{
    /// <inheritdoc />
    public int Id { get; set; }

    /// <inheritdoc />
    public string Text { get; set; }

    public SearchableKeyword()
    {
        Text = string.Empty;
    }
}
