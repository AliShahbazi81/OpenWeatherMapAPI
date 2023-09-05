using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenWeatherMap.Services
{
    public class IntFromStringJsonConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                {
                    var stringValue = reader.GetString();
                    if (int.TryParse(stringValue, out int intValue))
                    {
                        return intValue;
                    }

                    break;
                }
                case JsonTokenType.Number:
                    return reader.GetInt32();
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}