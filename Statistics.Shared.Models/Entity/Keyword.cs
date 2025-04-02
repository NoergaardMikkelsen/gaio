using Statistics.Shared.Abstraction.Interfaces.Models.Entity;

namespace Statistics.Shared.Models.Entity;

public class Keyword : IKeyword
{
    /// <inheritdoc />
    public int Id { get; set; }

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
