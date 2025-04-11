using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Statistics.Shared.Extensions;
using Statistics.Shared.Models.Entity;

namespace Statistics.Shared.Core.Newtonsoft.JsonConverters;

public class KeywordJsonConverter : JsonConverter<Keyword>
{
    public override bool CanWrite => false;

    public override Keyword ReadJson(
        JsonReader reader, Type objectType, Keyword existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType != JsonToken.StartObject)
        {
            throw new JsonException();
        }

        JObject? jObject = JObject.Load(reader);
        int id = jObject[nameof(Keyword.Id).ToJsonPropertyCapitalisation()]?.Value<int>() ?? default;
        var text = jObject[nameof(Keyword.Text).ToJsonPropertyCapitalisation()]?.Value<string>();
        uint version = jObject[nameof(Keyword.Version).ToJsonPropertyCapitalisation()]?.Value<uint>() ?? default;
        DateTime createdDateTime =
            jObject[nameof(Keyword.CreatedDateTime).ToJsonPropertyCapitalisation()]?.Value<DateTime>() ?? default;
        DateTime updatedDateTime =
            jObject[nameof(Keyword.UpdatedDateTime).ToJsonPropertyCapitalisation()]?.Value<DateTime>() ?? default;
        bool useRegex = jObject[nameof(Keyword.UseRegex).ToJsonPropertyCapitalisation()]?.Value<bool>() ?? default;
        var startSearch = jObject[nameof(Keyword.StartSearch).ToJsonPropertyCapitalisation()]?.Value<DateTime?>();
        var endSearch = jObject[nameof(Keyword.EndSearch).ToJsonPropertyCapitalisation()]?.Value<DateTime?>();

        return new Keyword(id)
        {
            Text = text,
            Version = version,
            CreatedDateTime = createdDateTime,
            UpdatedDateTime = updatedDateTime,
            UseRegex = useRegex,
            StartSearch = startSearch,
            EndSearch = endSearch,
        };
    }

    public override void WriteJson(JsonWriter writer, Keyword value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
