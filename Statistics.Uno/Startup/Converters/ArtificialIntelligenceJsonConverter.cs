using System.Text.Json;
using System.Text.Json.Serialization;
using Statistics.Shared.Abstraction.Enum;
using Statistics.Shared.Models.Entity;

namespace Statistics.Uno.Startup.Converters;

public class ArtificialIntelligenceJsonConverter : JsonConverter<ArtificialIntelligence>
{
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
        Console.WriteLine($"Starting reading of {nameof(ArtificialIntelligence)} Json");

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
                        Console.WriteLine($"Reading property: {propertyName}");
                        reader.Read();

                        switch (propertyName)
                        {
                            case "id":
                                id = reader.GetInt32();
                                Console.WriteLine($"Id: {id}");
                                break;
                            case "name":
                                name = reader.GetString();
                                Console.WriteLine($"Name: {name}");
                                break;
                            case "key":
                                key = reader.GetString();
                                Console.WriteLine($"Key: {key}");
                                break;
                            case "aiType":
                                aiType = Enum.Parse<ArtificialIntelligenceType>(reader.GetString());
                                Console.WriteLine($"AiType: {aiType}");
                                break;
                            case "version":
                                version = reader.GetUInt32();
                                Console.WriteLine($"Version: {version}");
                                break;
                            case "createdDateTime":
                                createdDateTime = reader.GetDateTime();
                                Console.WriteLine($"CreatedDateTime: {createdDateTime}");
                                break;
                            case "updatedDateTime":
                                updatedDateTime = reader.GetDateTime();
                                Console.WriteLine($"UpdatedDateTime: {updatedDateTime}");
                                break;
                            case "responses":
                                var responseListConverter = new ResponseListJsonConverter();
                                responses = responseListConverter.Read(ref reader, typeof(List<Response>), options);
                                Console.WriteLine($"Responses: {responses.Count} items");
                                break;
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

        writer.WriteNumber("id", value.Id);
        writer.WriteString("name", value.Name);
        writer.WriteString("key", value.Key);
        writer.WriteString("aiType", value.AiType.ToString());
        writer.WriteNumber("version", value.Version);
        writer.WriteString("createdDateTime", value.CreatedDateTime);
        writer.WriteString("updatedDateTime", value.UpdatedDateTime);
        writer.WritePropertyName("responses");
        JsonSerializer.Serialize(writer, value.Responses, options);

        writer.WriteEndObject();
    }
}
