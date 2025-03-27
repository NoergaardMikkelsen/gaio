using System.Text.Json;
using System.Text.Json.Serialization;
using Statistics.Shared.Models.Entity;
using System.Collections.Generic;

namespace Statistics.Uno.Startup.Converters;

public class ResponseListJsonConverter : JsonConverter<List<Response>>
{
    public override List<Response> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException();
        }

        var responseConverter = (JsonConverter<Response>)options.GetConverter(typeof(Response));
        var responses = new List<Response>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                return responses;
            }

            var response = responseConverter.Read(ref reader, typeof(Response), options);
            responses.Add(response);
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, List<Response> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        var responseConverter = (JsonConverter<Response>)options.GetConverter(typeof(Response));

        foreach (var response in value)
        {
            responseConverter.Write(writer, response, options);
        }

        writer.WriteEndArray();
    }
}
