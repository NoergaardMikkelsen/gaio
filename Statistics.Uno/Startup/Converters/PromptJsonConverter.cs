using System.Text.Json;
using System.Text.Json.Serialization;
using Statistics.Shared.Models.Entity;

namespace Statistics.Uno.Startup.Converters;

public class PromptJsonConverter : JsonConverter<Prompt>
{
    public override Prompt Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        int id = default;
        string text = null;
        uint version = default;
        DateTime createdDateTime = default;
        DateTime updatedDateTime = default;
        List<Response> responses = new List<Response>();

        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.EndObject:
                    return new Prompt(id, responses)
                    {
                        Text = text,
                        Version = version,
                        CreatedDateTime = createdDateTime,
                        UpdatedDateTime = updatedDateTime
                    };
                case JsonTokenType.PropertyName:
                    {
                        string propertyName = reader.GetString();
                        reader.Read();

                        switch (propertyName)
                        {
                            case "id":
                                id = reader.GetInt32();
                                break;
                            case "text":
                                text = reader.GetString();
                                break;
                            case "version":
                                version = reader.GetUInt32();
                                break;
                            case "createdDateTime":
                                createdDateTime = reader.GetDateTime();
                                break;
                            case "updatedDateTime":
                                updatedDateTime = reader.GetDateTime();
                                break;
                            case "responses":
                                responses = JsonSerializer.Deserialize<List<Response>>(ref reader, options);
                                break;
                        }

                        break;
                    }
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Prompt value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteNumber("id", value.Id);
        writer.WriteString("text", value.Text);
        writer.WriteNumber("version", value.Version);
        writer.WriteString("createdDateTime", value.CreatedDateTime);
        writer.WriteString("updatedDateTime", value.UpdatedDateTime);
        writer.WritePropertyName("responses");
        JsonSerializer.Serialize(writer, value.Responses, options);

        writer.WriteEndObject();
    }
}
