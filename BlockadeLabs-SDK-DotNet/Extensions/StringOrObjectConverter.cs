// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlockadeLabsSDK
{
    internal sealed class StringOrObjectConverter<T> : JsonConverter<dynamic>
    {
        public override dynamic Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.TokenType switch
            {
                JsonTokenType.Null => null,
                JsonTokenType.String => reader.GetString(),
                JsonTokenType.True => reader.GetBoolean(),
                JsonTokenType.False => reader.GetBoolean(),
                JsonTokenType.StartObject => JsonSerializer.Deserialize<T>(ref reader, options),
                _ => throw new JsonException($"Unexpected token type: {reader.TokenType}")
            };

        public override void Write(Utf8JsonWriter writer, dynamic value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case null:
                    writer.WriteNullValue();
                    break;
                case string stringValue:
                    writer.WriteStringValue(stringValue);
                    break;
                case bool:
                    writer.WriteBooleanValue(value);
                    break;
                case T @object:
                    JsonSerializer.Serialize(writer, @object, options);
                    break;
                default:
                    throw new JsonException($"Unexpected value type: {value.GetType()}");
            }
        }
    }
}
