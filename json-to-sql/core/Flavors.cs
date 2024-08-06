using System.Text.Json.Serialization;

namespace sqljson.core
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Flavors
    {
        postgresql,
        mysql,
        mssql,
        sqlite
    }
}
