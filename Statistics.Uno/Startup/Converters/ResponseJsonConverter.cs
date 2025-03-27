using System.Text.Json;
using System.Text.Json.Serialization;
using Statistics.Shared.Extensions;
using Statistics.Shared.Models.Entity;

namespace Statistics.Uno.Startup.Converters;

public class ResponseJsonConverter : JsonConverter<Response>
{
    private static readonly string Id = nameof(Response.Id).ToJsonPropertyCapitalisation();
    private static readonly string Text = nameof(Response.Text).ToJsonPropertyCapitalisation();
    private static readonly string AiId = nameof(Response.AiId).ToJsonPropertyCapitalisation();
    private static readonly string PromptId = nameof(Response.PromptId).ToJsonPropertyCapitalisation();
    private static readonly string Version = nameof(Response.Version).ToJsonPropertyCapitalisation();
    private static readonly string CreatedDateTime = nameof(Response.CreatedDateTime).ToJsonPropertyCapitalisation();
    private static readonly string UpdatedDateTime = nameof(Response.UpdatedDateTime).ToJsonPropertyCapitalisation();
    private static readonly string Ai = nameof(Response.Ai).ToJsonPropertyCapitalisation();
    private static readonly string Prompt = nameof(Response.Prompt).ToJsonPropertyCapitalisation();

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

                        if (propertyName == Id)
                        {
                            id = reader.GetInt32();
                        }
                        else if (propertyName == Text)
                        {
                            text = reader.GetString();
                        }
                        else if (propertyName == AiId)
                        {
                            aiId = reader.GetInt32();
                        }
                        else if (propertyName == PromptId)
                        {
                            promptId = reader.GetInt32();
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
                        else if (propertyName == Ai)
                        {
                            ai = JsonSerializer.Deserialize<ArtificialIntelligence>(ref reader, options);
                        }
                        else if (propertyName == Prompt)
                        {
                            prompt = JsonSerializer.Deserialize<Prompt>(ref reader, options);
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

        writer.WriteNumber(Id, value.Id);
        writer.WriteString(Text, value.Text);
        writer.WriteNumber(AiId, value.AiId);
        writer.WriteNumber(PromptId, value.PromptId);
        writer.WriteNumber(Version, value.Version);
        writer.WriteString(CreatedDateTime, value.CreatedDateTime);
        writer.WriteString(UpdatedDateTime, value.UpdatedDateTime);
        writer.WritePropertyName(Ai);
        JsonSerializer.Serialize(writer, value.Ai, options);
        writer.WritePropertyName(Prompt);
        JsonSerializer.Serialize(writer, value.Prompt, options);

        writer.WriteEndObject();
    }
}
