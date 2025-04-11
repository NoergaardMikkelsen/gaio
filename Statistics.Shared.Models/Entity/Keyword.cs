using System.Text.Json.Serialization;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;

namespace Statistics.Shared.Models.Entity;

public class Keyword : IKeyword
{
    private readonly int id;

    public Keyword()
    {
    }

    /// <summary>
    ///     Constructor for Entity Framework Core and Tests to use.
    /// </summary>
    /// <param name="id"></param>
    [JsonConstructor]
    public Keyword(int id)
    {
        this.id = id;
    }

    /// <inheritdoc />
    public int Id
    {
        get => id;
        set => throw new InvalidOperationException(
            $"{nameof(Id)} cannot be changed after creation of {nameof(ArtificialIntelligence)} entity");
    }

    /// <inheritdoc />
    public string Text { get; set; }

    /// <inheritdoc />
    public uint Version { get; set; }

    /// <inheritdoc />
    public DateTime CreatedDateTime { get; set; }

    /// <inheritdoc />
    public DateTime UpdatedDateTime { get; set; }

    /// <inheritdoc />
    public bool UseRegex { get; set; }

    /// <inheritdoc />
    public DateTime? StartSearch { get; set; }

    /// <inheritdoc />
    public DateTime? EndSearch { get; set; }
}
