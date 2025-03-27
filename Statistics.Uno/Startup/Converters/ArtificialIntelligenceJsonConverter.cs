using System.Text.Json;
using System.Text.Json.Serialization;
using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Extensions;
using Statistics.Shared.Models.Entity;

namespace Statistics.Uno.Startup.Converters;

public class ArtificialIntelligenceJsonConverter : JsonConverter<ArtificialIntelligence>
{
    private static readonly string Id = nameof(ArtificialIntelligence.Id).ToJsonPropertyCapitalisation();
    private static readonly string Name = nameof(ArtificialIntelligence.Name).ToJsonPropertyCapitalisation();
    private static readonly string Key = nameof(ArtificialIntelligence.Key).ToJsonPropertyCapitalisation();
    private static readonly string AiType = nameof(ArtificialIntelligence.AiType).ToJsonPropertyCapitalisation();
    private static readonly string Version = nameof(ArtificialIntelligence.Version).ToJsonPropertyCapitalisation();
    private static readonly string CreatedDateTime = nameof(ArtificialIntelligence.CreatedDateTime).ToJsonPropertyCapitalisation();
    private static readonly string UpdatedDateTime = nameof(ArtificialIntelligence.UpdatedDateTime).ToJsonPropertyCapitalisation();
    private static readonly string Responses = nameof(ArtificialIntelligence.Responses).ToJsonPropertyCapitalisation();

    public override ArtificialIntelligence Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        int id = default;
        string name = null;
        string key = null;
        ArtificialIntelligenceType aiType = default;
        uint version = default;
        DateTime createdDateTime = default;
        DateTime updatedDateTime = default;
        List<Response> responses = new List<Response>();

        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.EndObject:
                    return new ArtificialIntelligence(id, responses)
                    {
                        Name = name,
                        Key = key,
                        AiType = aiType,
                        Version = version,
                        CreatedDateTime = createdDateTime,
                        UpdatedDateTime = updatedDateTime
                    };
                case JsonTokenType.PropertyName:
                    {
                        string propertyName = reader.GetString();
                        reader.Read();

                        if (propertyName == Id)
                        {
                            id = reader.GetInt32();
                        }
                        else if (propertyName == Name)
                        {
                            name = reader.GetString();
                        }
                        else if (propertyName == Key)
                        {
                            key = reader.GetString();
                        }
                        else if (propertyName == AiType)
                        {
                            aiType = Enum.Parse<ArtificialIntelligenceType>(reader.GetString());
                        }
                        else if (propertyName == Version)
                        {
                            version = reader.GetUInt32();
                        }
                        else if (propertyName == CreatedDateTime)
                        {
                            createdDateTime = reader.GetDateTime();
                        }
                        else if (propertyName == UpdatedDateTime)
                        {
                            updatedDateTime = reader.GetDateTime();
                        }
                        else if (propertyName == Responses)
                        {
                            var responseListConverter = new ResponseListJsonConverter();
                            responses = responseListConverter.Read(ref reader, typeof(List<Response>), options);
                        }

                        break;
                    }
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, ArtificialIntelligence value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteNumber(Id, value.Id);
        writer.WriteString(Name, value.Name);
        writer.WriteString(Key, value.Key);
        writer.WriteString(AiType, value.AiType.ToString());
        writer.WriteNumber(Version, value.Version);
        writer.WriteString(CreatedDateTime, value.CreatedDateTime);
        writer.WriteString(UpdatedDateTime, value.UpdatedDateTime);
        writer.WritePropertyName(Responses);
        JsonSerializer.Serialize(writer, value.Responses, options);

        writer.WriteEndObject();
    }
}
