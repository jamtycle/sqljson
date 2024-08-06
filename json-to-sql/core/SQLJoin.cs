using System.Text.Json.Serialization;

namespace sqljson.core
{
    public class SQLJoin
    {
        [JsonPropertyName("schema")]
        public string Schema { get; set; } = string.Empty;
        [JsonPropertyName("with")]
        public string With { get; set; } = string.Empty;
        [JsonPropertyName("alias")]
        public string? Alias { get; set; }
        [JsonPropertyName("local")]
        public string? Local { get; set; }
        [JsonPropertyName("foreign")]
        public string? Foreign { get; set; }
        [JsonPropertyName("type")]
        public SQLJoinTypes Type { get; set; } = SQLJoinTypes.INNER;
        [JsonPropertyName("on")]
        public SQLJoinOn? On { get; set; }
    }

    public class SQLJoinOn
    {
        [JsonPropertyName("local")]
        public string Local { get; set; } = string.Empty;
        [JsonPropertyName("foreign")]
        public string Foreign { get; set; } = string.Empty;
    }
}