namespace Statistics.Shared.Abstraction.Interfaces.Model.Searchable;

public interface ISearchableResponse
{
    string Text { get; set; }
    int AiId { get; set; }
    int PromptId { get; set; }
}
