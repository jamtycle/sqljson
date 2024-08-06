using System.Text.Json.Serialization;

namespace sqljson.core
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SQLOrderTypes
    {
        ASC,
        DESC
    }

    public class SQLOrder
    {
        [JsonPropertyName("source")]
        public string Source { get; set; } = string.Empty;
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("type")]
        public SQLOrderTypes Type { get; set; } = SQLOrderTypes.ASC;
    }
}
