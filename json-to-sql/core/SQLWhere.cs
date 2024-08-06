using System.Text.Json;
using System.Text.Json.Serialization;

namespace sqljson.core
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SQLWhereOperators
    {
        [JsonPropertyName("between")]
        BETWEEN,
        [JsonPropertyName("in")]
        IN,
        [JsonPropertyName("nin")]
        NOT_IN,
        [JsonPropertyName("like")]
        LIKE,
        [JsonPropertyName("nlike")]
        NOT_LIKE,
        [JsonPropertyName("gt")]
        GT,
        [JsonPropertyName("gte")]
        GTE,
        [JsonPropertyName("lt")]
        LT,
        [JsonPropertyName("lte")]
        LTE,
        [JsonPropertyName("eq")]
        EQ,
        [JsonPropertyName("neq")]
        NEQ
    }

    public static class SQLWhereOperatorsExtensions
    {
        public static string ToSQLString(this SQLWhereOperators? _sql_operator)
        {
            return _sql_operator switch
            {
                SQLWhereOperators.BETWEEN => "BETWEEN",
                SQLWhereOperators.IN => "IN",
                SQLWhereOperators.NOT_IN => "NOT IN",
                SQLWhereOperators.LIKE => "LIKE",
                SQLWhereOperators.NOT_LIKE => "NOT LIKE",
                SQLWhereOperators.GT => ">",
                SQLWhereOperators.GTE => ">=",
                SQLWhereOperators.LT => "<",
                SQLWhereOperators.LTE => "<=",
                SQLWhereOperators.EQ => "=",
                SQLWhereOperators.NEQ => "!=",
                _ => string.Empty,
            };
        }
    }

    public enum SQLWhereConnectors
    {
        AND,
        OR
    }

    public class SQLWhere
    {
        [JsonPropertyName("source")]
        public string Source { get; set; } = string.Empty;
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("operator")]
        public SQLWhereOperators? Operator { get; set; }
        [JsonPropertyName("value")]
        public JsonElement? Value { get; set; }

        [JsonPropertyName("connector")]
        public string? Connector { get; set; }

        public string GetWhereTerm()
        {
            if (this.Value == null) return string.Empty;
            // TODO: Fix flavors
            var value = ExtractValue(this.Value.Value, Flavors.postgresql);
            if (value == null) return string.Empty;

            string fvalue = JoinWhereTerm(this.Operator, value);
            if (fvalue == string.Empty) return string.Empty;


            return $"{fvalue} {this.Connector ?? string.Empty}";
        }

        public static object ExtractValue(JsonElement _value, Flavors _flavor)
        {
            return _value.ValueKind switch
            {
                JsonValueKind.String => $"'{_value.GetString()}'",
                JsonValueKind.Number => $"{_value.GetDouble()}",
                JsonValueKind.True or JsonValueKind.False => _flavor switch
                {
                    Flavors.postgresql => _value.GetBoolean() ? "TRUE" : "FALSE",
                    _ => _value.GetBoolean() ? "1" : "0"
                },
                JsonValueKind.Null => "NULL",
                JsonValueKind.Array => _value.EnumerateArray().Select((x) => ExtractValue(x, _flavor)).ToArray(),
                _ => string.Empty
            };
        }

        public static string JoinWhereTerm(SQLWhereOperators? _operator, object _value)
        {
            string op = _operator.ToSQLString();
            switch (_operator)
            {
                case SQLWhereOperators.BETWEEN:
                    return $"{op} {ConnectBetween((object[])_value)}";
                case SQLWhereOperators.IN:
                case SQLWhereOperators.NOT_IN:
                    return $"{op} {ConnectIn((object[])_value)}";
                default:
                    return $"{op} {_value}";
            }
        }

        public static string ConnectIn(object[] _values)
        {
            string[] values = new string[_values.Length];
            int i = 0;
            foreach (object val in _values)
            {
                // TODO: Sanitize string [val]
                values[i] = val switch
                {
                    string s => $"'{s}'",
                    int n => $"{n}",
                    float f => $"{f}",
                    double d => $"{d}",
                    _ => string.Empty
                };

                i++;
            }

            return $"({string.Join(", ", values)})";
        }

        public static string ConnectBetween(object[] _values)
        {
            if (_values.Length != 2) return string.Empty;

            List<string> values = new();
            foreach (string val in _values.Cast<string>())
            {
                // TODO: Sanitize string [val]
                values.Add(val);
            }

            return string.Join(" AND ", values);
        }
    }
}
