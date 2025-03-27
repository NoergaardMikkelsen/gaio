using System.Text.Json;
using System.Text.Json.Serialization;
using Statistics.Shared.Models.Entity;

namespace Statistics.Uno.Startup.Converters;

public class ResponseJsonConverter : JsonConverter<Response>
{
    public override Response Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        int id = default;
        string text = null;
        int aiId = default;
        int promptId = default;
        uint version = default;
        DateTime createdDateTime = default;
        DateTime updatedDateTime = default;
        ArtificialIntelligence ai = null;
        Prompt prompt = null;

        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.EndObject:
                    return new Response(id, prompt, ai)
                    {
                        Text = text,
                        AiId = aiId,
                        PromptId = promptId,
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
                            case "aiId":
                                aiId = reader.GetInt32();
                                break;
                            case "promptId":
                                promptId = reader.GetInt32();
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
                            case "ai":
                                ai = JsonSerializer.Deserialize<ArtificialIntelligence>(ref reader, options);
                                break;
                            case "prompt":
                                prompt = JsonSerializer.Deserialize<Prompt>(ref reader, options);
                                break;
                        }

                        break;
                    }
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Response value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteNumber("id", value.Id);
        writer.WriteString("text", value.Text);
        writer.WriteNumber("aiId", value.AiId);
        writer.WriteNumber("promptId", value.PromptId);
        writer.WriteNumber("version", value.Version);
        writer.WriteString("createdDateTime", value.CreatedDateTime);
        writer.WriteString("updatedDateTime", value.UpdatedDateTime);
        writer.WritePropertyName("ai");
        JsonSerializer.Serialize(writer, value.Ai, options);
        writer.WritePropertyName("prompt");
        JsonSerializer.Serialize(writer, value.Prompt, options);

        writer.WriteEndObject();
    }
}
