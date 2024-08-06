using System.Text;

namespace sqljson.core
{
    public interface ISQLJsonConverter
    {
        public string ProcessJson();
        
        string GenerateSelect();
        string GenerateFrom();
        string GenerateWhere();
        string GenerateGroupBy();
        string GenerateOrderBy();
    }
}
