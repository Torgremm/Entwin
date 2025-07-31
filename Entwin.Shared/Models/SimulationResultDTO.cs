using System.Text.Json.Serialization;
using System.Text.Json;

namespace Entwin.Shared.Models;

public class SimulationResultDTO
{
    [JsonConverter(typeof(SignalKeyDictionaryConverter<double>))]
    public Dictionary<SignalKey, double> Signals { get; set; } = new();

    public double Time { get; set; }
}

public record SignalKey(int FromId, int FromIndex, int ToId, int ToIndex);

public class SignalKeyDictionaryConverter<TValue> : JsonConverter<Dictionary<SignalKey, TValue>>
{
    public override Dictionary<SignalKey, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var result = new Dictionary<SignalKey, TValue>();

        using var doc = JsonDocument.ParseValue(ref reader);
        foreach (var element in doc.RootElement.EnumerateObject())
        {
            var parts = element.Name.Split('-').Select(int.Parse).ToArray();
            var key = new SignalKey(parts[0], parts[1], parts[2], parts[3]);
            var value = element.Value.Deserialize<TValue>(options)!;
            result[key] = value;
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<SignalKey, TValue> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var kvp in value)
        {
            var keyString = $"{kvp.Key.FromId}-{kvp.Key.FromIndex}-{kvp.Key.ToId}-{kvp.Key.ToIndex}";
            writer.WritePropertyName(keyString);
            JsonSerializer.Serialize(writer, kvp.Value, options);
        }
        writer.WriteEndObject();
    }
}