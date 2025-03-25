using System.Text.Json.Serialization;
using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;

namespace Statistics.Shared.Models.Entity;

public class ArtificialIntelligence : IArtificialIntelligence
{
    private int id;

    /// <inheritdoc />
    public int Id
    {
        get => id;
        set => throw new InvalidOperationException(
            $"{nameof(Id)} cannot be changed after creation of {nameof(ArtificialIntelligence)} entity");
    }

    /// <inheritdoc />
    public string Name { get; set; }

    /// <inheritdoc />
    public string Key { get; set; }

    /// <inheritdoc />
    public ArtificialIntelligenceType AiType { get; set; }

    /// <inheritdoc />
    public uint Version { get; set; }

    /// <inheritdoc />
    public DateTime CreatedDateTime { get; set; }

    /// <inheritdoc />
    public DateTime UpdatedDateTime { get; set; }

    /// <inheritdoc />
    public ICollection<IResponse> Responses { get; set; }

    public ArtificialIntelligence()
    {
    }

    /// <summary>
    /// Constructor for Entity Framework Core to use.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="Responses"></param>
    [JsonConstructor]
    private ArtificialIntelligence(int id, List<Response> Responses)
    {
        this.id = id;
        this.Responses = Responses.Cast<IResponse>() as ICollection<IResponse> ?? new List<IResponse>();
    }
}
