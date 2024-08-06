using System.Text.Json.Serialization;

namespace sqljson.core
{
    public class SQLSelectGroup : SQLSelectColumn
    {
        [JsonPropertyName("group")]
        public SQLGroupBy Group { get; set; }
        [JsonPropertyName("distinct")]
        public bool Distinct { get; set; } = false;

        public override string ToString()
        {
            if (this.Expr == null) return $"{this.Group}({this.Source}.{this.Name}){this.GetAlias()}";

            return $"{this.Group}({this.Expr}){this.GetAlias()}";
        }
    }

    public class SQLSelectExpression
    {
        [JsonPropertyName("operator")]
        public string? Operator { get; set; }
        [JsonPropertyName("terms")]
        public string[] Terms { get; set; } = Array.Empty<string>();

        public override string ToString()
        {
            if (this.Operator == null) return string.Join(" ", this.Terms);
            string op = this.Operator;

            List<string> terms = new();
            foreach (string t in this.Terms)
            {
                // TODO: Check if the term is a valid column
                if (t.StartsWith('\'') && !t.EndsWith('\''))
                {
                    Console.WriteLine($"[WARN]: Invalid expression: {t}");
                    continue;
                }

                terms.Add(t);
            }


            return $"{string.Join($" {op} ", terms)}";
        }
    }

    public class SQLSelectColumn
    {
        [JsonPropertyName("source")]
        public string? Source { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("expr")]
        public SQLSelectExpression? Expr { get; set; }
        [JsonPropertyName("alias")]
        public string? Alias { get; set; } = string.Empty;

        protected string GetAlias() => !string.IsNullOrWhiteSpace(Alias) && !string.IsNullOrEmpty(Alias) ? $" AS {this.Alias}" : string.Empty;

        public override string ToString()
        {
            if (this.Expr == null) return $"{this.Source}.{this.Name}{this.GetAlias()}";

            return $"{this.Expr}{this.GetAlias()}";
        }

        public string ToString(bool _alias)
        {
            if (this.Expr == null) return $"{this.Source}.{this.Name}{(_alias ? this.GetAlias() : string.Empty)}";

            return $"{this.Expr}{(_alias ? this.GetAlias() : string.Empty)}";
        }
    }

    public class SQLSelect
    {
        [JsonPropertyName("distinct")]
        public bool Distinct { get; set; }
        /// <summary>
        /// Columns to select, this can be a string or an SQLSelectColumn object
        /// </summary>
        [JsonPropertyName("columns")]
        public object[] Columns { get; set; } = Array.Empty<object>();
    }
}
