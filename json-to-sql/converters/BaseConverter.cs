using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using sqljson.core;

namespace sqljson.converters
{
    public abstract class BaseConverter : ISQLJsonConverter
    {
        private readonly string? raw_json;
        private string? error_message;
        private SQLJson? sqljson = null;

        public BaseConverter(string _filename)
        {
            try
            {
                raw_json = File.ReadAllText(_filename);
            }
            catch (Exception e)
            {
                error_message = e.Message;
            }
        }

        public BaseConverter(string _filename, Encoding _encoding)
        {
            try
            {
                raw_json = File.ReadAllText(_filename, _encoding);
            }
            catch (Exception e)
            {
                error_message = e.Message;
            }
        }

        public virtual string ProcessJson()
        {
            if (raw_json == null) return GetPrematureOutput();

            sqljson = ParseJson(raw_json);
            if (sqljson == null) return GetPrematureOutput();

            bool valid = sqljson.ValidateSQLJson();
            if (!valid) return "Invalid SQLJson object, there is spaces in the fields";

            return string.Empty;
        }

        private SQLJson? ParseJson(string _json_data)
        {
            try
            {
                return JsonSerializer.Deserialize<SQLJson>(_json_data, new JsonSerializerOptions()
                {
                    AllowTrailingCommas = true,
                });
            }
            catch (Exception e)
            {
                error_message = e.Message;
                return null;
            }
        }

        private string GetPrematureOutput()
        {
            if (error_message != null)
                return error_message;
            else return string.Empty;
        }

        public virtual string GenerateSelect()
        {
            throw new NotImplementedException();
        }

        public virtual string GenerateFrom()
        {
            throw new NotImplementedException();
        }

        public virtual string GenerateWhere()
        {
            throw new NotImplementedException();
        }

        public virtual string GenerateGroupBy()
        {
            throw new NotImplementedException();
        }

        public virtual string GenerateOrderBy()
        {
            throw new NotImplementedException();
        }

        protected bool IsColumnValid(string _column)
        {
            if (sqljson == null) return false;

            string[] parts = _column.Split(".");
            if (parts.Length != 2) return false;

            if (sqljson.From.Equals(parts[0])) return true;
            if (sqljson.Alias == parts[0]) return true;

            if (sqljson.Join.Any(x => x.Alias == parts[0] || x.With.Equals(parts[0]))) return true;

            return false;
        }

        public SQLJson? SQLJson => sqljson;
    }
}
