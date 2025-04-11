using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Statistics.Shared.Extensions;
using Statistics.Shared.Models.Searchable;

namespace Statistics.Shared.Core.Newtonsoft.JsonConverters;

public class ComplexSearchableJsonConverter : JsonConverter<ComplexSearchable>
{
    public override bool CanWrite => false;

    public override ComplexSearchable ReadJson(
        JsonReader reader, Type objectType, ComplexSearchable existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType != JsonToken.StartObject)
        {
            throw new JsonException();
        }

        JObject jObject = JObject.Load(reader);
        var searchableArtificialIntelligence =
            jObject[nameof(ComplexSearchable.SearchableArtificialIntelligence).ToJsonPropertyCapitalisation()]
                ?.ToObject<SearchableArtificialIntelligence>(serializer);
        var searchablePrompt = jObject[nameof(ComplexSearchable.SearchablePrompt).ToJsonPropertyCapitalisation()]
            ?.ToObject<SearchablePrompt>(serializer);
        var searchableKeyword = jObject[nameof(ComplexSearchable.SearchableKeyword).ToJsonPropertyCapitalisation()]
            ?.ToObject<SearchableKeyword>(serializer);
        var searchableResponse = jObject[nameof(ComplexSearchable.SearchableResponse).ToJsonPropertyCapitalisation()]
            ?.ToObject<SearchableResponse>(serializer);

        return new ComplexSearchable
        {
            SearchableArtificialIntelligence = searchableArtificialIntelligence,
            SearchablePrompt = searchablePrompt,
            SearchableKeyword = searchableKeyword,
            SearchableResponse = searchableResponse,
        };
    }

    public override void WriteJson(JsonWriter writer, ComplexSearchable value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
