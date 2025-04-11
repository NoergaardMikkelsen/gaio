using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Statistics.Shared.Extensions;
using Statistics.Shared.Models.Entity;

namespace Statistics.Shared.Core.Newtonsoft.JsonConverters;

public class ResponseJsonConverter : JsonConverter<Response>
{
    public override bool CanWrite => false;

    public override Response ReadJson(
        JsonReader reader, Type objectType, Response existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType != JsonToken.StartObject)
        {
            throw new JsonException();
        }

        JObject? jObject = JObject.Load(reader);
        int id = jObject[nameof(Response.Id).ToJsonPropertyCapitalisation()]?.Value<int>() ?? default;
        var text = jObject[nameof(Response.Text).ToJsonPropertyCapitalisation()]?.Value<string>();
        int aiId = jObject[nameof(Response.AiId).ToJsonPropertyCapitalisation()]?.Value<int>() ?? default;
        int promptId = jObject[nameof(Response.PromptId).ToJsonPropertyCapitalisation()]?.Value<int>() ?? default;
        uint version = jObject[nameof(Response.Version).ToJsonPropertyCapitalisation()]?.Value<uint>() ?? default;
        DateTime createdDateTime =
            jObject[nameof(Response.CreatedDateTime).ToJsonPropertyCapitalisation()]?.Value<DateTime>() ?? default;
        DateTime updatedDateTime =
            jObject[nameof(Response.UpdatedDateTime).ToJsonPropertyCapitalisation()]?.Value<DateTime>() ?? default;
        var ai = jObject[nameof(Response.Ai).ToJsonPropertyCapitalisation()]
            ?.ToObject<ArtificialIntelligence>(serializer);
        var prompt = jObject[nameof(Response.Prompt).ToJsonPropertyCapitalisation()]?.ToObject<Prompt>(serializer);

        return new Response(id, prompt, ai)
        {
            Text = text,
            AiId = aiId,
            PromptId = promptId,
            Version = version,
            CreatedDateTime = createdDateTime,
            UpdatedDateTime = updatedDateTime,
        };
    }

    public override void WriteJson(JsonWriter writer, Response value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
