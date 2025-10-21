using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Espectaculos.WebApi.JsonConverters;

/// <summary>
/// JsonConverterFactory that creates converters for enum types (including Nullable&lt;T&gt;) that accept
/// string enum names case-insensitively and numeric values.
/// </summary>
public sealed class CaseInsensitiveEnumConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        var t = Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;
        return t.IsEnum;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var underlyingType = Nullable.GetUnderlyingType(typeToConvert);
        if (underlyingType is not null)
        {
            var converterType = typeof(CaseInsensitiveNullableEnumConverter<>).MakeGenericType(underlyingType);
            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }

        var convType = typeof(CaseInsensitiveEnumConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter)Activator.CreateInstance(convType)!;
    }

    private sealed class CaseInsensitiveEnumConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var s = reader.GetString();
                if (string.IsNullOrEmpty(s)) throw new JsonException("Empty enum value.");
                if (Enum.TryParse<T>(s, ignoreCase: true, out var result)) return result;
                throw new JsonException($"Unknown value '{s}' for enum {typeof(T)}.");
            }
            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt32(out var i))
                {
                    return (T)Enum.ToObject(typeof(T), i);
                }
                throw new JsonException("Invalid numeric value for enum.");
            }
            throw new JsonException("Unexpected token when parsing enum.");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString());
    }

    private sealed class CaseInsensitiveNullableEnumConverter<T> : JsonConverter<T?> where T : struct, Enum
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            if (reader.TokenType == JsonTokenType.String)
            {
                var s = reader.GetString();
                if (string.IsNullOrEmpty(s)) return null;
                if (Enum.TryParse<T>(s, ignoreCase: true, out var result)) return result;
                throw new JsonException($"Unknown value '{s}' for enum {typeof(T)}.");
            }
            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt32(out var i))
                {
                    return (T)Enum.ToObject(typeof(T), i);
                }
                throw new JsonException("Invalid numeric value for enum.");
            }
            throw new JsonException("Unexpected token when parsing enum.");
        }

        public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
        {
            if (value.HasValue) writer.WriteStringValue(value.Value.ToString());
            else writer.WriteNullValue();
        }
    }
}
