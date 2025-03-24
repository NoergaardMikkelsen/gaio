using Statistics.Shared.Abstraction.Interfaces.Models.Entity;

namespace Statistics.Shared.Models.Entity;

public class Prompt : IPrompt
{
    private int id;

    /// <inheritdoc />
    public int Id
    {
        get => id;
        set => throw new InvalidOperationException(
            $"{nameof(Id)} cannot be changed after creation of {nameof(Prompt)} entity");
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
    public ICollection<IResponse> Responses { get; set; }

    public Prompt()
    {
    }

    /// <summary>
    /// Constructor for Entity Framework Core to use.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="responses"></param>
    private Prompt(int id, List<Response> responses)
    {
        this.id = id;
        Responses = responses.Cast<IResponse>() as ICollection<IResponse> ?? new List<IResponse>();
    }
}
