using System.Reflection;
using System.Text.Json.Serialization;

namespace sqljson.core
{
    public class SQLJson
    {
        [JsonPropertyName("flavor")]
        public Flavors Flavor { get; set; }

        [JsonPropertyName("schema")]
        public string? Schema { get; set; }

        [JsonPropertyName("select")]
        public SQLSelect Select { get; set; } = new SQLSelect();

        [JsonPropertyName("from")]
        public string From { get; set; } = string.Empty;

        [JsonPropertyName("alias")]
        public string? Alias { get; set; }

        [JsonPropertyName("join")]
        public SQLJoin[] Join { get; set; } = Array.Empty<SQLJoin>();

        [JsonPropertyName("where")]
        public SQLWhere[] Where { get; set; } = Array.Empty<SQLWhere>();

        [JsonPropertyName("order")]
        public SQLOrder[] Order { get; set; } = Array.Empty<SQLOrder>();

        public bool ValidateSQLJson()
        {
            // get all fields that are string and check for any spaces
            List<PropertyInfo> string_props = new();
            string_props.AddRange(this.GetType().GetProperties().Where(p => p.PropertyType == typeof(string)));
            string_props.AddRange(this.Select.GetType().GetProperties().Where(p => p.PropertyType == typeof(string)));
            foreach (SQLJoin join in this.Join)
            {
                string_props.AddRange(join.GetType().GetProperties().Where(p => p.PropertyType == typeof(string)));
                if (join.On != null)
                {
                    string_props.AddRange(join.On.GetType().GetProperties().Where(p => p.PropertyType == typeof(string)));
                }
            }
            string_props.AddRange(this.Where.GetType().GetProperties().Where(p => p.PropertyType == typeof(string)));

            return true;
        }
    }
}
