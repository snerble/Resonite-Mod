using JetBrains.Annotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Snerble.Utilities.Json;

[PublicAPI]
public class JsonDateTimeEpochSecondsConverter : JsonConverter<DateTime>
{
	public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return reader.TryGetInt64(out var seconds)
			? DateTimeOffset.FromUnixTimeSeconds(seconds).LocalDateTime
			: default;
	}

	public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
	{
		var seconds = new DateTimeOffset(value).ToUnixTimeSeconds();
		writer.WriteNumberValue(seconds);
	}
}
