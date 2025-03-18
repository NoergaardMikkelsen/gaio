namespace Statistics.Shared.Abstraction.Interfaces.Models.Searchable;

public interface ISearchableResponse
{
    string Text { get; set; }
    int AiId { get; set; }
    int PromptId { get; set; }
}
