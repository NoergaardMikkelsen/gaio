using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Statistics.Shared.Extensions;
using Statistics.Shared.Models.Entity;

namespace Statistics.Shared.Core.Newtonsoft.JsonConverters;

public class PromptJsonConverter : JsonConverter<Prompt>
{
    public override bool CanWrite => false;

    public override Prompt ReadJson(
        JsonReader reader, Type objectType, Prompt existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType != JsonToken.StartObject)
        {
            throw new JsonException();
        }

        JObject? jObject = JObject.Load(reader);
        int id = jObject[nameof(Prompt.Id).ToJsonPropertyCapitalisation()]?.Value<int>() ?? default;
        var text = jObject[nameof(Prompt.Text).ToJsonPropertyCapitalisation()]?.Value<string>();
        uint version = jObject[nameof(Prompt.Version).ToJsonPropertyCapitalisation()]?.Value<uint>() ?? default;
        DateTime createdDateTime =
            jObject[nameof(Prompt.CreatedDateTime).ToJsonPropertyCapitalisation()]?.Value<DateTime>() ?? default;
        DateTime updatedDateTime =
            jObject[nameof(Prompt.UpdatedDateTime).ToJsonPropertyCapitalisation()]?.Value<DateTime>() ?? default;
        var responses =
            jObject[nameof(Prompt.Responses).ToJsonPropertyCapitalisation()]?.ToObject<List<Response>>(serializer) ??
            new List<Response>();

        return new Prompt(id, responses)
        {
            Text = text,
            Version = version,
            CreatedDateTime = createdDateTime,
            UpdatedDateTime = updatedDateTime,
        };
    }

    public override void WriteJson(JsonWriter writer, Prompt value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
