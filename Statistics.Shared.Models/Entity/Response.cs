using System.Text.Json.Serialization;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;

namespace Statistics.Shared.Models.Entity;

public class Response : IResponse
{
    private readonly int id;

    public Response()
    {
    }

    /// <summary>
    ///     Constructor for Entity Framework Core to use.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="prompt"></param>
    /// <param name="ai"></param>
    [JsonConstructor]
    public Response(int id, Prompt prompt, ArtificialIntelligence ai)
    {
        this.id = id;
        Prompt = prompt;
        Ai = ai;
    }

    /// <summary>
    ///     Constructor for Tests to use.
    /// </summary>
    /// <param name="id"></param>
    public Response(int id)
    {
        this.id = id;
    }

    /// <inheritdoc />
    public int Id
    {
        get => id;
        set => throw new InvalidOperationException(
            $"{nameof(Id)} cannot be changed after creation of {nameof(Response)} entity");
    }

    /// <inheritdoc />
    public string Text { get; set; }

    /// <inheritdoc />
    public int AiId { get; set; }

    /// <inheritdoc />
    public int PromptId { get; set; }

    /// <inheritdoc />
    public uint Version { get; set; }

    /// <inheritdoc />
    public DateTime CreatedDateTime { get; set; }

    /// <inheritdoc />
    public DateTime UpdatedDateTime { get; set; }

    /// <inheritdoc />
    public IArtificialIntelligence Ai { get; set; }

    /// <inheritdoc />
    public IPrompt Prompt { get; set; }
}
