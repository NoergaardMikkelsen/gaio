using System.Text.Json;
using System.Text.Json.Serialization;
using Statistics.Shared.Extensions;
using Statistics.Shared.Models.Entity;

namespace Statistics.Uno.Startup.Converters;

public class PromptJsonConverter : JsonConverter<Prompt>
{
    private static readonly string Id = nameof(Prompt.Id).ToJsonPropertyCapitalisation();
    private static readonly string Text = nameof(Prompt.Text).ToJsonPropertyCapitalisation();
    private static readonly string Version = nameof(Prompt.Version).ToJsonPropertyCapitalisation();
    private static readonly string CreatedDateTime = nameof(Prompt.CreatedDateTime).ToJsonPropertyCapitalisation();
    private static readonly string UpdatedDateTime = nameof(Prompt.UpdatedDateTime).ToJsonPropertyCapitalisation();
    private static readonly string Responses = nameof(Prompt.Responses).ToJsonPropertyCapitalisation();

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

                        if (propertyName == Id)
                        {
                            id = reader.GetInt32();
                        }
                        else if (propertyName == Text)
                        {
                            text = reader.GetString();
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
                            responses = JsonSerializer.Deserialize<List<Response>>(ref reader, options);
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

        writer.WriteNumber(Id, value.Id);
        writer.WriteString(Text, value.Text);
        writer.WriteNumber(Version, value.Version);
        writer.WriteString(CreatedDateTime, value.CreatedDateTime);
        writer.WriteString(UpdatedDateTime, value.UpdatedDateTime);
        writer.WritePropertyName(Responses);
        JsonSerializer.Serialize(writer, value.Responses, options);

        writer.WriteEndObject();
    }
}
