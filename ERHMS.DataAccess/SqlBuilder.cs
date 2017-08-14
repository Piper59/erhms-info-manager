using ERHMS.Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERHMS.DataAccess
{
    public class SqlBuilder
    {
        private static string Escape(string identifier)
        {
            return IDbConnectionExtensions.Escape(identifier);
        }

        private ICollection<string> separators;

        public IList<string> SelectClauses { get; private set; }
        public IList<string> FromClauses { get; private set; }
        public string OtherClauses { get; set; }

        public string SplitOn
        {
            get { return string.Join(", ", separators); }
        }

        public SqlBuilder()
        {
            SelectClauses = new List<string>();
            FromClauses = new List<string>();
            separators = new List<string>();
        }

        private string EscapeTableName(string tableName, string alias)
        {
            return alias == null ? Escape(tableName) : string.Format("{0} AS {1}", Escape(tableName), Escape(alias));
        }

        public void AddTable(string tableName, string alias = null)
        {
            SelectClauses.Add(Escape(alias ?? tableName) + ".*");
            FromClauses.Add(EscapeTableName(tableName, alias));
        }

        public void AddTable(JoinInfo join)
        {
            SelectClauses.Add(Escape(join.AliasTo ?? join.TableNameTo) + ".*");
            FromClauses.Add(string.Format(
                "{0} JOIN {1} ON {2}.{3} = {4}.{5}",
                join.JoinType.ToSql(),
                EscapeTableName(join.TableNameTo, join.AliasTo),
                Escape(join.AliasTo ?? join.TableNameTo),
                Escape(join.ColumnNameTo),
                Escape(join.TableNameOrAliasFrom),
                Escape(join.ColumnNameFrom)));
        }

        public void AddSeparator()
        {
            string separator = "Separator" + (separators.Count + 1);
            SelectClauses.Add("NULL AS " + Escape(separator));
            separators.Add(separator);
        }

        private string FormatSelectClauses()
        {
            return string.Join(", ", SelectClauses);
        }

        private string FormatFromClauses()
        {
            StringBuilder result = new StringBuilder();
            result.Append(string.Join(" ", FromClauses.Take(2)));
            for (int index = 2; index < FromClauses.Count; index++)
            {
                result.Insert(0, "(");
                result.Append(") " + FromClauses[index]);
            }
            return result.ToString();
        }

        public override string ToString()
        {
            return string.Format("SELECT {0} FROM {1} {2}", FormatSelectClauses(), FormatFromClauses(), OtherClauses).Trim();
        }
    }
}
