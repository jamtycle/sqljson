using System.Text.Json.Serialization;

namespace sqljson.core
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SQLGroupBy
    {
        COUNT,
        SUM,
        AVG,
        MIN,
        MAX
    }
}
