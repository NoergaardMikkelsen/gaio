using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Abstraction.Interfaces.Models.Entity;
using Statistics.Shared.Extensions;
using Statistics.Shared.Models.Entity;
using JsonException = Newtonsoft.Json.JsonException;

namespace Statistics.Shared.Core.Newtonsoft.JsonConverters;

public class ArtificialIntelligenceJsonConverter : JsonConverter<ArtificialIntelligence>
{
    public override ArtificialIntelligence ReadJson(JsonReader reader, Type objectType, ArtificialIntelligence existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType != JsonToken.StartObject)
        {
            throw new JsonException();
        }

        var jObject = JObject.Load(reader);
        var id = jObject[nameof(ArtificialIntelligence.Id).ToJsonPropertyCapitalisation()]?.Value<int>() ?? default;
        var name = jObject[nameof(ArtificialIntelligence.Name).ToJsonPropertyCapitalisation()]?.Value<string>();
        var key = jObject[nameof(ArtificialIntelligence.Key).ToJsonPropertyCapitalisation()]?.Value<string>();
        var aiType = jObject[nameof(ArtificialIntelligence.AiType).ToJsonPropertyCapitalisation()]?.Value<string>();
        var version = jObject[nameof(ArtificialIntelligence.Version).ToJsonPropertyCapitalisation()]?.Value<uint>() ?? default;
        var createdDateTime = jObject[nameof(ArtificialIntelligence.CreatedDateTime).ToJsonPropertyCapitalisation()]?.Value<DateTime>() ?? default;
        var updatedDateTime = jObject[nameof(ArtificialIntelligence.UpdatedDateTime).ToJsonPropertyCapitalisation()]?.Value<DateTime>() ?? default;
        var responses = jObject[nameof(ArtificialIntelligence.Responses).ToJsonPropertyCapitalisation()]?.ToObject<List<Response>>(serializer) ?? new List<Response>();

        return new ArtificialIntelligence(id, responses)
        {
            Name = name,
            Key = key,
            AiType = Enum.Parse<ArtificialIntelligenceType>(aiType),
            Version = version,
            CreatedDateTime = createdDateTime,
            UpdatedDateTime = updatedDateTime
        };
    }

    public override void WriteJson(JsonWriter writer, ArtificialIntelligence value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanWrite
    {
        get { return false; }
    }
}
