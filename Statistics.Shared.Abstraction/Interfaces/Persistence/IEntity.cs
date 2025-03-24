namespace Statistics.Shared.Abstraction.Interfaces.Persistence;

public interface IEntity : ISearchable
{
    uint Version { get; set; }
    DateTime CreatedDateTime { get; set; }
    DateTime UpdatedDateTime { get; set; }
}
