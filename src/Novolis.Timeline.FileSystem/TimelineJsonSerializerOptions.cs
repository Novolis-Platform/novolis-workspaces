using System.Text.Json;
using System.Text.Json.Serialization;

namespace Novolis.Timeline.FileSystem;

/// <summary>JSON options for persisting timeline graphs.</summary>
public static class TimelineJsonSerializerOptions
{
    public static JsonSerializerOptions Create() =>
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new BranchIdJsonConverter(),
                new TimelineNodeIdJsonConverter(),
            },
        };

    private sealed class BranchIdJsonConverter : JsonConverter<BranchId>
    {
        public override BranchId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            new(Guid.Parse(reader.GetString() ?? throw new JsonException("Expected branch id string.")));

        public override void Write(Utf8JsonWriter writer, BranchId value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value.Value.ToString("D"));

        public override BranchId ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            new(Guid.Parse(reader.GetString() ?? throw new JsonException("Expected branch id property name.")));

        public override void WriteAsPropertyName(Utf8JsonWriter writer, BranchId value, JsonSerializerOptions options) =>
            writer.WritePropertyName(value.Value.ToString("D"));
    }

    private sealed class TimelineNodeIdJsonConverter : JsonConverter<TimelineNodeId>
    {
        public override TimelineNodeId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            new(Guid.Parse(reader.GetString() ?? throw new JsonException("Expected node id string.")));

        public override void Write(Utf8JsonWriter writer, TimelineNodeId value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value.Value.ToString("D"));

        public override TimelineNodeId ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            new(Guid.Parse(reader.GetString() ?? throw new JsonException("Expected node id property name.")));

        public override void WriteAsPropertyName(Utf8JsonWriter writer, TimelineNodeId value, JsonSerializerOptions options) =>
            writer.WritePropertyName(value.Value.ToString("D"));
    }
}
