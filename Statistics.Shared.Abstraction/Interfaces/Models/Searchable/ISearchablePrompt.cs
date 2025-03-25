using Statistics.Shared.Abstraction.Interfaces.Persistence;

namespace Statistics.Shared.Abstraction.Interfaces.Models.Searchable;

public interface ISearchablePrompt : ISearchable
{
    string? Text { get; set; }
}
