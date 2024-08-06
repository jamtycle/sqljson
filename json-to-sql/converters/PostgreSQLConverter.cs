using System.Text.Json;
using sqljson.core;

namespace sqljson.converters
{
    public class PostgreSQLConverter : BaseConverter
    {
        private readonly List<string> non_grouping_columns = new();
        private readonly List<string> grouping_columns = new();

        public PostgreSQLConverter(string _filename) : base(_filename)
        {

        }

        public override string ProcessJson()
        {
            string pre_process = base.ProcessJson();
            if (pre_process != string.Empty) return pre_process;
            if (this.SQLJson == null) return "SQLJson object is null";

            if (this.SQLJson.Flavor != Flavors.postgresql) return "Invalid JSON flavor";

            string[] clauses = new string[] { GenerateSelect(), GenerateFrom(), GenerateWhere(), GenerateGroupBy(), GenerateOrderBy() };
            string sql = string.Join("\n", clauses.Where(x => x != string.Empty));

            return sql;
        }

        public override string GenerateSelect()
        {
            if (this.SQLJson == null) return string.Empty;
            string select_statement = "SELECT ";

            if (this.SQLJson.Select.Distinct) select_statement += "DISTINCT ";

            List<string> columns = new();
            int i = 0;
            foreach (JsonElement column in SQLJson.Select.Columns.Select(v => (JsonElement)v))
            {
                switch (column.ValueKind)
                {
                    case JsonValueKind.String:
                        string v = column.GetString() ?? string.Empty;
                        if (!this.IsColumnValid(v))
                        {
                            Console.WriteLine("[WARN]: Invalid column: " + v);
                            continue;
                        }

                        columns.Add(v);
                        non_grouping_columns.Add(v);
                        break;
                    case JsonValueKind.Object:
                        Console.WriteLine();
                        if (column.TryGetProperty("group", out JsonElement gcol))
                        {
                            SQLSelectGroup? group = column.Deserialize<SQLSelectGroup>();
                            if (group == null)
                            {
                                Console.WriteLine($"[WARN]: Invalid group column: {gcol}");
                                continue;
                            }

                            columns.Add(group.ToString());
                            grouping_columns.Add(group.ToString());
                            break;
                        }

                        SQLSelectColumn? col = column.Deserialize<SQLSelectColumn>();
                        if (col == null)
                        {
                            Console.WriteLine($"[WARN]: Invalid column: {column}");
                            continue;
                        }

                        columns.Add(col.ToString());
                        non_grouping_columns.Add(col.ToString(false));
                        break;
                    default:
                        Console.WriteLine("[SKIP]: Invalid column: " + column.ToString());
                        break;
                }

                i++;
            }

            select_statement += string.Join(", ", columns);

            return select_statement;
        }

        public override string GenerateFrom()
        {
            if (this.SQLJson == null) return string.Empty;
            string table_name = this.SQLJson.Alias ?? this.SQLJson.From;
            string form_statement = $"FROM {(this.SQLJson.Alias == null ? this.SQLJson.From : $"{this.SQLJson.From} AS {this.SQLJson.Alias}")}";

            foreach (SQLJoin join in this.SQLJson.Join)
            {
                string with = join.With;
                string alias = join.Alias == null ? string.Empty : $"AS {join.Alias}";
                string keys = $"{table_name}.{join.Local} = {alias}.{join.Foreign}";
                SQLJoinTypes join_tytpe = join.Type;

                form_statement += $" {join_tytpe} JOIN {with} {alias} ON {keys}";
            }

            return form_statement;
        }

        public override string GenerateWhere()
        {
            if (this.SQLJson == null) return string.Empty;
            if (this.SQLJson.Where.Last().Connector != null)
            {
                Console.WriteLine("[WARN]: Last connector in WHERE clause is not needed");
                this.SQLJson.Where.Last().Connector = null;
            }

            List<string> where_clauses = new();
            foreach (SQLWhere where in this.SQLJson.Where)
            {
                string right = $"{where.Source}.{where.Name}";

                if (!this.IsColumnValid(right))
                {
                    Console.WriteLine("[WARN]: Invalid column: " + $"{right}");
                    continue;
                }

                where_clauses.Add($"{right} {where.GetWhereTerm()}");
            }

            return $"WHERE {string.Join(" ", where_clauses)}";
        }

        public override string GenerateGroupBy()
        {
            if (this.SQLJson == null) return string.Empty;
            if (this.grouping_columns.Count == 0) return string.Empty;

            string group_by_statement = "GROUP BY ";
            group_by_statement += string.Join(", ", this.non_grouping_columns);

            return group_by_statement;
        }

        public override string GenerateOrderBy()
        {
            if (this.SQLJson == null) return string.Empty;
            string order_by_statement = "ORDER BY ";

            var valid_orders = this.SQLJson.Order.Where(x => this.IsColumnValid($"{x.Source}.{x.Name}"));
            order_by_statement += string.Join(", ", valid_orders.Select(x => $"{x.Source}.{x.Name} {x.Type}"));

            return order_by_statement;
        }
    }
}
