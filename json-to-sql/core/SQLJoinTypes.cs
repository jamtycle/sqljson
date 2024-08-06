using System.Text.Json.Serialization;

namespace sqljson.core
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SQLJoinTypes
    {
        INNER,
        LEFT,
        RIGHT,
        FULL
    }
}
