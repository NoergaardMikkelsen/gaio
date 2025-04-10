namespace Statistics.Shared.Abstraction.Interfaces.Models.Searchable;

public interface IComplexSearchable
{
    public ISearchableArtificialIntelligence? SearchableArtificialIntelligence { get; set; }
    public ISearchablePrompt? SearchablePrompt { get; set; }
    public ISearchableKeyword? SearchableKeyword { get; set; }
    public ISearchableResponse? SearchableResponse { get; set; }
}
