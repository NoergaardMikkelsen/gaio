using Statistics.Shared.Abstraction.Interfaces.Persistence;

namespace Statistics.Shared.Abstraction.Interfaces.Models.Searchable;

public interface ISearchableResponse : ISearchable
{
    string Text { get; set; }
    int AiId { get; set; }
    int PromptId { get; set; }
}
