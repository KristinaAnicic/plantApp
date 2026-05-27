using System.Text.Json;
using System.Text.Json.Serialization;

namespace PlantApp.Domain.Utils;

public class EmptyStringToNullConverter : JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? value = reader.GetString();
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}
